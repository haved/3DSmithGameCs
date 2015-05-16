using System;

namespace DSmithGameCs
{
	public static class MeshCollection
	{
		static Mesh[] meshes;

		public static void Load()
		{
			meshes = new Mesh[1];
			meshes [0] = new Mesh ("../../res/GreatswordCast.ply");
		}

		public static void Dispose()
		{
			meshes = null;
		}

		public static Mesh GreatswordCast{get {return meshes[0];}}
	}
}

