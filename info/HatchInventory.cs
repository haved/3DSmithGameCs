using System;
using System.IO;
using System.Collections.Generic;

namespace DSmithGameCs
{
	public class HatchInventory : IDisposable
	{
		readonly List<Item> items;

		public HatchInventory ()
		{
			items = new List<Item> ();
		}

		~HatchInventory()
		{
			if (!disposed)
				Console.Out.WriteLine ("HatchInventory was not disposed before finalizer");
			Dispose ();
		}

		bool disposed;
		public void Dispose()
		{
			if (!disposed)
				foreach (Item i in items)
					i.Dispose ();

			disposed = true;
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

		public void SaveToFile(Stream writer)
		{
			writer.WriteByte ((byte)items.Count);

			foreach(Item i in items)
			{
				StreamIO.SaveItem (i, writer);
			}
		}

		public void LoadFromFile(Stream reader)
		{
			int itemCount = reader.ReadByte();
			items.Clear ();

			for(int i = 0; i < itemCount; i++)
			{
				items.Add(StreamIO.LoadItem (reader));
			}
		}
	}
}

