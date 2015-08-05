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
		float impurity;

		public Alloy(Alloy other)
		{
			metals = new List<MetalMass>(other.metals);
			totalAmount = other.totalAmount;
			impurity = other.impurity;
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

		public void AddMetal(int metal, float purity, float amount)
		{
			if (amount <= 0)
				return;

			KnownMetal m = KnownMetal.Metals [metal];

			color = totalAmount * color + amount * m.Color;
			totalAmount += amount;
			color /= totalAmount;

			impurity += (1-purity) * amount;

			if (m.MeltingPoint-15 > highestMeltingPoint)
				highestMeltingPoint = m.MeltingPoint-15;

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

		public float Amount {
			get { return totalAmount; }
		}

		public Alloy Normalized()
		{
			var output = new Alloy();
			foreach( MetalMass mm in metals)
				output.AddMetal (mm.Metal, Purity, mm.Amount / totalAmount);
			return output;
		}

		public void SetAmount(float amount)
		{
			if (amount < 0.005f) {
				totalAmount = 0;
				highestMeltingPoint = 0;
				impurity = 0;
				metals.Clear ();
				return;
			}

			foreach (MetalMass mm in metals)
				mm.Amount = mm.Amount / totalAmount * amount;

			impurity = impurity / totalAmount * amount;
			totalAmount = amount;
		}

		public int MetalCount{	get { return metals.Count; } }
		public KnownMetal this [int index]{ get { return KnownMetal.Metals [metals [index].Metal]; } }
		public int GetMetalID (int index) { return metals [index].Metal; }
		public float GetMetalAmount(int index) { return metals [index].Amount; }
		public float GetMetalFraction(int index) { return metals [index].Amount/totalAmount; }
		public float Purity { get { return 1-impurity/totalAmount; } }
	}
}

