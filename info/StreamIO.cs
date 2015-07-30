using System;
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
			types [2] = typeof(BladeItem);
		}

		public static Item LoadItem(Stream reader)
		{
			int id = reader.ReadByte ();
			var output = (Item)Activator.CreateInstance(types[id]);
			try{
				output.LoadInfoFromFile (reader);
			}
			catch(Exception e) {
				Console.Error.WriteLine (e);
				try
				{
					output.Dispose();
				}
				catch(Exception e2)
				{
					Console.Error.WriteLine (e2);
				}

				throw e;
			}
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
		public static Alloy LoadAlloy(Stream reader)
		{
			var output = new Alloy ();

			if(reader.ReadByte()==0) //The alloy is empty
				return output;

			var buffer = new byte[sizeof(float)];

			reader.Read (buffer, 0, buffer.Length);
			float Purity = BitConverter.ToSingle (buffer, 0);
			int metalCount = reader.ReadByte ();
			for (int i = 0; i < metalCount; i++) {
				int id = reader.ReadByte ();
				reader.Read (buffer, 0, buffer.Length);
				float amount = BitConverter.ToSingle (buffer, 0);
				output.AddMetal (id, Purity, amount);
			}
			return output;
		}

		public static void SaveAlloy(Alloy alloy, Stream writer)
		{
			if (alloy.Amount <= 0) {
				writer.WriteByte ((byte)0);
				return;
			}
			writer.WriteByte ((byte)1);
			writer.Write (BitConverter.GetBytes (alloy.Purity), 0, sizeof(float)); //Purity
			writer.WriteByte ((byte)alloy.MetalCount); //metalCount
			for (int i = 0; i < alloy.MetalCount; i++) {
				writer.WriteByte((byte) alloy.GetMetalID(i)); //metalID
				writer.Write(BitConverter.GetBytes (alloy.GetMetalAmount (i)), 0, sizeof(float)); //metalAmount
			}
		}
	}
}

