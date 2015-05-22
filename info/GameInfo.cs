using OpenTK;

namespace DSmithGameCs
{
	public class GameInfo
	{
		public Inventory PlayerInventory;
		public HatchInventory HatchInv;
		public CastItem CurrentCast;

		public IngotItem[] FoundryContents = new IngotItem[FoundryMeshInfo.IngotAmount];

		public void NewGame()
		{
			PlayerInventory = new Inventory ();
			HatchInv = new HatchInventory ();
			PlayerInventory.AddItem (new IngotItem(IngotItemType.Iron));
			HatchInv.AddItem (new CastItem (MeshCollection.GreatswordCast, new Vector4 (205 / 255f, 165 / 255f, 68 / 255f, 1)));
			HatchInv.AddItem (new IngotItem(IngotItemType.Iron));
			HatchInv.AddItem (new IngotItem(IngotItemType.Iron));
			HatchInv.AddItem (new IngotItem(IngotItemType.Iron));
		}
	}
}

