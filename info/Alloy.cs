using System;
using System.Collections.Generic;
using OpenTK;

namespace DSmithGameCs
{
	public class Alloy : IMetal
	{
		public static Alloy Steel = new Alloy();

		static Alloy()
		{
			Steel.AddMetal (BasicMetal.Iron, 0.90f);
			Steel.AddMetal (BasicMetal.Carbon, 0.1f);
		}

		class MetalMass
		{
			public IMetal Metal;
			public float Amount;
			public MetalMass(IMetal metal, float amount)
			{
				Metal = metal;
				Amount = amount;
			}
		}

		List<MetalMass> metals = new List<MetalMass>();
		float totalAmount;
		Vector4 color = Vector4.Zero;
		int meltingPoint;

		public void AddMetal(IMetal m, float amount)
		{
			if (amount <= 0)
				return;

			Alloy alloy = m as Alloy;
			if (alloy != null) {
				foreach (MetalMass mm in alloy.metals)
					AddMetal (mm.Metal, mm.Amount/alloy.totalAmount*amount);
			} else {
				color = totalAmount * color + amount * m.GetColor ();
				meltingPoint = (int)(totalAmount * meltingPoint + amount * m.GetMeltingPoint ());
				totalAmount += amount;
				color /= totalAmount;
				meltingPoint = (int)(meltingPoint / totalAmount);
				for (int i = 0; i < metals.Count; i++) {
					if (metals [i].Metal == m) {
						metals [i].Amount += amount;
						return;
					}
				}
				metals.Add (new MetalMass (m, amount));
			}
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
			string contents = "";
			foreach (MetalMass m in metals) {
				if (!contents.Equals (""))
					contents += " + ";
				contents += (m.Amount/totalAmount * 100)  + "% " + m.Metal.GetName ();
			}
			return "Alloy: " + contents;
		}

		#endregion


		public float GetAmount()
		{
			return totalAmount;
		}
	}
}

