﻿using System;

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
}

