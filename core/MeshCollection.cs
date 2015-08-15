namespace DSmithGameCs
{
	public static class MeshCollection
	{

		public static Mesh GreatswordCast{ get; private set;}
		public static BladeMesh GreatswordBlade{ get; private set;}
		public static Mesh IngotCast{ get; private set;}
		public static Mesh Ingot{ get; private set;}
		public static Mesh Diamond{ get; private set;}

		public static void Load()
		{
			GreatswordCast = new Mesh (Util.PATH+"res/meshes/collection/GreatswordCast.plybin");
			Ingot = new Mesh (Util.PATH+"res/meshes/collection/ingot.plybin");
			IngotCast = new Mesh (Util.PATH+"res/meshes/collection/IngotCast.plybin");
			GreatswordBlade = new BladeMesh (Util.PATH+"res/meshes/collection/GreatswordBlade.ply", Util.PATH+"res/meshes/collection/GreatswordBladeSharp.ply");
			Diamond = new Mesh (Util.PATH+"res/meshes/collection/Diamond.plybin");
		}
	}
}

