using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Link
{
    public class HelpCommands : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        [Summary(".. This command.")]
        public async Task HelpCommand(string module = "")
        {
            if (module == "")
            {
                // List modules
                var _pages = new List<PagedEmbed.EmbedPage>();
                _pages.Add(new PagedEmbed.EmbedPage());

                var _modules = CommandHandlingService.commands.Modules;

                int _currentPage = 0;
                int _perPageCounter = 0;
                foreach (var _module in _modules)
                {
                    if (_perPageCounter == 7)
                    {
                        _perPageCounter = 0;
                        _currentPage++;

                        _pages.Add(new PagedEmbed.EmbedPage());
                    }

                    _perPageCounter++;

                    _pages[_currentPage].Fields.Add(_module.Name, _module.Summary ?? "No summary.");
                }

                new PagedEmbed(Context, _pages.ToArray(), "Help - All modules");
            }
            else
            {
                // Try to find module
                var _module = CommandHandlingService.commands.Modules.First(s => s.Name.ToLower() == module);
                if (_module == null)
                {
                    await Respond.SendResponse(Context,
                        $"I could not find a module with the name `{module}`. You can use `>help` to get a full list of modules.");
                    return;
                }

                // List commands
                var _pages = new List<PagedEmbed.EmbedPage>();
                _pages.Add(new PagedEmbed.EmbedPage());

                int _currentPage = 0;
                int _perPageCounter = 0;
                foreach (var command in _module.Commands)
                {
                    if (_perPageCounter == 7)
                    {
                        _perPageCounter = 0;
                        _currentPage++;

                        _pages.Add(new PagedEmbed.EmbedPage());
                    }

                    _perPageCounter++;

                    // Build parameters
                    StringBuilder _paramsBuilder = new StringBuilder();
                    foreach (var param in command.Parameters)
                    {
                        _paramsBuilder.Append($" <{param.Name}>");
                    }

                    _pages[_currentPage]
                        .Fields
                        .Add($"{command.Name} {_paramsBuilder.ToString()}", command.Summary ?? "No summary.");
                }

                new PagedEmbed(Context, _pages.ToArray(), $"Help - {_module.Name}");
            }
        }
    }
}
