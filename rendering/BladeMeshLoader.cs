using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

namespace DSmithGameCs
{
	public class BladeMeshLoader
	{
		public BladeVertex[] Vertices;
		public uint[] Indices;

		public BladeMeshLoader (string flatSword, string sharpSword)
		{
			try
			{
				string[] flatLines = File.ReadAllLines(flatSword);

				uint flatVertexAmount = 0;
				uint flatFaceAmount = 0;
				bool flatNormals=false;
				bool flatColors=false;

				uint flatProgress = 0;
				for (; flatProgress < flatLines.Length; flatProgress++)
				{
					if (flatLines [flatProgress].StartsWith ("element vertex ", StringComparison.Ordinal))
						flatVertexAmount = UInt32.Parse (flatLines [flatProgress].Substring (15));
					if (flatLines [flatProgress].StartsWith ("element face ", StringComparison.Ordinal))
						flatFaceAmount = UInt32.Parse (flatLines [flatProgress].Substring (13));
					flatNormals |= flatLines [flatProgress].StartsWith ("property float nx", StringComparison.Ordinal);
					flatColors |= flatLines [flatProgress].StartsWith ("property uchar red", StringComparison.Ordinal); 
					if (flatLines [flatProgress].StartsWith ("end_header", StringComparison.Ordinal))
						break;
				}
				flatProgress++;


				string[] sharpLines = File.ReadAllLines(sharpSword);

				uint sharpVertexAmount = 0;
				uint sharpFaceAmount = 0;
				bool sharpNormals=false;

				uint sharpProgress = 0;
				for (; sharpProgress < sharpLines.Length; sharpProgress++)
				{
					if (sharpLines [sharpProgress].StartsWith ("element vertex ", StringComparison.Ordinal))
						sharpVertexAmount = UInt32.Parse (sharpLines [sharpProgress].Substring (15));
					if (sharpLines [sharpProgress].StartsWith ("element face ", StringComparison.Ordinal))
						sharpFaceAmount = UInt32.Parse (sharpLines [sharpProgress].Substring (13));
					sharpNormals |= sharpLines [sharpProgress].StartsWith ("property float nx", StringComparison.Ordinal);
					if (sharpLines [sharpProgress].StartsWith ("end_header", StringComparison.Ordinal))
						break;
				}
				sharpProgress++;

				if(flatVertexAmount != sharpVertexAmount)
					Console.Error.WriteLine("Stuffs bad, y'all. There are " + Math.Abs(flatVertexAmount-sharpVertexAmount) + " " + (flatVertexAmount>sharpVertexAmount?"more":"less") +
						" vertices in the flat mesh than in the sharp mesh" + "\n" + flatSword);

				var vertices = new BladeVertex[flatVertexAmount];

				String[] flatNumbers;
				String[] sharpNumbers;
				for(uint j = 0; j < vertices.Length; j++)
				{
					flatNumbers = flatLines[flatProgress].Split(' ');
					sharpNumbers = sharpLines[sharpProgress].Split(' ');

					vertices [j] = new BladeVertex(Convert.ToSingle(flatNumbers[0], CultureInfo.InvariantCulture), Convert.ToSingle(flatNumbers[1], CultureInfo.InvariantCulture),
						Convert.ToSingle(flatNumbers[2], CultureInfo.InvariantCulture), Convert.ToSingle(sharpNumbers[0], CultureInfo.InvariantCulture),
						Convert.ToSingle(sharpNumbers[1], CultureInfo.InvariantCulture), Convert.ToSingle(sharpNumbers[2], CultureInfo.InvariantCulture));

					if(flatNormals)
					{
						vertices[j].Normal.X = Convert.ToSingle(flatNumbers[3], CultureInfo.InvariantCulture);
						vertices[j].Normal.Y = Convert.ToSingle(flatNumbers[4], CultureInfo.InvariantCulture);
						vertices[j].Normal.Z = Convert.ToSingle(flatNumbers[5], CultureInfo.InvariantCulture);
					}
					if(sharpNormals)
					{
						vertices[j].SharpNormal.X = Convert.ToSingle(sharpNumbers[3], CultureInfo.InvariantCulture);
						vertices[j].SharpNormal.Y = Convert.ToSingle(sharpNumbers[4], CultureInfo.InvariantCulture);
						vertices[j].SharpNormal.Z = Convert.ToSingle(sharpNumbers[5], CultureInfo.InvariantCulture);
					}
					if(flatColors)
					{
						vertices[j].Red = Byte.Parse(flatNumbers[6])/255f;
						vertices[j].Green = Byte.Parse(flatNumbers[7])/255f;
						vertices[j].Blue = Byte.Parse(flatNumbers[8])/255f;
					}

					flatProgress++;
					sharpProgress++;
				}

				var indices = new List<uint>();

				if(flatFaceAmount != sharpFaceAmount)
					Console.Error.WriteLine("Stuffs bad, y'all. There are " + Math.Abs(flatVertexAmount-sharpVertexAmount) + " " + (flatVertexAmount>sharpVertexAmount?"more":"less") +
						" faces in the flat mesh than in the sharp mesh" + "\n" + flatSword);

				for(uint j = 0; j < flatFaceAmount; j++)
				{
					if(flatLines[flatProgress] != sharpLines[sharpProgress])
						Console.Error.WriteLine("Stuffs bad, y'all. Line " + flatProgress + " of the flat mesh and line " + sharpProgress + " of the sharp mesh don't match up!" + "\n" +
							"(" + flatLines[flatProgress] + " vs." + sharpLines[sharpProgress] + ")");

					flatNumbers = flatLines[flatProgress].Split(' ');
					if(flatNumbers[0].Equals("3"))
					{
						indices.Add(UInt32.Parse(flatNumbers[1]));
						indices.Add(UInt32.Parse(flatNumbers[2]));
						indices.Add(UInt32.Parse(flatNumbers[3]));
					}
					else if(flatNumbers[0].Equals("4"))
					{
						indices.Add(UInt32.Parse(flatNumbers[1]));
						indices.Add(UInt32.Parse(flatNumbers[2]));
						indices.Add(UInt32.Parse(flatNumbers[3]));

						indices.Add(UInt32.Parse(flatNumbers[1]));
						indices.Add(UInt32.Parse(flatNumbers[3]));
						indices.Add(UInt32.Parse(flatNumbers[4]));
					}

					flatProgress++;
					sharpProgress++;
				}

				uint[] indicesA = indices.ToArray();

				if(!flatNormals)
					BladeMesh.CalcFlatNormals(vertices, indicesA);

				if(!sharpNormals)
					BladeMesh.CalcSharpNormals(vertices, indicesA);

				MeshDataLoaded(vertices, indicesA);
			}
			catch(Exception e)
			{
				Console.Error.WriteLine ("Failed to load mesh!");
				Console.Error.WriteLine(e.StackTrace);
			}
		}

		public void MeshDataLoaded(BladeVertex[] vertices, uint[] indices)
		{
			Vertices = vertices;
			Indices = indices;
		}
	}
}

