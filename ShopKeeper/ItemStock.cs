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
			int index = items.FindIndex(item => item.name.ToUpper().Equals(name.Trim().ToUpper()));
			if(index >= 0)
				return false;


			items.Add(new Item(name.Trim(), price, stock));

			items = items.OrderBy(x => x.name).ThenBy(x => x.price).ToList();

			printToFile();
			return true;
		}

		public bool removeItem(string itemName)
		{
			int index = items.FindIndex(item => item.name.ToUpper().Equals(itemName.Trim().ToUpper()));
			if(index == -1)
			{
				return false;
			}

			items.RemoveAt(index);
			printToFile();
			return true;
		}

		// -2 means it failed due to not existing
		// -1 means that it failed due to being out of stock
		// Anything else returns the price and is success
		public int buyItem(string itemName, int itemsBought)
		{
			int index = items.FindIndex(item => item.name.ToUpper().Equals(itemName.Trim().ToUpper()));
			if (index == -1)
			{
				return -2;
			}

			var stock = Int32.Parse(items.ElementAt(index).stock);
			if (stock - itemsBought < 0)
				return -1;
			
			// The blank "" at the start auto casts the int to a string
			items.ElementAt(index).stock = "" + (stock - itemsBought);
			return Int32.Parse(items.ElementAt(index).price) * itemsBought;
		}

		private void printToFile()
		{
			string json = JsonConvert.SerializeObject(items);
			Debug.WriteLine(json);

			System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "shopItems.txt", json);
		}

		public string displayStock()
		{
			StringBuilder s = new StringBuilder();
			s.AppendLine(String.Format("{0} | {1} | {2}", "Name".PadRight(10), "Stock".PadRight(5), "Price"));
			s.AppendLine();
			for (int i = 0; i < items.Count; i++)
			{	
				Item item = items.ElementAt(i);
				s.AppendLine(String.Format("{0} | {1} | {2}", item.name.PadRight(10), item.stock.ToString().PadRight(5), item.price));

			}
			return s.ToString();
		}

		

		override
		public string ToString()
		{
			StringBuilder s = new StringBuilder();
			for(int i = 0; i < items.Count; i++)
			{
				Item item = items.ElementAt(i);
				s.Append(item + "\n");
			}
			return s.ToString();
		}
	}
}
