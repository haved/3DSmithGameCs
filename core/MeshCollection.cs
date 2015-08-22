namespace DSmithGameCs
{
	public static class MeshCollection
	{

		public static Mesh GreatswordCast{ get; private set;}
		public static BladeMesh GreatswordBlade{ get; private set;}
		public static BladeMesh BroadswordBlade{ get; private set;}
		public static Mesh IngotCast{ get; private set;}
		public static Mesh Ingot{ get; private set;}
		public static Mesh Diamond{ get; private set;}

		public static void Load()
		{
			GreatswordCast = new Mesh (Util.MESHPATH+"collection/GreatswordCast.plybin");
			Ingot = new Mesh (Util.MESHPATH+"collection/ingot.plybin");
			IngotCast = new Mesh (Util.MESHPATH+"collection/IngotCast.plybin");
			GreatswordBlade = new BladeMesh (Util.MESHPATH+"collection/GreatswordBlade.ply", Util.MESHPATH+"collection/GreatswordBladeSharp.ply");
			BroadswordBlade = new BladeMesh (Util.MESHPATH+"collection/BroadswordBlade.ply", Util.MESHPATH+"collection/BroadswordBladeSharp.ply");
			Diamond = new Mesh (Util.MESHPATH+"collection/Diamond.plybin");
		}
	}
}

