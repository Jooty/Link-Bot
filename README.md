![Header](https://i.ibb.co/sWwTrt4/header.jpg)
[![Center](https://i.ibb.co/bKZ6rWb/center.jpg)]()
[![Footer](https://i.ibb.co/6ZDFvT7/footer.jpg)](https://discordapp.com/api/oauth2/authorize?client_id=401994184602681344&permissions=8&scope=bot)

## Still under major development.

#### A modular Discord bot focused on maximum accessibility.

This project is opened sourced, edit the code to your will, or host a version of the bot yourself.

If you commit another command, please source to a service if the command may take some time to process.
```cs
[Command("yourcommand")]
[Summary("do something!")]
public async Task TestCommand(string test) 
  => YourService.Test(Context, test);
```


There is also an internal reply service, which you should use to respond to users.
```cs
public static async Task SendResponse(SocketCommandContext Context, string response)
```
