using System;

namespace DSmithGameCs
{
	public interface ICastItemCreator
	{
		bool CanCreateItem(int metal, float purity, Smith2DGame game);

		Item CreateItem(int metal, float purity);
	}

	public class IngotItemCreator : ICastItemCreator
	{
		#region ICastItemCreator implementation
		public bool CanCreateItem (int metal, float purity, Smith2DGame game)
		{
			return true;
		}
		public Item CreateItem (int metal, float purity)
		{
			return new IngotItem (metal, purity);
		}
		#endregion
	}

	public class BladeCreator : ICastItemCreator
	{
		readonly BladeItem.BladeType type;

		public BladeCreator(BladeItem.BladeType type)
		{
			this.type = type;
		}

		#region ICastItemCreator implementation
		public bool CanCreateItem (int metal, float purity, Smith2DGame game)
		{
			return true;
		}
		public Item CreateItem (int metal, float purity)
		{
			return new BladeItem (type, metal, purity);
		}
		#endregion
	}
}

