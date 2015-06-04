using System;
using System.IO;

namespace DSmithGameCs
{
	public static class ItemIO
	{
		private static Type[] types = new Type[3];
		static ItemIO()
		{
			types [0] = typeof(IngotItem);
			types [1] = typeof(CastItem);
		}

		public static Item LoadItem(StreamReader reader)
		{
			int id = int.Parse( reader.ReadLine().Substring (5));
			var output = (Item)Activator.CreateInstance(types[id]);
			output.LoadFromFile (reader);
			return output;
		}

		public static void SaveItem(Item item, StreamWriter writer)
		{
			writer.Write ("Item:");
			if(item is IngotItem)
				writer.WriteLine (0);
			else if(item is CastItem)
				writer.WriteLine (1);
			item.SaveToFile (writer);
		}
	}
}

