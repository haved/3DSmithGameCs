using System;

namespace Smith2D
{
	public class Time
	{
		private static float deltaTime;

		public static void SetDelta(float delta)
		{
			deltaTime = delta;
		}

		public static float delta()
		{
			return deltaTime;
		}
	}
}

