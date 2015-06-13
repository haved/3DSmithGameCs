using System;
using System.Collections.Generic;
using OpenTK;

namespace DSmithGameCs
{
	public class Alloy
	{
		class MetalMass
		{
			public int Metal;
			public float Amount;
			public MetalMass(int metal, float amount)
			{
				Metal = metal;
				Amount = amount;
			}
		}

		List<MetalMass> metals;
		float totalAmount;
		Vector4 color = Vector4.Zero;
		int highestMeltingPoint=0;

		public Alloy(Alloy other)
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

		public void AddMetal(int metal, float amount)
		{
			if (amount <= 0)
				return;

			BasicMetal m = BasicMetal.Metals [metal];

			color = totalAmount * color + amount * m.Color;
			totalAmount += amount;
			color /= totalAmount;

			if (m.MeltingPoint > highestMeltingPoint)
				highestMeltingPoint = m.MeltingPoint;

			for (int i = 0; i < metals.Count; i++) {
				if (metals [i].Metal == metal) {
					metals [i].Amount += amount;
					return;
				}
			}
			metals.Add (new MetalMass (metal, amount));
		}

		public int GetMeltingPoint ()
		{
			return highestMeltingPoint;
		}

		public Vector4 GetColor ()
		{
			return color;
		}

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
		public BasicMetal this[int index]{get { return BasicMetal.Metals[metals[index].Metal];}}
		public float GetMetalAmount(int index) { return metals[index].Amount;}

		public AlloyRecreator GetRecreator()
		{
			return new AlloyRecreator (this);
		}

		[Serializable]
		public struct AlloyRecreator
		{
			readonly int[] metals;
			readonly float[] amounts;

			public AlloyRecreator(Alloy alloy)
			{
				metals = new int[alloy.metals.Count];
				amounts = new float[alloy.metals.Count];

				for(int i = 0; i < alloy.metals.Count; i++)
				{
					metals[i] = alloy.metals[i].Metal;
					amounts[i] = alloy.metals[i].Amount;
				}
			}

			public Alloy GetAlloy ()
			{
				var output = new Alloy();
				for (int i = 0; i < metals.Length; i++) {
					output.AddMetal (metals[i],amounts[i]);
				}
				return output;
			}
		}
	}
}

