using System;

namespace DSmithGameCs
{
	public class Item
	{
		public Item ()
		{
		}

		public virtual uint GetSize()
		{
			return 1;
		}

		public virtual void Render(float x, float y, float width, float height)
		{
			
		}
	}
}

