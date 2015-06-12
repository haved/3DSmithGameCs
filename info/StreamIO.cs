﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DSmithGameCs
{
	public static class StreamIO
	{
		private static Type[] types = new Type[3];
		static StreamIO()
		{
			types [0] = typeof(IngotItem);
			types [1] = typeof(CastItem);
		}

		public static Item LoadItem(Stream reader)
		{
			int id = reader.ReadByte ();
			var output = (Item)Activator.CreateInstance(types[id]);
			output.LoadInfoFromFile (reader);
			return output;
		}

		public static void SaveItem(Item item, Stream writer)
		{
			for (byte i = 0; i < types.Length; i++) {
				if (types [i] == null)
					break;
				if (item.GetType () == types [i])
					writer.WriteByte (i);
			}
			item.SaveInfoToFile (writer);
		}

		static readonly BinaryFormatter formater = new BinaryFormatter();
		public static IMetal LoadMetal(Stream reader)
		{
			return ((IMetalRecreator)formater.Deserialize (reader)).GetMetal ();
		}

		public static void SaveMetal(IMetal metal, Stream writer)
		{
			formater.Serialize (writer, metal.GetRecreator ());
		}
	}
}

