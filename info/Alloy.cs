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
		int highestMeltingPoint=0;

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
				totalAmount += amount;
				color /= totalAmount;

				if (m.GetMeltingPoint() > highestMeltingPoint)
					highestMeltingPoint = m.GetMeltingPoint ();

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
			return highestMeltingPoint;
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

		public Alloy Normalized()
		{
			var output = new Alloy();
			foreach( MetalMass mm in metals)
				output.AddMetal (mm.Metal, mm.Amount / totalAmount);
			return output;
		}

		public void SetAmount(float amount)
		{
			if (amount < 0.005f) {
				totalAmount = 0;
				highestMeltingPoint = 0;
				metals.Clear ();
				return;
			}

			foreach (MetalMass mm in metals)
				mm.Amount = mm.Amount / totalAmount * amount;

			totalAmount = amount;
		}

		public int MetalTypeAmount{	get { return metals.Count; }}
		public IMetal this[int index]{get { return metals[index].Metal;}}
		public float GetMetalAmount(int index) { return metals[index].Amount;}
	}
}

