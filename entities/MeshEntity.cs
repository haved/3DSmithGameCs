using System;

namespace DSmithGameCs
{
	public class MeshEntity : Entity
	{
		public readonly Mesh Mesh;
		protected bool Solid;
		protected float XSize, YSize;
		public MeshEntity(Mesh mesh) : this(mesh, 0, 0, 0, 0, 0, 0){}

		public MeshEntity(Mesh m, float x, float y, float z) : this(m, x, y, z, 0, 0, 0){}

		public MeshEntity(Mesh m, float x, float y, float z, float rotX, float rotY, float rotZ, float xSize, float ySize) : this(m, x, y, z, rotX, rotY, rotZ)
		{
			this.XSize = xSize / 2;
			this.YSize = ySize / 2;
			this.Solid = true;
		}

		public MeshEntity(Mesh m, float x, float y, float z, float rotX, float rotY, float rotZ)
		{
			Mesh = m;
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
			Mesh.Draw ();
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
	}
}

