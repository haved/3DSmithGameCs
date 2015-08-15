﻿using System;
using System.IO;

namespace DSmithGameCs
{
	public class MainPlyBinary
	{
		public static void Main(string[] args)
		{
			try {
				if (args.Length == 0) {}
				else if (args [0].Equals ("-h") | args [0].Equals ("--help") | args [0].Equals ("-?")) {
					Console.Out.WriteLine (
						@"Help file for ply binary converter
plyToBinary <ply file> <target file> (loads the ply file and saves the mesh to the target file as a plybin)
binaryToPly <plybin file> <target file> (loads the plybin file and saves the mesh to the target file as a ply)
plyDirToBinary <ply directory> <target directory> (converts all the .ply files in ply directory into .plybin in the target directory)");
					return;
				} else if (args [0].Equals ("plyToBinary")) {
					if (args.Length != 3)
						Console.Out.WriteLine ("Wrong amount of arguments supplied");
					else {
						PlyToBinary(args[1], args[2]);

						return;
					}
				} else if (args [0].Equals ("binaryToPly")) {
					if (args.Length != 3)
						Console.Out.WriteLine ("Wrong amount of arguments supplied");
					else {
						string infile = args [1];
						string outfile = args [2];

						using(var stream = new FileStream(infile, FileMode.Open)){
							var loader = new MeshLoader(stream);
							loader.WriteTo(outfile);
						}
					
						Console.Out.WriteLine("wrote ply to '{0}'", outfile);

						return;
					}
				} else if (args [0].Equals ("plyDirToBinary")) {
					if (args.Length != 3)
						Console.Out.WriteLine ("Wrong amount of arguments supplied");
					else {
						PlyDirToBinaryDir(args[1], args[2], true);
					}
				}

				Console.Out.WriteLine ("-h for help");
			} catch (Exception e) {
				Console.Error.WriteLine (e);
				Console.Error.WriteLine ("-h for help");
			}
		}

		static void PlyToBinary(string infile, string outfile)
		{
			Console.Out.WriteLine ("infile (ply): {0}, outfile (plybin): {1}", infile, outfile);

			var loader = new MeshLoader(infile);
			using(var stream = new FileStream(outfile, FileMode.Create))
				loader.WriteTo(stream);

			Console.Out.WriteLine("wrote binary to '{0}'", outfile);
		}

		static void PlyDirToBinaryDir(string indir, string outdir, bool recursive)
		{
			if(!Directory.Exists(indir))
				Console.Out.WriteLine("The input directory doesn't exist");
			else {
				if (!Directory.Exists (outdir))
					Directory.CreateDirectory (outdir);

				if (recursive) {
					string[] dirs = Directory.GetDirectories (indir);
					foreach (string dir in dirs) {
						string name = dir.Substring (indir.Length, dir.Length-indir.Length);
						PlyDirToBinaryDir (indir + name, outdir + "/" + name, true);
					}
				}
				string[] files = Directory.GetFiles(indir);
				foreach (string file in files)
				{
					if(file.EndsWith(".ply", StringComparison.InvariantCulture))
					{
						string name = file.Substring (indir.Length, file.Length-indir.Length)+"bin";
						PlyToBinary (file, outdir+name);
					}
				}
				return;
			}
		}














		public static void LegacyMain(string[] args)
		{
			try {
				if (args.Length > 0) {
					if (args [0].Equals ("-h") | args [0].Equals ("-?") | args [0].Equals ("--help"))
						Console.Out.WriteLine (@"convert -plyin <file> -plybinout <file>
convert -plybinin <file> - plyout <file>
bladeconvert -plybladein <flatFile> <sharpFile> -plybinbladeout <file>
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

						if(loader != null) {
							for (i = 1; i < args.Length - 1; i++) {
								if (args [i].Equals ("-plyout")) {
									loader.WriteTo(args[i+1]);
									break;
								}
								if (args [i].Equals ("-plybinout")) {
									using(var stream = new FileStream(args[i+1], FileMode.CreateNew))
										loader.WriteTo(stream);
									break;
								}
							}
						}

						Console.Out.WriteLine("-h for help");

					} else if (args [0].Equals ("bladeconvert") & args.Length >= 5) {

					} else {
						Console.WriteLine ("-h for help");
					}
				} else
					Console.WriteLine ("-h for help");
			} catch(Exception e) {
				Console.Out.WriteLine (e);
				Console.WriteLine ("-h for help");
			}
		}

		static string GetFileName(string directory, string file, bool binary)
		{
			if (file.EndsWith ("bin", StringComparison.InvariantCulture))
				file.Substring (0, file.Length - 3);

			return directory + "/" + file + (binary ? "bin" : "");
		}
	}
}
