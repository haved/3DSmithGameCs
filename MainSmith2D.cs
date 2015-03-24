using System;

namespace Smith2D
{
	class MainSmith2D
	{
		public static void Main (string[] args)
		{
			using (SmithGameWindow window = new SmithGameWindow ())
				window.Run ();
		}
	}
}
