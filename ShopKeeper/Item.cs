using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace ShopKeeper
{
	class Item
	{
		// This is to make my stuff compatible with reading from the file
		[JsonProperty("name")]
		public string name { get; set; }

		// saying public string {get; set;} is a short hand way of making
		// getters and setters so I don't have to explicitly name them.
		[JsonProperty("price")]
		public string price { get; set; }

		[JsonProperty("stock")]
		public string stock { get; set; }

		// The constructor for this class needs a name, price, and a stock.
		// Validation is done prior to calling this
		public Item(string name, int price, int stock)
		{
			this.name = name;
			this.price = price.ToString();
			this.stock = stock.ToString();
		}

		// Trying to print out a custom object doesn't usually work the way you want it to
		override
		public string ToString()
		{
			// So instead I override it with my own format. Now, if I try to print an item
			// just by saying print(Item), it will print this out instead of what it would 
			// have printed otherwise.
			return "Name: " + name + "(" + stock + ")   Price: " + price;
		}
	}
}
