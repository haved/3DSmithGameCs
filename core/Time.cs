using System;

namespace DSmithGameCs
{
	public static class Time
	{
		private static float deltaTime;

		public static void SetDelta(float delta)
		{
			deltaTime = delta;
		}

		public static float Delta()
		{
			return deltaTime;
		}

		public static int CurrentTimeMillis()
		{
			return Environment.TickCount;
		}

		public static float CurrentTime()
		{
			return Environment.TickCount / 1000f;
		}
	}
}

