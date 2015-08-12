using System;

namespace DSmithGameCs
{
	public class MeshEntity : ColliderEntity
	{
		public readonly Mesh Mesh;
		public MeshEntity(Mesh mesh) : this(mesh, 0, 0, 0, 0, 0, 0, 0, 0){}

		public MeshEntity(Mesh m, float x, float y, float z) : this(m, x, y, z, 0, 0, 0, 0, 0){}

		public MeshEntity (Mesh m, float x, float y, float z, float xSize, float ySize) : this (m, x, y, z, 0, 0, 0, xSize, ySize) {}

		public MeshEntity (Mesh m, float x, float y, float z, float rotX, float rotY, float rotZ, float xSize, float ySize) : base (xSize, ySize)
		{
			Mesh = m;
			Pos.X = x;
			Pos.Y = y;
			Pos.Z = z;
			Rot.X = rotX;
			Rot.Y = rotY;
			Rot.Z = rotZ;
		}

		public override void Update(Scene s){}

		public override void Draw(Scene s)
		{
			Mesh.Draw ();
		}

		public override void DisposeEntity()
		{
			Mesh.Dispose ();
		}
	}
}

