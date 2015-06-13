using System;
using System.IO;

namespace DSmithGameCs
{
	public class GameInfo
	{
		public Inventory PlayerInventory;
		public HatchInventory HatchInv;
		public CastItem CurrentCast;

		public int CastMetal;
		public float CastFilling;
		public float CastingTemprature;
		public float OldFoundryAmount;

		public SolidList<IngotItem> FoundryIngots;
		public Alloy FoundryAlloy;

		public float AirQuality;
		public float CoalPercent;
		public float FoundryTemprature;

		public void NewGame()
		{
			PlayerInventory = new Inventory ();
			HatchInv = new HatchInventory ();
			PlayerInventory.AddItem (new IngotItem(BasicMetal.Iron));
			HatchInv.AddItem (new CastItem (CastItem.GreatsowordCast));
			HatchInv.AddItem (new CastItem (CastItem.IngotCast));
			HatchInv.AddItem (new IngotItem(BasicMetal.Gold));
			HatchInv.AddItem (new IngotItem(BasicMetal.Iron));
			HatchInv.AddItem (new IngotItem(BasicMetal.Gold));
			FoundryIngots = new SolidList<IngotItem> (FoundryMeshInfo.IngotAmount);
			FoundryAlloy = new Alloy ();
			AirQuality = 25;
			CoalPercent = 80;
			FoundryTemprature = 25;
			CastMetal = -1;
		}

		public void SaveGame(Stream writer)
		{
			Console.Out.WriteLine ("Game saved!");
			PlayerInventory.SaveToFile (writer); 									//PlayerInventory
			HatchInv.SaveToFile (writer);											//HatchInv
			writer.WriteByte ((byte)(CurrentCast != null ? 1 : 0)); 				//Is CurrentCast not null?
			if (CurrentCast != null)
				StreamIO.SaveItem (CurrentCast, writer); 							//CurrentCast if it's not null.
			writer.WriteByte((byte)CastMetal); 										//CastMetal
			writer.Write (BitConverter.GetBytes (CastFilling), 0, sizeof(float)); 	//CastFilling
			writer.Write (BitConverter.GetBytes (CastingTemprature), 0, sizeof(float)); //CastingTemerature
			writer.Write (BitConverter.GetBytes (OldFoundryAmount), 0, sizeof(float)); //OldFoundryAmount
			for (int i = 0; i < FoundryIngots.Capacity; i++) {
				writer.WriteByte((byte)(FoundryIngots[i]!=null?1 : 0)); 			//Is The Ingot at i not null?
				if (FoundryIngots [i] != null)
					StreamIO.SaveItem (FoundryIngots [i], writer);
			}
			StreamIO.SaveAlloy (FoundryAlloy, writer);								//FoundryAlloy
			writer.Write (BitConverter.GetBytes (AirQuality), 0, sizeof(float)); 	//AirQualtiy
			writer.Write (BitConverter.GetBytes (CoalPercent), 0, sizeof(float)); 	//CoalPercent
			writer.Write (BitConverter.GetBytes (FoundryTemprature), 0, sizeof(float)); //FoundryTemperature
			Console.Out.WriteLine (writer.Position);
		}

		public void LoadGame(Stream reader)
		{
			Console.WriteLine ("Loading game!");
			PlayerInventory = new Inventory ();
			PlayerInventory.LoadFromFile (reader); 									//PlayerInventory
			HatchInv = new HatchInventory ();
			HatchInv.LoadFromFile (reader); 										//HatchInv
			if(reader.ReadByte()!=0) 												//Check if CurrentCast is not null
				CurrentCast = (CastItem)StreamIO.LoadItem(reader); 					//CurrentCast	
			CastMetal = reader.ReadByte();											//CastMetal
			var buffer = new byte[sizeof(float)];
			reader.Read (buffer, 0, buffer.Length); 								//CastFilling
			CastFilling = BitConverter.ToSingle (buffer,0);
			reader.Read (buffer, 0, buffer.Length); 								//CastingTemperature
			CastingTemprature = BitConverter.ToSingle (buffer,0);
			reader.Read (buffer, 0, buffer.Length); 								//OldFoundryAmount
			OldFoundryAmount = BitConverter.ToSingle (buffer,0);
			FoundryIngots = new SolidList<IngotItem> (FoundryMeshInfo.IngotAmount);
			for (int i = 0; i < FoundryIngots.Capacity; i++) {
				if (reader.ReadByte () != 0) 										//Check if the item is not null
					FoundryIngots [i] = (IngotItem)StreamIO.LoadItem (reader);  	//Read IngotItems
			}
			FoundryAlloy = StreamIO.LoadAlloy (reader);								//FoundryAlloy
			reader.Read (buffer, 0, buffer.Length); 								//AirQuality
			AirQuality = BitConverter.ToSingle (buffer,0);
			reader.Read (buffer, 0, buffer.Length); 								//CoalPercent
			CoalPercent = BitConverter.ToSingle (buffer,0);
			reader.Read (buffer, 0, buffer.Length); 								//FoundryTemperature
			FoundryTemprature = BitConverter.ToSingle (buffer,0);
			Console.Out.WriteLine (reader.Position);
		}

		public void SaveGame()
		{
			using (var writer = new FileStream (GetSaveGamePath () + "save.sav", FileMode.Create, FileAccess.Write)) {
				SaveGame (writer);
				writer.Close ();
			}
		}

		public void LoadGame()
		{
			using (var reader = new FileStream (GetSaveGamePath () + "save.sav", FileMode.Open, FileAccess.Read)) {
				LoadGame (reader);
				reader.Close ();
			}
		}

		string saveGamePath;
		string GetSaveGamePath()
		{
			if (saveGamePath != null)
				return saveGamePath;
			try {
				string[] lines = File.ReadAllLines (Util.PATH + "savepath.txt");
				foreach (string s in lines)
					if (!s.StartsWith ("#", StringComparison.InvariantCulture)) {
						saveGamePath = s;
						break;
					}
			} catch (FileNotFoundException e) {
				Console.Error.WriteLine (e);
			}
			if (!Directory.Exists (saveGamePath)) {
				saveGamePath = Util.PATH + "saves/";
				Console.Error.WriteLine ("The path specified in savepath.txt is not a directory. Dafaulting to /saves");
			}
			if(!saveGamePath.EndsWith("/", StringComparison.InvariantCulture))
				saveGamePath+="/";
			if (!Directory.Exists (saveGamePath))
				Directory.CreateDirectory (saveGamePath);
			return saveGamePath;
		}
	}
}

