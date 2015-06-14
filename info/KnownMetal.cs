using System;
using OpenTK;

namespace DSmithGameCs
{
	public class KnownMetal
	{
		public static KnownMetal[] Metals = new KnownMetal[10];

		public static KnownMetal Copper = new KnownMetal(0, new Vector4(198/255f, 93/255f, 23/255f, 1f), 1084, 8.96f, "Copper", null);
		public static KnownMetal Gold = new KnownMetal(1, new Vector4(1, 215/255f, 0, 1f), 1063, 19.3f, "Gold", null);
		public static KnownMetal Iron = new KnownMetal(2, new Vector4(142/255f, 139/255f, 139/255f, 1f), 1536, 0, "Iron", null);
		public static KnownMetal Lead = new KnownMetal(3, new Vector4(0.3f, 0.3f, 0.3f, 1f), 327, 0, "Lead", null);
		public static KnownMetal Tin = new KnownMetal(4, new Vector4(193/255f, 193/255f, 193/255f, 1f), 231, 0, "Tin", null);
		public static KnownMetal Zinc = new KnownMetal(5, new Vector4(183/255f, 183/255f, 183/255f, 1f), 420, 0, "Zinc", null);

		public static KnownMetal Bronze = new KnownMetal(6, new Vector4(207/255f, 127/255f, 50/255f, 1f), 420, 0, "Bronze", new Part[]{new Part(Copper, 0.75f), new Part(Tin, 0.25f)});

		public readonly int Id;
		public readonly Vector4 Color;
		public readonly int MeltingPoint;
		public readonly float Density;
		readonly string name;

		struct Part
		{
			public readonly int Metal;
			public readonly float Amount;

			public Part(KnownMetal metal, float amount) : this(metal.Id, amount) {}

			public Part(int metal, float amount)
			{
				Metal = metal;
				Amount = amount;
			}
		}

		readonly Part[] parts;

		KnownMetal (int id, Vector4 color, int meltingPoint, float density, string name, Part[] parts)
		{
			Id = id;
			if (Metals [id] != null)
				throw new Exception ("The BasicMetal.Metals id " + id + " is already taken");
			Metals [id] = this;
			Color = color;
			MeltingPoint = meltingPoint;
			Density = density;
			this.name = name;
			this.parts = parts ?? new[] { new Part (id, 1) }; //If parts == null, than the new[] after ?? is used, otherwise, parts is used.
		}

		public string Name {get{ return Localization.GetLocalization ("ui.metal." + name); }}

		public float GetPurityFrom(Alloy alloy)
		{
			float purity = 0;

			foreach (Part p in parts)
				for (int i = 0; i < alloy.MetalCount; i++)
					if (p.Metal == alloy.GetMetalID (i))
						purity += Math.Min (alloy.GetMetalFraction (i), p.Amount) * alloy.Purity;

			return purity;
		}


		public float MinimumPurity
		{
			get{ return 0.6f; }
		}

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

