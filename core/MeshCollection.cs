using System;

namespace DSmithGameCs
{
	public static class MeshCollection
	{
		static Mesh[] meshes;

		public static void Load()
		{
			meshes = new Mesh[2];
			meshes [0] = new Mesh ("../../res/GreatswordCast.ply");
			meshes [1] = new Mesh ("../../res/ingot.ply");
		}

		public static void Dispose()
		{
			meshes = null;
		}

		public static Mesh GreatswordCast{get {return meshes[0];}}
		public static Mesh Ingot{get {return meshes[1];}}
	}
}

