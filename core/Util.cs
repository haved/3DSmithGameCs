using System;

namespace DSmithGameCs
{
	public static class Util
	{
		private readonly static Random r = new Random ();

		public static float NextFloat()
		{
			return (float)r.NextDouble ();
		}

		public static int NextInt(int max)
		{
			return r.Next (max);
		}
	}
}

