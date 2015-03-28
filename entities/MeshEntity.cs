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
			pos.X = x;
			pos.Y = y;
			pos.Z = z;
			rot.X = rotX;
			rot.Y = rotY;
			rot.Z = rotZ;
			UpdateModelspaceMatrix ();
		}

		public override void Update(Scene scene){}

		public override void Draw(Scene scene)
		{
			mesh.Draw ();
		}

		public override bool IsSolid()
		{
			return solid;
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

