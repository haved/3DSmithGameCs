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

		List<MetalMass> metals;
		float totalAmount;
		Vector4 color = Vector4.Zero;
		int highestMeltingPoint=0;

		Alloy(Alloy other)
		{
			metals = new List<MetalMass>(other.metals);
			totalAmount = other.totalAmount;
			color = other.color;
			highestMeltingPoint = other.highestMeltingPoint;
		}

		public Alloy()
		{
			metals = new List<MetalMass>();
			totalAmount = 0;
			color = Vector4.Zero;
			highestMeltingPoint = 0;
		}

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
				contents += (int)(m.Amount/totalAmount * 100+0.4f)  + "% " + m.Metal.GetName ();
			}
			return contents;
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

		public IMetal Clone()
		{
			return metals.Count == 1 ? metals [0].Metal : new Alloy (this);
		}

		public IMetalRecreator GetRecreator()
		{
			return new AlloyRecreator (this);
		}

		[Serializable]
		public struct AlloyRecreator : IMetalRecreator
		{
			readonly IMetalRecreator[] metals;
			readonly float[] amounts;

			public AlloyRecreator(Alloy alloy)
			{
				metals = new IMetalRecreator[alloy.metals.Count];
				amounts = new float[alloy.metals.Count];

				for(int i = 0; i < alloy.metals.Count; i++)
				{
					metals[i] = alloy.metals[i].Metal.GetRecreator();
					amounts[i] = alloy.metals[i].Amount;
				}
			}

			#region IMetalRecreator implementation

			public IMetal GetMetal ()
			{
				var output = new Alloy();
				for (int i = 0; i < metals.Length; i++) {
					output.AddMetal (metals[i].GetMetal(),amounts[i]);
				}
				return output;
			}

			#endregion
		}
	}
}

