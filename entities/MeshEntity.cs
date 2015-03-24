using System;

namespace Smith2D
{
	public class MeshEntity : Entity
	{
		Mesh mesh;
		public MeshEntity(Mesh mesh) : this(mesh, 0, 0, 0){}

		public MeshEntity(Mesh m, float x, float y, float z)
		{
			this.mesh = m;
			pos.X = x;
			pos.Y = y;
			pos.Z = z;
			UpdateModelspaceMatrix ();
		}

		public override void Update(Scene scene){}

		public override void Draw(Scene scene)
		{
			mesh.Draw ();
		}
	}
}

