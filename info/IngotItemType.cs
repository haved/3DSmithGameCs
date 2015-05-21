using System;
using OpenTK;

namespace DSmithGameCs
{
	public class IngotItemType
	{
		public static IngotItemType Iron = new IngotItemType(new Vector4(0.7f, 0.7f, 0.7f, 1f));

		Vector4 color;
		public IngotItemType (Vector4 color)
		{
			this.color = color;
		}

		public virtual Mesh GetSolidMesh()
		{
			return MeshCollection.Ingot;
		}

		public Vector4 GetColor()
		{
			return color;
		}
	}
}

