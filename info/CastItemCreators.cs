using System;

namespace DSmithGameCs
{
	public interface ICastItemCreator
	{
		bool CanCreateItem(int metal, Smith2DGame game);

		Item CreateItem(int metal);
	}

	public class IngotItemCreator : ICastItemCreator
	{
		#region ICastItemCreator implementation
		public bool CanCreateItem (int metal, Smith2DGame game)
		{
			return true;
		}
		public Item CreateItem (int metal)
		{
			return new IngotItem (metal);
		}
		#endregion
	}
}

