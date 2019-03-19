using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using YoutubeSearch;
using VideoLibrary;
using MediaToolkit.Model;
using MediaToolkit;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Link
{
    public class AudioClientWrapper
    {
        public IAudioClient Client { get; set; }
        public ulong GuildId { get; set; }
        public AudioQueueEntry CurrentlyPlaying { get; set; }
        public List<AudioQueueEntry> Queue = new List<AudioQueueEntry>();
        public IUserMessage PlayerMessage { get; set; }

        private float playingVolume = 0.5f;
        private int pause = 0;

        private readonly object cancelLock = new object();

        private Emoji playEmoji = new Emoji("▶");
        private Emoji pauseEmoji = new Emoji("⏸");
        private Emoji ffEmoji = new Emoji("⏩");

        private SocketCommandContext context;

        private bool calledForSkip = false;

        private CancellationTokenSource cancelToken = new CancellationTokenSource();
        private CancellationTokenSource pauseToken = new CancellationTokenSource();

        private Process ffmpeg;

        public AudioClientWrapper(SocketCommandContext Context) 
            => Task.Run(() 
                => Initialize(Context));

        public async Task Initialize(SocketCommandContext Context)
        {
            var _voiceState = Context.User as IVoiceState;
            if (_voiceState.VoiceChannel == null) return;

            context = Context;

            Client = await _voiceState.VoiceChannel.ConnectAsync();
            GuildId = Context.Guild.Id;
            PlayerMessage = await SendMusicEmbedAsync(Context.Channel);

            await PlayQueueAsync();
        }

        public async Task AddToQueue(SocketCommandContext Context, string song)
        {
            var _result = Search(song);
            if (_result == null)
            {
                await Respond.SendResponse(context, $"Could not find a video by search: `{song}`");
                return;
            }

            if (_result.Duration.Where(s => s == ':').Count() >= 2)
            {
                await Respond.SendResponse(context, "Sorry, videos over an hour long are currently not supported. ;(");
                return;
            }
            else if (Queue.Count + 1 > 10)
            {
                await Respond.SendResponse(context, "Sorry, only 10 videoes in the queue at a time is supported. ;(");
                return;
            }

            Queue.Add(new AudioQueueEntry(_result, Context.User));

            await RefreshEmbedAsync();
            Respond.SendTimedResponse(context, $"**{_result.Title}** added to the queue. Index: **{Queue.Count}**", TimeSpan.FromSeconds(5));
        }

        public void Skip()
        {
            if (Queue.Count != 0)
            {
                calledForSkip = true;
                Stop();
            }
        }

        public void Stop()
        {
            lock (cancelLock)
            {
                if (IsPlaying())
                {
                    using (cancelToken)
                    {
                        cancelToken.Cancel();
                    }
                    cancelToken = new CancellationTokenSource();
                }

                CurrentlyPlaying = null;
                RefreshEmbedAsync();
            }
        }

        public Task Pause()
        {
            if (Interlocked.Exchange(ref pause, value: 1) == 0)
            {
                using (pauseToken)
                {
                    pauseToken.Cancel();
                }
                pauseToken = new CancellationTokenSource();

                return RefreshEmbedAsync();
            }
            return Task.CompletedTask;
        }

        public Task Resume()
        {
            if (Interlocked.Exchange(ref pause, value: 0) == 1)
            {
                using (pauseToken)
                {
                    pauseToken.Cancel();
                }
                pauseToken = new CancellationTokenSource();
                return RefreshEmbedAsync();
            }
            return Task.CompletedTask;
        }

        public bool IsPlaying()
        {
            return !(ffmpeg == null || ffmpeg.HasExited);
        }

        public async Task Remove(SocketCommandContext Context, int index)
        {
            if (Queue[index] == null) return;

            Queue.RemoveAt(index);

            await RefreshEmbedAsync();
        }

        private VideoInformation Search(string query)
        {
            return new VideoSearch().SearchQuery(query, 1).FirstOrDefault();
        }

        private async Task<string> DownloadAsync(string url)
        {
            try
            {
                var _source = $@"{Directory.GetCurrentDirectory()}\Music\";
                var _youtube = YouTube.Default;
                var _video = await _youtube.GetVideoAsync(url);
                File.WriteAllBytes($@"{_source}{_video.FullName}", await _video.GetBytesAsync());

                var _inputfile = new MediaFile { Filename = _source + _video.FullName };
                var _outputfile = new MediaFile { Filename = $"{_source + _video.FullName}.mp3" };

                using (var engine = new Engine())
                {
                    engine.GetMetadata(_inputfile);

                    engine.Convert(_inputfile, _outputfile);
                }

                File.Delete(_inputfile.Filename);

                return _outputfile.Filename;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        private async Task<IUserMessage> SendMusicEmbedAsync(ISocketMessageChannel channel)
        {
            var _embed = new EmbedBuilder()
                .WithColor(Color.Blue)
                .WithTitle("Music player")
                .WithDescription("Not playing anything! Use `>play <link/search>`.");

            var _message = await channel.SendMessageAsync("", false, _embed.Build());

            //await _message.AddReactionAsync(playEmoji);
            //await _message.AddReactionAsync(pauseEmoji);
            //await _message.AddReactionAsync(ffEmoji);

            return _message;
        }

        private async Task RefreshEmbedAsync()
        {
            var _embed = new EmbedBuilder()
                .WithTitle($"{(pause > 0 ? "⏸" : "▶")} Music Player")
                .WithColor(Color.Blue);

            if (CurrentlyPlaying == null)
            {
                _embed.WithDescription("Not playing anything! Use `>play <link/search>`.");
                return;
            }

            _embed.WithThumbnailUrl(CurrentlyPlaying.VideoInformation.Thumbnail);

            // Create up next description
            if (Queue.Count > 1)
            {
                StringBuilder _builder = new StringBuilder();
                int _i = 1;
                foreach (var song in Queue)
                {
                    if (_i == 1)
                    {
                        _i++;
                        continue;
                    }

                    _builder.Append($"\n{_i - 1}: " +
                        $"{(song.VideoInformation.Title.Count() > 35 ? (song.VideoInformation.Title.Substring(0, 35) + "*...*") : song.VideoInformation.Title)}" +
                        $"\tBy: `{song.User.Username}#{song.User.Discriminator}`");
                    _i++;
                }
                _embed.Description = $"__Currently Playing__: **{CurrentlyPlaying.VideoInformation.Title}** By: `{CurrentlyPlaying.User.Username}`" 
                    + "\n\n__Up Next__:" 
                    + _builder.ToString();
            }
            else
            {
                _embed.Description = $"__Currently Playing__: **{CurrentlyPlaying.VideoInformation.Title}**";
            }

            await PlayerMessage.ModifyAsync(s => s.Embed = _embed.Build()).ConfigureAwait(false);
        }

        private async Task PlayQueueAsync()
        {
            while (true)
            {
                if (IsPlaying()) continue;
                if (Queue.Count == 0) continue;

                CurrentlyPlaying = Queue.First();
                var _path = await DownloadAsync(CurrentlyPlaying.VideoInformation.Url);

                await SendAudioAsync(_path);

                Queue.Remove(Queue.First());
                File.Delete(_path);

                CurrentlyPlaying = Queue.First();
                await RefreshEmbedAsync();
            }
        }

        private async Task SendAudioAsync(string fileName)
        {
            using (ffmpeg = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel panic -i \"{fileName}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = false
            }))
            using (var stream = Client.CreatePCMStream(AudioApplication.Music))
            {
                try
                {
                    await PausableCopyToAsync(ffmpeg.StandardOutput.BaseStream, stream, 81920).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    if (!calledForSkip)
                    {
                        Queue.Remove(Queue.First());
                    }
                    else
                    {
                        calledForSkip = false;
                    }
                }
                finally
                {
                    await stream.FlushAsync().ConfigureAwait(false);
                }
            }
        }

        // Credit: "Joe4evr"
        private async Task PausableCopyToAsync(Stream source, Stream destination, int buffersize)
        {
            Contract.Requires(source != null && source.CanRead, $"{nameof(source)} is null or not readable.");
            Contract.Requires(destination != null && destination.CanWrite, $"{nameof(destination)} is null or not writable.");
            Contract.Requires(buffersize > 0 && IsEvenBinaryOp(buffersize), $"{nameof(buffersize)} is 0 or less or not even.");

            byte[] buffer = new byte[buffersize];
            int count;

            while ((count = await source.ReadAsync(buffer, 0, buffersize, cancelToken.Token).ConfigureAwait(false)) > 0)
            {
                if (pause > 0)
                {
                    try
                    {
                        await Task.Delay(Timeout.Infinite, pauseToken.Token);
                    }
                    catch (OperationCanceledException) { }
                }

                await destination.WriteAsync(buffer, 0, count, cancelToken.Token).ConfigureAwait(false);
            }
        }

        // Credit: "Joe4evr"
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsEvenBinaryOp(int number)
        {
            const int magic = int.MaxValue - 1;
            return (number | magic) == magic;
        }
    }
}
