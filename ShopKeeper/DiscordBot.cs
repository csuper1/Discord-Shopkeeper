using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.IO;

namespace ShopKeeper
{	
	public class DiscordBot : ModuleBase
	{	
		// Recreate item list if we have on already
		private static ItemStock items = initializeItemStock();
		private static readonly string currencyName = "gold pieces";
		private static readonly string[] greetings = {	"Hello!",
														"It's so nice to see you again.",
														"I'm happy that you came to me!",
														"It's always a wonderful day to shop!",
														"Zzz... Huh, what? No, I wasn't sleeping!",
														"Aah! You scared me."};

		private static readonly string[] questions = {	"How can I help you today? Is there anything you'd like?",
														"Please, enjoy the store. Let me know if you want anything!",
														"Is there anything I can help you find today?",
														"I'm happy to help if you find what you're looking for!",
														"Here's what I've got in store for you today. Let me know if something catches your eye!"};

		private static readonly string[] rumors = {	"Rumor Placehodler 1.",
													"Rumor Placeholder 2.",
													"Rumor Placeholder 3.",
													"Rumor Placeholder 4.",
													"Rumor Placeholder 5."};

		

		[Command ("hello")]
		[Summary ("say hello!")]
		[Alias   ("hi")]
		public async Task Hello()
		{
			await ReplyAsync("hello " + Context.Message.Author.Mention);
		}

		[Command ("say")]
		[Summary ("why tho")]
		public async Task Say([Remainder, Summary("idfk")] string echo = null)
		{
			if (echo == null)
			{
				await ReplyAsync("Say something!");
			}
			else
			{
				await ReplyAsync(Context.User.Username + " says: " + echo);
			}
		}

		[Command ("addItem")]
		[Summary ("Add an item to the shop. Takes [name stock price]")]
		public async Task addItem(string name, int stock, int price)
		{
			if (name.Length > 10) {
				await ReplyAsync("Name cannot be longer than 10 characters");
				return;
			}
			if (price > 99999) {
				await ReplyAsync("Price cannot exceed 5 digits");
				return;
			}
			if(stock > 99999)
			{
				await ReplyAsync("Stock cannot exceed 5 digits");
				return;
			}
			if(price < 0)
			{
				await ReplyAsync("Price cannot be negative");
				return;
			}
			if(stock < 0)
			{
				await ReplyAsync("Stock cannot be negatige");
				return;
			}

			if (items.addItem(name, price, stock))
				await ReplyAsync("Added item " + name + " to the stock.");
			else
				await ReplyAsync("Item " + name + " already exists in the shop.");
		}

		[Command ("showStock")]
		[Summary ("Show the stock's current stock.")]
		public async Task showStock()
		{
			Random rng = new Random();
			string intro = greetings[rng.Next(0, greetings.Length)] + " " + rumors[rng.Next(0, rumors.Length)] + " " + questions[rng.Next(0, questions.Length)];
			await ReplyAsync(intro + "\n\n```" + items.displayStock() + "```");
		}

		[Command ("buyItem")]
		[Summary ("Buy one or many items")]
		public async Task buyItem(string name, int count)
		{
			int result = items.buyItem(name, count);
			if(result == -2)
			{
				await ReplyAsync("I'm sorry. We don't sell " + name + " here!");
				return;
			}
			if(result == -1)
			{
				await ReplyAsync("I don't have that many items in stock!");
				return;
			}

			await ReplyAsync("Thank you! That will be " + result + " " + currencyName);
		}

		[Command ("buyItem")]
		public async Task buyItem(string name)
		{
			int result = items.buyItem(name, 1);
			if (result == -2)
			{
				await ReplyAsync("I'm sorry. We don't sell " + name + " here!");
				return;
			}
			if (result == -1)
			{
				await ReplyAsync("I'm sorry. We're all out of that!");
				return;
			}

			await ReplyAsync("Thank you! That will be " + result + " " + currencyName);
		}


		[Command ("removeItem")]
		[Summary ("Remove an item stock by name")]
		public async Task removeItem(string itemName)
		{
			if (!items.removeItem(itemName))
			{
				await ReplyAsync("Could not remove " + itemName + " because it doesn't exist");
			}

			await ReplyAsync("Removed " + itemName);
		}

		private static ItemStock initializeItemStock()
		{
			string fileName = AppDomain.CurrentDomain.BaseDirectory + "shopItems.txt";
			if (!File.Exists(fileName))
			{
				return new ItemStock();
			}

			string jsonText = File.ReadAllText(fileName);
			var result = JsonConvert.DeserializeObject<List<Item>>(jsonText);
			return new ItemStock(result);
		}
	}
}
