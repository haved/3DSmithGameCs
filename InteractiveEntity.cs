using System;

namespace Smith2D
{
	public class InteractiveEntity : MeshEntity
	{
		private float xSize;
		private float ySize;
		public InteractiveEntity (Mesh m, float x, float y, float z, float xSize, float ySize) : base(m, x, y, z)
		{
			this.xSize = xSize / 2f;
			this.ySize = ySize / 2f;
		}

		public override bool IsSolid()
		{
			return true;
		}

		public override float GetSolidX1()
		{
			return pos.X - xSize;
		}

		public override float GetSolidY1()
		{
			return pos.Y - ySize;
		}

		public override float GetSolidX2()
		{
			return pos.X + xSize;
		}

		public override float GetSolidY2()
		{
			return pos.Y + ySize;
		}
	}
}

