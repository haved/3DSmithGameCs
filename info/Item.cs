using System;

namespace DSmithGameCs
{
	public abstract class Item
	{
		public Item ()
		{
		}

		public abstract uint GetSize ();

		public abstract void RenderItem (float x, float y, float width, float height);
	}
}

