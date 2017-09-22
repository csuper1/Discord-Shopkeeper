using System;
using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

public class Program
{
	private CommandService commands;
	private DiscordSocketClient client;
	private IServiceProvider services;

	static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

	public async Task Start()
	{
		client = new DiscordSocketClient(new DiscordSocketConfig{
			WebSocketProvider = Discord.Net.Providers.WS4Net.WS4NetProvider.Instance
			});
		commands = new CommandService();

		// This is my token for getting the bot to run, however, since it's up on a public
		// repository, I shoved it off to another cs file where I can pull from it and keep
		// it hidden.
		string token = new ShopKeeper.DiscordKey().token;

		services = new ServiceCollection()
				.BuildServiceProvider();

		await InstallCommands();

		await client.LoginAsync(TokenType.Bot, token);
		await client.StartAsync();

		client.Log += Log;
		await Task.Delay(-1);
	}

	public async Task InstallCommands()
	{
		// Hook the MessageReceived Event into our Command Handler
		client.MessageReceived += HandleCommand;
		// Discover all of the commands in this assembly and load them.
		await commands.AddModulesAsync(Assembly.GetEntryAssembly());
	}

	public async Task HandleCommand(SocketMessage messageParam)
	{
		// Don't process the command if it was a System Message
		var message = messageParam as SocketUserMessage;
		if (message == null) return;
		// Create a number to track where the prefix ends and the command begins
		int argPos = 0;
		// Determine if the message is a command, based on if it starts with '!' or a mention prefix
		if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) return;
		// Create a Command Context
		var context = new CommandContext(client, message);
		// Execute the command. (result does not indicate a return value, 
		// rather an object stating if the command executed successfully)
		var result = await commands.ExecuteAsync(context, argPos, services);
		if (!result.IsSuccess)
			await context.Channel.SendMessageAsync(result.ErrorReason);
	}

	private Task Log(LogMessage msg)
	{
		Console.WriteLine(msg.ToString());
		return Task.CompletedTask;
	}
}
