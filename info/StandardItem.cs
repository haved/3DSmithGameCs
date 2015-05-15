using System;

namespace DSmithGameCs
{
	public class StaticItem : Item
	{
		readonly ItemType type;

		public StaticItem (ItemType type)
		{
			this.type = type;
		}

		public override void Render()
		{

		}

		public override uint GetSize()
		{
			return type.GetSize ();
		}
	}
}

