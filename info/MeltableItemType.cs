using System;
using OpenTK;

namespace DSmithGameCs
{
	public class MeltableItemType : ItemType
	{
		private Vector4 color;
		public MeltableItemType (uint id, uint texture, Vector4 color) : base(id, texture)
		{
			this.color = color;
		}

		public virtual Mesh GetSolidMesh()
		{
			MeshCollection.Ingot;
		}

		public Vector4 GetColor()
		{
			return color;
		}
	}
}

