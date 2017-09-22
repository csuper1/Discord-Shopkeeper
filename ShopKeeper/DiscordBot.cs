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

		// Putting these strings in an easy place to edit
		private static readonly string currencyName = "gold pieces";

		// That way I can call these instead of writing it inside of the code when I want it
		private static readonly string[] greetings = {	"Hello!", "It's so nice to see you again.",
			"I'm happy that you came to me!", "It's always a wonderful day to shop!",
			"Zzz... Huh, what? No, I wasn't sleeping!", "Aah! You scared me."};

		// This way if I want to change something, it will update everywhere else that I call one of these
		private static readonly string[] questions = {	"How can I help you today? Is there anything you'd like?",
			"Please, enjoy the store. Let me know if you want anything!", "Is there anything I can help you find today?",
			"I'm happy to help if you find what you're looking for!",
			"Here's what I've got in store for you today. Let me know if something catches your eye!" };

		private static readonly string[] rumors = {	"Rumor Placehodler 1.", "Rumor Placeholder 2.",
			"Rumor Placeholder 3.", "Rumor Placeholder 4.", "Rumor Placeholder 5."};

		// Discord commands are weird, but to make one, you folllow this set up.
		// All you REALLY need is the Command portion. A summary is like a tooltip for 
		// what your command does, and an alias is another way you can call this command
		// without typing out the entire thing. It's like shorthand.
		[Command ("hello")]
		[Summary ("say hello!")]
		[Alias   ("hi")]
		public async Task Hello()
		{
			// all this does is say "put something in the channel where this called,
			// but do it as an asynchronous task. 
			// Async is beneficial because it basically makes a job on the side.
			// If a task takes a really long time, the bot should be able to run other commands while
			// it's still computing the other job.
			// If it wasn't async, the bot would be unresponsive while it's taking a long time with that 
			// other task.
			await ReplyAsync("hello " + Context.Message.Author.Mention);
		}

		[Command ("addItem")]
		[Summary ("Add an item to the shop. Takes [name stock price]")]
		// So here, whenever somebody types !addItem, it the bot automatically checks to see
		// if there's a function matched with the command. If there is and the arugments match,
		// then it runs this. Otherwise, if I were to put in !addItem cherry 15 asdf
		// in discord, then discord automatically spits back that price needs to be an integer
		public async Task addItem(string name, int stock, int price)
		{
			// These are all checks prior to creating an item.
			// These should be somewhat self explanatory
			if (name.Length > 10)
			{
				await ReplyAsync("Name cannot be longer than 10 characters");
				return;
			}
			if (price > 99999)
			{
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

			// addItem returns a boolean.
			// If that boolean is true, then it means addItem was successful.
			// If that boolean is false, then it means that there was already an item on the list
			if (items.addItem(name, price, stock))
				await ReplyAsync("Added item " + name + " to the stock.");
			else
				await ReplyAsync("Item " + name + " already exists in the shop.");
		}

		[Command ("showStock")]
		[Summary ("Show the stock's current stock.")]
		public async Task showStock()
		{
			// Create a new random number generator
			Random rng = new Random();
			// Using the number generator, select an item within the array and strap it all together to make the message made when !showstock is called
			string intro = greetings[rng.Next(0, greetings.Length)] + " " + rumors[rng.Next(0, rumors.Length)] + " " + questions[rng.Next(0, questions.Length)];
			await ReplyAsync(intro + "\n\n```" + items.displayStock() + "```");
		}

		[Command ("buyItem")]
		[Summary ("Buy one or many items")]
		public async Task buyItem(string name, int count)
		{
			// Try to buy the item. buyItem returns a number
			int result = items.buyItem(name, count);
			// If the number is -2, it means the item wasn't found.
			// I chose -2 arbitrarily. I like it because it's negative,
			// so the ONLY way the result is -2 is the specific condition
			// I defined in buyItem
			if(result == -2)
			{
				await ReplyAsync("I'm sorry. We don't sell " + name + " here!");
				return;
			}
			// Same thing. -1 doesn't mean anything other than being impossible unless
			// there was an error in trying to add an item
			if(result == -1)
			{
				await ReplyAsync("I don't have that many items in stock!");
				return;
			}

			await ReplyAsync("Thank you! That will be " + result + " " + currencyName);
		}

		// buyItem is overloaded.
		// That means that the program will -automatically- determine which one I'm trying to call
		// For example, saying !buyItem cherry will call this method because there is no second argument
		// Saying !buyItem cherry 5 will call the other function, because it has two argument sent in
		[Command ("buyItem")]
		public async Task buyItem(string name)
		{
			// Assume that the person buying it only wants to buy 1 item
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
			// removeItem returns a boolean
			// If that boolean is false, it means that we couldn't find the item in the list
			// NOTE: The exclamation point means NOT
			// So, if items.removeItems returned false, I want to make it NOT(false), therefore true
			// It needs to be true because then the if statement will run.
			// If statements ONLY run if the the expression turns out to be true.
			if (!items.removeItem(itemName))
			{
				await ReplyAsync("Could not remove " + itemName + " because it doesn't exist");
			}

			// If that boolean is true, then that means we 
			await ReplyAsync("Removed " + itemName);
		}

		private static ItemStock initializeItemStock()
		{
			// Make a string based off of the current directory, and search for the file "shopItems.txt"
			string fileName = AppDomain.CurrentDomain.BaseDirectory + "shopItems.txt";

			// Use the File class to see if the file exists
			if (!File.Exists(fileName))
			{
				// if the file doesn't exist, then we make an empty list
				return new ItemStock();
			}

			// Otherwise, read all the text into a string
			string jsonText = File.ReadAllText(fileName);

			// And convert that string into a list of Items.
			// Dont' worry about this part.
			List<Item> result = JsonConvert.DeserializeObject<List<Item>>(jsonText);

			// Make a new itemstock with our items
			return new ItemStock(result);
		}
	}
}
