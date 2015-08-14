using System;
using System.IO;

namespace DSmithGameCs
{
	public class MainPlyBinary
	{
		public static void Main(string[] args)
		{
			try {
				if (args.Length > 0) {
					if (args [0].Equals ("-h") | args [0].Equals ("-?") | args [0].Equals ("--help"))
						Console.Out.WriteLine (@"convert -plyin <file> -plybinout <directory>
convert -plybinin <file> - plyout <directory>
bladeconvert -plybladein <flatFile> <sharpFile> -plybinbladeout <directory>
bladeconvert -plybinbladein <file> - plyout <directory>");
					else if (args [0].Equals ("convert") & args.Length == 5) {
						MeshLoader loader = null;
						int i;
						for (i = 1; i < args.Length - 1; i++) {
							if (args [i].Equals ("-plyin")) {
								loader = new MeshLoader (args [i + 1]);
								break;
							}
							if (args [i].Equals ("-plybinin")) {
								using(var stream = new FileStream(args [i + 1], FileMode.Open))
									loader = new MeshLoader (stream);
								break;
							}
						}

						if (loader == null) {
							Console.Out.WriteLine ("ERROR: must supply an input file, either -plyin <file> or -plybinin <file>");
							return;
						}

						string filename = args[i+1];
						filename = filename.Substring(Math.Max(filename.LastIndexOf("/", StringComparison.InvariantCulture), filename.LastIndexOf(@"\", StringComparison.InvariantCulture))+1, filename.Length);
						filename = filename.Substring(0, filename.IndexOf(".", StringComparison.InvariantCulture));

						for (i = 1; i < args.Length - 1; i++) {
							if (args [i].Equals ("-plyout")) {
								loader.WriteTo(args[i+1]+filename+".ply");
								break;
							}
							if (args [i].Equals ("-plybinout")) {
								using(var stream = new FileStream(args[i+1]+filename+".plybin", FileMode.CreateNew))
									loader.WriteTo(stream);
								break;
							}
						}

					} else if (args [0].Equals ("bladeconvert") & args.Length >= 5) {

					} else {
						Console.WriteLine ("-h for help");
					}
				} else
					Console.WriteLine ("-h for help");
			} catch {
				Console.WriteLine ("-h for help");
			}
		}
	}
}

