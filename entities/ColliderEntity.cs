using System;
using OpenTK;

namespace DSmithGameCs
{
	public abstract class ColliderEntity : Entity
	{
		protected bool Solid;
		protected float XSize, YSize;

		protected ColliderEntity(float x, float y, float z, float xSize, float ySize) : this(xSize, ySize)
		{
			Pos.X = x;
			Pos.Y = y;
			Pos.Z = z;
			UpdateModelspaceMatrix ();
		}

		protected ColliderEntity(float xSize, float ySize)
		{
			if (xSize > 0 & ySize > 0) {
				XSize = xSize / 2;
				YSize = ySize / 2;
				Solid = true;
			} else
				Solid = false;
		}

		public override bool IsSolid()
		{
			return Solid;
		}

		public override float GetSolidX1()
		{
			return Pos.X - XSize;
		}

		public override float GetSolidY1()
		{
			return Pos.Y - YSize;
		}

		public override float GetSolidX2()
		{
			return Pos.X + XSize;
		}

		public override float GetSolidY2()
		{
			return Pos.Y + YSize;
		}

		public bool IsInField(Vector2 point)
		{
			return ((point - Pos.Xy) * new Vector2 (1 / XSize, 1 / YSize)).Length < 1.4f;
		}
	}
}

