namespace DSmithGameCs
{
	public static class MeshCollection
	{

		public static Mesh GreatswordCast{ get; private set;}
		public static Mesh GreatswordBlade{ get; private set;}
		public static Mesh IngotCast{ get; private set;}
		public static Mesh Ingot{ get; private set;}
		public static Mesh Diamond{ get; private set;}
		public static Mesh Hammer{ get; private set;}

		public static void Load()
		{
			GreatswordCast = new Mesh (Util.PATH+"res/mesh/GreatswordCast.ply");
			Ingot = new Mesh (Util.PATH+"res/mesh/ingot.ply");
			IngotCast = new Mesh (Util.PATH+"res/mesh/IngotCast.ply");
			GreatswordBlade = new Mesh (Util.PATH+"res/mesh/GreatswordBlade.ply");
			Diamond = new Mesh (Util.PATH+"res/mesh/Diamond.ply");
			Hammer = new Mesh (Util.PATH+"res/mesh/hammer.ply");
		}
	}
}

