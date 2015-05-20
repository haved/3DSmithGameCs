using System;
using OpenTK;

namespace DSmithGameCs
{
	public class MeltableItem : StaticItem
	{
		private MeltableItemType type;

		public MeltableItem (MeltableItemType type) : base(type)
		{
			this.type = type;
		}

		public void RenderMesh(float x, float y, float z, float width, float height)
		{
			BasicShader.GetInstance ().SetColor (type.GetColor ());
			type.GetSolidMesh ();
		}
	}
}

