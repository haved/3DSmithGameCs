﻿using System;
using OpenTK;

namespace DSmithGameCs
{
	public static class Util
	{
		public static Vector2 X1Y1 = new Vector2(1,1);
		public static Vector3 X1Y1Z1 = new Vector3(1,1,1);
		public static Vector4 White = new Vector4(1,1,1,1);
		public const float PI = (float) Math.PI;
		readonly static Random r = new Random ();

		public static float NextFloat()
		{
			return (float)r.NextDouble ();
		}

		public static int NextInt(int max)
		{
			return r.Next (max);
		}

		public static float GetSmoothTransition(float transition)
		{
			return (float)Math.Pow(-Math.Cos (transition*PI)/2+0.5f, 1.4f);
		}
	}
}
