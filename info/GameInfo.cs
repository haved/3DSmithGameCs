using OpenTK;
using System.Collections.Generic;

namespace DSmithGameCs
{
	public class GameInfo
	{
		public Inventory PlayerInventory;
		public HatchInventory HatchInv;
		public CastItem CurrentCast;

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
			HatchInv.AddItem (new CastItem (MeshCollection.GreatswordCast, new Vector4 (205 / 255f, 165 / 255f, 68 / 255f, 1)));
			HatchInv.AddItem (new CastItem (MeshCollection.IngotCast, new Vector4 (205 / 255f, 165 / 255f, 68 / 255f, 1)));
			HatchInv.AddItem (new IngotItem(BasicMetal.Brass));
			HatchInv.AddItem (new IngotItem(BasicMetal.Brass));
			HatchInv.AddItem (new IngotItem(BasicMetal.Iron));
		}
	}
}

