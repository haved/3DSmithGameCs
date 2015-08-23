namespace DSmithGameCs
{
	public static class MeshCollection
	{

		public static Mesh GreatswordCast{ get; private set;}
		public static BladeMesh GreatswordBlade{ get; private set;}
		public static Mesh BroadswordCast{ get; private set;}
		public static BladeMesh BroadswordBlade{ get; private set;}
		public static Mesh IngotCast{ get; private set;}
		public static Mesh Ingot{ get; private set;}
		public static Mesh Diamond{ get; private set;}

		public static void Load()
		{
			GreatswordCast = new Mesh (Util.MESHPATH+"collection/GreatswordCast.plybin");
			BroadswordCast = new Mesh (Util.MESHPATH+"collection/BroadswordCast.plybin");
			Ingot = new Mesh (Util.MESHPATH+"collection/ingot.plybin");
			IngotCast = new Mesh (Util.MESHPATH+"collection/IngotCast.plybin");
			GreatswordBlade = new BladeMesh (Util.MESHPATH+"blades/GreatswordBlade.bladebin");
			BroadswordBlade = new BladeMesh (Util.MESHPATH+"blades/BroadswordBlade.bladebin");
			Diamond = new Mesh (Util.MESHPATH+"collection/Diamond.plybin");
		}
	}
}

