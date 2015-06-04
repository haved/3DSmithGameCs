using System;

namespace DSmithGameCs
{
	public interface ICastItemCreator
	{
		bool CanCreateItem(IMetal metal, Smith2DGame game);

		Item CreateItem(IMetal metal);
	}

	public class IngotItemCreator : ICastItemCreator
	{
		#region ICastItemCreator implementation
		public bool CanCreateItem (IMetal metal, Smith2DGame game)
		{
			return true;
		}
		public Item CreateItem (IMetal metal)
		{
			return new IngotItem (metal.Clone());
		}
		#endregion
	}
}

