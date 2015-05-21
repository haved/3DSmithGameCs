using System;

namespace DSmithGameCs
{
	public class StaticItemType
	{
		//Well. There are no standard items implemented yet. Come back later!

		readonly uint texture;

		public StaticItemType(uint texture)
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

