using System;
using OpenTK;

namespace DSmithGameCs
{
	public class CoalBellowEntity : BellowEntity
	{
		public CoalBellowEntity (Smith2DGame game, Mesh bellow, Matrix4 bellowTransform, Mesh rig, float x, float y, float z, float xSize, float ySize) : base(game, bellow, bellowTransform, rig, x, y, z, xSize, ySize) {}
	
		public override void ImproveOxygen(float f)
		{
			Console.Out.WriteLine ("Oxygen improved for the coal bellow");
		}

		public override float GetOxygenPrecent ()
		{
			return 50;
		}
	}
}

