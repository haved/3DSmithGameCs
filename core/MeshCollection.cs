namespace DSmithGameCs
{
	public static class MeshCollection
	{

		public static Mesh GreatswordCast{ get; private set;}
		public static Mesh GreatswordBlade{ get; private set;}
		public static Mesh IngotCast{ get; private set;}
		public static Mesh Ingot{ get; private set;}
		public static Mesh Diamond{ get; private set;}

		public static void Load()
		{
			GreatswordCast = new Mesh ("../../res/mesh/GreatswordCast.ply");
			Ingot = new Mesh ("../../res/mesh/ingot.ply");
			IngotCast = new Mesh ("../../res/mesh/IngotCast.ply");
			GreatswordBlade = new Mesh ("../../res/mesh/GreatswordBlade.ply");
			Diamond = new Mesh ("../../res/mesh/Diamond.ply");
		}
	}
}

