using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopKeeper
{
	class ItemStock
	{
		// A list is a container that I can put things into.
		// In this case, I'm specifically saying I want 
		// a list of "Item". Not string, not int. "Item"
		private List<Item> items;

		// Constructor using a pre-defined list. 
		// This will be called when we have items
		// stored in a file already
		public ItemStock(List<Item> items)
		{
			this.items = items;
		}

		// Default constructor
		public ItemStock() {
			items = new List<Item>();
		}

		public bool addItem(string name, int price, int stock)
		{
			// If we have a copy of our item already in stock
			// To make sure we're talking about the same item. I use .toUpper() to change the string into all caps,
			// That way I can compare the names regardless of what the user input is in regards to capitalization
			int index = items.FindIndex(item => item.name.ToUpper().Equals(name.Trim().ToUpper()));
			if(index >= 0)
				return false;

			// Take the name, and then make sure there isn't any white space left or right
			items.Add(new Item(name.Trim(), price, stock));

			// Order the items by their name; if two things are equal by name, order it by price next
			items = items.OrderBy(x => x.name).ThenBy(x => x.price).ToList();

			// Save the item stock since there's a new item
			printToFile();
			return true;
		}

		public bool removeItem(string itemName)
		{
			int index = items.FindIndex(item => item.name.ToUpper().Equals(itemName.Trim().ToUpper()));
			
			// If the item wasn't found, index will be -1
			if(index == -1)
			{
				return false;
			}

			// otherwise index will be an index in our list
			items.RemoveAt(index);
			// Save the item stock since we removed an item
			printToFile();
			return true;
		}

		// -2 means it failed due to not existing
		// -1 means that it failed due to being out of stock
		// Anything else returns the price and is success
		public int buyItem(string itemName, int itemsBought)
		{
			int index = items.FindIndex(item => item.name.ToUpper().Equals(itemName.Trim().ToUpper()));

			// If we couldn't find the item we want to buy
			if (index == -1)
			{
				return -2;
			}

			// since stock is a string, we need to convert to an integer
			int stock = Int32.Parse(items.ElementAt(index).stock);

			// If the we want to buy more items than the stock
			if (stock - itemsBought < 0)
				return -1;
			
			// The blank "" at the start auto casts the int to a string
			items.ElementAt(index).stock = "" + (stock - itemsBought);

			// Save to our file.
			printToFile();

			// return the total price of the items
			return Int32.Parse(items.ElementAt(index).price) * itemsBought;
		}

		private void printToFile()
		{
			// Use JSONconvert to turn this into a JSON object.
			// Don't worry about this, really. It's just an easy way of storing key,value pairs.
			string json = JsonConvert.SerializeObject(items);
			// Write to our current directory plus the file name,								 the string we're writing
			System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "shopItems.txt", json);
		}

		public string displayStock()
		{
			StringBuilder s = new StringBuilder();
			// String format works by taking the things in brackets and replacing it by the things on the right.
			// So {0} gets replaced with "Name.padRight(10)", {1} with "Stock.PadRight(5)", {2} with "Price
			// Pad right adds spaces to the string if it doesn't have 10 characters.
			// The point of this is to make the shop items look pretty 
			// This is what makes the bot say:
			// Name      | Stock | Price
			s.AppendLine(String.Format("{0} | {1} | {2}", "Name".PadRight(10), "Stock".PadRight(5), "Price"));

			// Make a new line before showing our items
			s.AppendLine();

			// For every item that we have in our list
			for (int i = 0; i < items.Count; i++)
			{	
				// Make a temp item for readability
				Item item = items.ElementAt(i);

				// Same concept as before, but now we're listing off each and every item and making it line up with the table headers
				s.AppendLine(String.Format("{0} | {1} | {2}", item.name.PadRight(10), item.stock.ToString().PadRight(5), item.price));

			}
			return s.ToString();
		}

		// This overrides a standard object's "toString" method, which is automatically called if 
		// I try to print this object.
		override
		public string ToString()
		{
			// Generally, trying to print out an object doesn't really give you what you want.
			StringBuilder s = new StringBuilder();

			// So I make my own method of doing it so I can see what I want if I try print this object
			// It's nice because then I can just call this every time I want a string representation of
			// this object
			for(int i = 0; i < items.Count; i++)
			{
				
				Item item = items.ElementAt(i);
				// each object Item has its own toString as well.
				s.Append(item + "\n");
			}
			return s.ToString();
		}
	}
}
