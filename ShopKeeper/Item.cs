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
		[JsonProperty("name")]
		public string name { get; set; }

		[JsonProperty("price")]
		public string price { get; set; }

		[JsonProperty("stock")]
		public string stock { get; set; }

		public Item(string name, int price, int stock)
		{
			this.name = name;
			this.price = price.ToString();
			this.stock = stock.ToString();
		}

		override
		public string ToString()
		{
			return "Name: " + name + "(" + stock + ")   Price: " + price;
		}
	}
}
