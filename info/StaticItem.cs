using System;
using OpenTK;

namespace DSmithGameCs
{
	public class StaticItem : Item
	{
		const uint ItemsPerRow = 8;
		readonly StaticItemType type;

		public StaticItem (StaticItemType type)
		{
			this.type = type;
		}

		public override void RenderItem(float x, float y, float width, float height)
		{
			uint i = type.GetTexture ();
			OrthoRenderEngine.DrawTexturedBox (TextureCollection.Items, x, y, width, height, i%ItemsPerRow*1f/ItemsPerRow, i/ItemsPerRow*1f/ItemsPerRow, 1f/ItemsPerRow, 1f/ItemsPerRow);
		}

		public override uint GetSize()
		{
			return type.GetSize ();
		}

		public override string GetTooltipName()
		{
			return "!!!StaticItem!!!";
		}

		public override Vector4 GetTooltipColor()
		{
			return Vector4.Zero;
		}
	}
}

