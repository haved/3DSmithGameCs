using System;
using OpenTK;

namespace DSmithGameCs
{
	public class BasicMetal : IMetal
	{
		public static BasicMetal[] Metals = new BasicMetal[255];

		public static BasicMetal Copper = new BasicMetal(0, new Vector4(198/255f, 93/255f, 23/255f, 1f), 1084, "Copper");
		public static BasicMetal Gold = new BasicMetal(1, new Vector4(1, 215/255f, 0, 1f), 1063, "Gold");
		public static BasicMetal Iron = new BasicMetal(2, new Vector4(142/255f, 139/255f, 139/255f, 1f), 1536, "Iron");
		public static BasicMetal Lead = new BasicMetal(3, new Vector4(0.3f, 0.3f, 0.3f, 1f), 327, "Lead");
		public static BasicMetal Tin = new BasicMetal(4, new Vector4(193/255f, 193/255f, 193/255f, 1f), 231, "Tin");
		public static BasicMetal Zinc = new BasicMetal(5, new Vector4(183/255f, 183/255f, 183/255f, 1f), 420, "Zinc");

		public static BasicMetal Carbon = new BasicMetal(6, new Vector4(139/255f, 69/255f, 19/255f, 1f), 0, "Carbon"); //Not really a metal

		readonly uint id;
		readonly Vector4 color;
		readonly int meltingPoint;
		readonly string name;
		public BasicMetal (uint id, Vector4 color, int meltingPoint, string name)
		{
			this.id = id;
			if (Metals [id] != null)
				throw new Exception ("The BasicMetal.Metals id " + id + " is already taken");
			Metals [id] = this;
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
			return Localization.GetLocalization("ui.metal."+name);
		}

		public IMetal Clone()
		{
			return this;
		}

		public IMetalRecreator GetRecreator()
		{
			return new BasicMetalRecreator (id);
		}

		#endregion

		[Serializable]
		public struct BasicMetalRecreator : IMetalRecreator
		{
			public uint Id;

			public BasicMetalRecreator(uint id)
			{
				Id = id;
			}

			#region IMetalRecreator implementation

			public IMetal GetMetal ()
			{
				return BasicMetal.Metals [Id];
			}

			#endregion
		}
	}
}

