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
plyDirToBinary <ply directory> <target directory> (recursivly converts all .ply in the ply directory to .plybin in the target directory)
binaryDirToPly <plybin directory> <target directory> (recursivly converts all .plybin the in plybin directory to .ply in the target directory)
plysToBladebin <flat blade ply mesh> <sharp blade ply mesh> <target> (turns two ply files with the same amount of vertices/faces into one bladebin file)
bladebinToPlys <bladebin file> <flat ply target> <sharp ply target> (turns a bladebin file into two ply's, one for the flat and one for the sharp mesh)");
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
						BinaryToPly(args[1], args[2]);

						return;
					}
				} else if (args [0].Equals ("plyDirToBinary")) {
					if (args.Length != 3)
						Console.Out.WriteLine ("Wrong amount of arguments supplied");
					else {
						PlyDirToBinaryDir(args[1], args[2], true);

						return;
					}
				} else if (args [0].Equals ("binaryDirToPly")) {
					if (args.Length != 3)
						Console.Out.WriteLine ("Wrong amount of arguments supplied");
					else {
						BinaryDirToPlyDir(args[1], args[2], true);

						return;
					}
				} else if (args [0].Equals ("plysToBladebin")) {
					if (args.Length != 4)
						Console.Out.WriteLine ("Wrong amount of arguments supplied");
					else {
						PlysToBladebin(args[1], args[2], args[3]);

						return;
					}
				} else if (args [0].Equals ("bladebinToPlys")) {
					if (args.Length != 3)
						Console.Out.WriteLine ("Wrong amount of arguments supplied");
					else {
						BladebinToPlys(args[1], args[2], args[3]);

						return;
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
						string name = dir.Substring (indir.Length, dir.Length-indir.Length); //To get the name from the full path.
						PlyDirToBinaryDir (indir + name, outdir + "/" + name, true);
					}
				}
				string[] files = Directory.GetFiles(indir);
				foreach (string file in files)
				{
					if(file.EndsWith(".ply", StringComparison.InvariantCulture))
					{
						string name = file.Substring (indir.Length, file.Length-indir.Length)+"bin"; //To get the name with a .plybin extension from the full path.
						PlyToBinary (file, outdir+name);
					}
				}
				return;
			}
		}

		static void BinaryToPly(string infile, string outfile)
		{
			Console.Out.WriteLine ("infile (plybin): {0}, outfile (ply): {1}", infile, outfile);

			using (var stream = new FileStream (infile, FileMode.Open)) {
				var loader = new MeshLoader (stream);
				loader.WriteTo (outfile);
			}

		}

		static void BinaryDirToPlyDir(string indir, string outdir, bool recursive)
		{
			if(!Directory.Exists(indir))
				Console.Out.WriteLine("The input directory doesn't exist");
			else {
				if (!Directory.Exists (outdir))
					Directory.CreateDirectory (outdir);

				if (recursive) {
					string[] dirs = Directory.GetDirectories (indir);
					foreach (string dir in dirs) {
						string name = dir.Substring (indir.Length, dir.Length-indir.Length); //To get the name from the full path.
						BinaryDirToPlyDir (indir + name, outdir + "/" + name, true);
					}
				}
				string[] files = Directory.GetFiles(indir);
				foreach (string file in files)
				{
					if(file.EndsWith(".plybin", StringComparison.InvariantCulture))
					{
						string name = file.Substring (indir.Length, file.Length-indir.Length); //To get the name from the full path.
						name.Substring (0, name.Length - 3); //To remove the bin from .plybin
						BinaryToPly (file, outdir+name);
					}
				}
				return;
			}
		}

		static void PlysToBladebin(string flatfile, string sharpfile, string outfile)
		{
			Console.Out.WriteLine ("flatfile (ply): {0}, sharpfile (ply): {1}, outfile (bladebin): {2}", flatfile, sharpfile, outfile);

			using (Stream stream = new FileStream(outfile, FileMode.Create)) {
				var loader = new BladeMeshLoader (flatfile, sharpfile);
				loader.WriteTo (stream);
			}

		}

		static void BladebinToPlys(string bladebin, string outflatfile, string outsharpfile)
		{
			Console.Out.WriteLine ("inputfile (bladebi): {0}, outflatfile (ply): {1}, outsharpfile (ply): {2}", bladebin, outflatfile, outsharpfile);

			using (Stream stream = new FileStream(bladebin, FileMode.Open)) {
				var loader = new BladeMeshLoader (stream);
				loader.WriteTo (outflatfile, outsharpfile);
			}

		}
	}
}
