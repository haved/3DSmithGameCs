using System;

namespace DSmithGameCs
{
	public class MainPlyBinary
	{
		public static void Main(string[] args)
		{
			if (args.Length > 0) {
				if (args [0].Equals ("-h") | args [0].Equals ("-?") | args [0].Equals ("--help"))
					Console.Out.WriteLine (@"convert -plyin <file> -plybinout <directory>
					convert -plybinin <file> - plyout <directory>
					bladeconvert -plybladein <flatFile> <sharpFile> -plybinbladeout <directory>
					bladeconvert -plybinbladein <file> - plyout <directory>");
				else if (args [0].Equals ("convert") & args.Length == 5) {

				} else if (args [0].Equals ("bladeconvert") & args.Length > 6) {

				} else {
					Console.WriteLine ("-h for help");
				}
			}
			else
				Console.WriteLine ("-h for help");
		}
	}
}

