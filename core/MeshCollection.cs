using System;

namespace DSmithGameCs
{
	public static class MeshCollection
	{
		static Mesh[] meshes;

		public static void Load()
		{
			meshes = new Mesh[4];
			meshes [0] = new Mesh ("../../res/mesh/GreatswordCast.ply");
			meshes [1] = new Mesh ("../../res/mesh/ingot.ply");
			meshes [2] = new Mesh ("../../res/mesh/IngotCast.ply");
			meshes [3] = new Mesh ("../../res/mesh/GreatswordBlade.ply");
		}

		public static void Dispose()
		{
			meshes = null;
		}

		public static Mesh GreatswordCast{get {return meshes[0];}}
		public static Mesh GreatswordBlade{get {return meshes[3];}}
		public static Mesh IngotCast{get {return meshes[2];}}
		public static Mesh Ingot{get {return meshes[1];}}
	}
}

