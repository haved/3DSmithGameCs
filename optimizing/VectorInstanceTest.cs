using System;
using OpenTK;

namespace DSmithGameCs
{
	public static class VectorInstanceTest
	{
		const uint iterations = 200000000;
		public static void Run()
		{
			//Test #1 ================================================================
			Vector3 result=new Vector3();
			float start = Time.CurrentTimeMillis ();

			for (int i = 0; i < iterations; i++)
				result += new Vector3 (1, Util.GetSmoothTransition(i/100f), 1f/i);

			Console.Out.WriteLine ("Time taken for test #1: " + (Time.CurrentTimeMillis()-start));
			Console.Out.WriteLine ("Result:" + result);

			//Test #2 ================================================================
			float result1 = 0;
			float result2 = 0;
			float result3 = 0;
			start = Time.CurrentTimeMillis ();

			for (int i = 0; i < iterations; i++) {
				result1 += 1;
				result2 += Util.GetSmoothTransition(i/100f);
				result3 += 1f / i;
			}

			Console.Out.WriteLine ("Time taken for test #2: " + (Time.CurrentTimeMillis()-start));
			Console.Out.WriteLine ("Result:" + result1 + ", " + result2 + ", " + result3);

			//Test #3 ================================================================
			result = new Vector3(0, 0, 0);
			start = Time.CurrentTimeMillis ();

			Vector3 adder = new Vector3 ();
			for (int i = 0; i < iterations; i++) {
				adder.X = 1;
				adder.Y = Util.GetSmoothTransition(i/100f);
				adder.Z = 1f / i;
				result += adder;
			}

			Console.Out.WriteLine ("Time taken for test #3: " + (Time.CurrentTimeMillis()-start));
			Console.Out.WriteLine ("Result:" + result);
		}

		static Vector3 v = new Vector3(0, 1, 3);
		public static void RunSet2()
		{
			//Test #1 ================================================================
			Vector3 result=new Vector3();
			float start = Time.CurrentTimeMillis ();

			for (int i = 0; i < iterations; i++) {
				result += v;
			}

			Console.Out.WriteLine ("Time taken for test #1: " + (Time.CurrentTimeMillis()-start));
			Console.Out.WriteLine ("Result:" + result);

			//Test #2 ================================================================

			result=new Vector3();
			start = Time.CurrentTimeMillis ();

			for (int i = 0; i < iterations; i++) {
				result += new Vector3(0, 1, 3);
			}

			Console.Out.WriteLine ("Time taken for test #2: " + (Time.CurrentTimeMillis()-start));
			Console.Out.WriteLine ("Result:" + result);
		}
	}
}

