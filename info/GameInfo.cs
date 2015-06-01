using OpenTK;
using System.Collections.Generic;

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
			HatchInv.AddItem (new CastItem (MeshCollection.GreatswordCast, new Vector4 (205 / 255f, 165 / 255f, 68 / 255f, 1), "Greatsword", 2, 0.38f));
			HatchInv.AddItem (new CastItem (MeshCollection.IngotCast, new Vector4 (205 / 255f, 165 / 255f, 68 / 255f, 1), "Ingot", 1, 0.63f));
			HatchInv.AddItem (new IngotItem(BasicMetal.Gold));
			HatchInv.AddItem (new IngotItem(Alloy.Steel));
			HatchInv.AddItem (new IngotItem(BasicMetal.Iron));
			HatchInv.AddItem (new IngotItem(BasicMetal.Gold));
		}
	}
}

