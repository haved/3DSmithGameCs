using System;
using System.Collections.Generic;

namespace DSmithGameCs
{
	public class HatchInventory
	{
		
		readonly List<Item> items;

		public HatchInventory ()
		{
			items = new List<Item> ();
		}

		public int GetItemAmount()
		{
			return items.Count;
		}

		public Item GetItem(int index)
		{
			return items [index];
		}

		public void AddItem(Item item)
		{
			items.Add (item);
		}

		public void RemoveItem(int index)
		{
			items.RemoveAt (index);
		}
	}
}

