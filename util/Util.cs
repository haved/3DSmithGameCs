using System;
using System.Drawing;
using OpenTK;

namespace DSmithGameCs
{
	public static class Util
	{
		public const string PATH = "../../";
		public static Vector2 X1Y1 = new Vector2(1,1);
		public static Vector3 X1Y1Z1 = new Vector3(1,1,1);
		public static Vector4 White = new Vector4(1,1,1,1);
		public static Vector3 White3 = new Vector3 (1,1,1);
		public static Vector4 White60 = new Vector4(1,1,1,.6f);
		public static Vector4 LightRed60 = new Vector4(1,0.4f,0.4f,.6f);
		public static Vector4 DefaultEmission = new Vector4(0.7f, 0.1f, 0, 0.3f);
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

		public static Color GetColorFromVector(Vector4 v)
		{
			return Color.FromArgb ((int)(v.W * 255), (int)(v.X * 255), (int)(v.Y * 255) , (int)(v.Z * 255));
		}
	}
}

