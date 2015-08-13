using System;
using System.Drawing;
using OpenTK;

namespace DSmithGameCs
{
	public class BigBellowEntity : BellowEntity
	{
		public BigBellowEntity (Smith2DGame game, Mesh bellow, Matrix4 bellowTransform, Mesh rig, float x, float y, float z, float xSize, float ySize) : base(game, bellow, bellowTransform, rig, x, y, z, xSize, ySize) {}

		public override void ImproveOxygen(float f)
		{
			Game.GameStats.AirQuality += f;
		}

		public override float GetOxygenPrecent ()
		{
			return Game.GameStats.AirQuality;
		}
	}
}

