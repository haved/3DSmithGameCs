using System;

namespace DSmithGameCs
{
	public class MeshEntity : Entity
	{
		Mesh mesh;
		protected bool solid;
		protected float xSize, ySize;
		public MeshEntity(Mesh mesh) : this(mesh, 0, 0, 0, 0, 0, 0){}

		public MeshEntity(Mesh m, float x, float y, float z) : this(m, x, y, z, 0, 0, 0){}

		public MeshEntity(Mesh m, float x, float y, float z, float rotX, float rotY, float rotZ, float xSize, float ySize) : this(m, x, y, z, rotX, rotY, rotZ)
		{
			this.xSize = xSize / 2;
			this.ySize = ySize / 2;
			this.solid = true;
		}

		public MeshEntity(Mesh m, float x, float y, float z, float rotX, float rotY, float rotZ)
		{
			mesh = m;
			Pos.X = x;
			Pos.Y = y;
			Pos.Z = z;
			Rot.X = rotX;
			Rot.Y = rotY;
			Rot.Z = rotZ;
			UpdateModelspaceMatrix ();
		}

		public override void Update(Scene s){}

		public override void Draw(Scene s)
		{
			mesh.Draw ();
		}

		public override bool IsSolid()
		{
			return solid;
		}

		public override float GetSolidX1()
		{
			return Pos.X - xSize;
		}

		public override float GetSolidY1()
		{
			return Pos.Y - ySize;
		}

		public override float GetSolidX2()
		{
			return Pos.X + xSize;
		}

		public override float GetSolidY2()
		{
			return Pos.Y + ySize;
		}
	}
}

