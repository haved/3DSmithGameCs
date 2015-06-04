using System.IO;

namespace DSmithGameCs
{
	public class GameInfo
	{
		public Inventory PlayerInventory;
		public HatchInventory HatchInv;
		public CastItem CurrentCast;

		public Alloy CastAlloy;
		public float CastFilling;
		public float CastingTemprature;
		public float OldFoundryAmount;

		public SolidList<IngotItem> FoundryIngots = new SolidList<IngotItem> (FoundryMeshInfo.IngotAmount);
		public Alloy FoundryAlloy = new Alloy();

		public float AirQuality = 25;
		public float CoalPercent = 80;
		public float FoundryTemprature = 25;

		public void NewGame()
		{
			PlayerInventory = new Inventory ();
			HatchInv = new HatchInventory ();
			PlayerInventory.AddItem (new IngotItem(BasicMetal.Iron));
			HatchInv.AddItem (new CastItem (CastItem.GreatsowordCast));
			HatchInv.AddItem (new CastItem (CastItem.IngotCast));
			HatchInv.AddItem (new IngotItem(BasicMetal.Gold));
			HatchInv.AddItem (new IngotItem(Alloy.Steel));
			HatchInv.AddItem (new IngotItem(BasicMetal.Iron));
			HatchInv.AddItem (new IngotItem(BasicMetal.Gold));
		}

		public void SaveGame(StreamWriter writer)
		{

		}

		public void LoadGame(StreamReader reader)
		{

		}
	}
}

