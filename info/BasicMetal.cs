using System;
using OpenTK;

namespace DSmithGameCs
{
	public class BasicMetal : IMetal
	{
		public static BasicMetal Copper = new BasicMetal(new Vector4(198/255f, 93/255f, 23/255f, 1f), 1084, "Copper");
		public static BasicMetal Gold = new BasicMetal(new Vector4(1, 215/255f, 0, 1f), 1063, "Gold");
		public static BasicMetal Iron = new BasicMetal(new Vector4(142/255f, 139/255f, 139/255f, 1f), 1536, "Iron");
		public static BasicMetal Lead = new BasicMetal(new Vector4(0.3f, 0.3f, 0.3f, 1f), 327, "Lead");
		public static BasicMetal Tin = new BasicMetal(new Vector4(193/255f, 193/255f, 193/255f, 1f), 231, "Tin");
		public static BasicMetal Zinc = new BasicMetal(new Vector4(183/255f, 183/255f, 183/255f, 1f), 420, "Zinc");

		public static BasicMetal Carbon = new BasicMetal(new Vector4(139/255f, 69/255f, 19/255f, 1f), 0, "Carbon"); //Not really a metal

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

		public IMetal Clone()
		{
			return this;
		}

		#endregion
	}
}

