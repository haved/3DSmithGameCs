using System;
using OpenTK;

namespace DSmithGameCs
{
	public class BasicMetal : IMetal
	{
		public static BasicMetal Brass = new BasicMetal(new Vector4(181f/255, 166f/255, 66f/255, 1f), 530, "Brass"); //930'C
		public static BasicMetal Copper = new BasicMetal(new Vector4(0.7f, 0.7f, 0.7f, 1f), 1084, "Copper");
		public static BasicMetal Gold = new BasicMetal(new Vector4(0.7f, 0.7f, 0.7f, 1f), 1063, "Gold");
		public static BasicMetal Iron = new BasicMetal(new Vector4(0.7f, 0.7f, 0.7f, 1f), 1536, "Iron");
		public static BasicMetal Lead = new BasicMetal(new Vector4(0.7f, 0.7f, 0.7f, 1f), 327, "Lead");
		public static BasicMetal Steal = new BasicMetal(new Vector4(0.7f, 0.7f, 0.7f, 1f), 1510, "Steal");

		readonly Vector4 color;
		readonly int meltingPoint;
		readonly string name;
		public BasicMetal (Vector4 color, int meltingPoint, string name)
		{
			this.color = color;
			this.meltingPoint = meltingPoint;
			this.name = name;
		}

		#region IMetal implementation

		public int GetMeltingPoint ()
		{
			return meltingPoint;
		}

		public Vector4 GetColor ()
		{
			return color;
		}

		public string GetName ()
		{
			return name;
		}

		#endregion
	}
}

