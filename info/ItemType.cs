using System;

namespace DSmithGameCs
{
	public class ItemType
	{
		private readonly ItemType[] types = new ItemType[256];
		public ItemType Iron = new ItemType(0, 0);

		private readonly uint texture;

		public ItemType(uint id)
		{
			types [id] = this;

		}

		public ItemType(uint id, uint texture) : this(id)
		{
			this.texture = texture;
		}

		public uint GetTexture()
		{
			return texture;
		}

		public uint GetSize()
		{
			return 1;
		}
	}
}

