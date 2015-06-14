using System;
using System.Collections.Generic;
using OpenTK;

namespace DSmithGameCs
{
	public class KnownMetal
	{
		public static KnownMetal[] Metals = new KnownMetal[255];

		public static KnownMetal Copper = new KnownMetal(0, new Vector4(198/255f, 93/255f, 23/255f, 1f), 1084, 8.96f, "Copper");
		public static KnownMetal Gold = new KnownMetal(1, new Vector4(1, 215/255f, 0, 1f), 1063, 19.3f, "Gold");
		public static KnownMetal Iron = new KnownMetal(2, new Vector4(142/255f, 139/255f, 139/255f, 1f), 1536, 0, "Iron");
		public static KnownMetal Lead = new KnownMetal(3, new Vector4(0.3f, 0.3f, 0.3f, 1f), 327, 0, "Lead");
		public static KnownMetal Tin = new KnownMetal(4, new Vector4(193/255f, 193/255f, 193/255f, 1f), 231, 0, "Tin");
		public static KnownMetal Zinc = new KnownMetal(5, new Vector4(183/255f, 183/255f, 183/255f, 1f), 420, 0, "Zinc");

		public readonly int Id;
		public readonly Vector4 Color;
		public readonly int MeltingPoint;
		public readonly float Density;
		readonly string name;

		delegate float GetPurity(Alloy a);

		public KnownMetal (int id, Vector4 color, int meltingPoint, float density, string name)
		{
			Id = id;
			if (Metals [id] != null)
				throw new Exception ("The BasicMetal.Metals id " + id + " is already taken");
			Metals [id] = this;
			Color = color;
			MeltingPoint = meltingPoint;
			Density = density;
			this.name = name;
		}

		public string Name {get{ return Localization.GetLocalization ("ui.metal." + name); }}

		public static int GetMeltingPoint(int id)
		{
			return Metals [id].MeltingPoint;
		}

		public static Vector4 GetColor(int id)
		{
			return Metals [id].Color;
		}

		public static float GetDensity(int id)
		{
			return Metals [id].Density;
		}

		public static string GetName(int id)
		{
			return Metals [id].Name;
		}
	}
}

