using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using OpenTK;

namespace DSmithGameCs
{
	public class MeshLoader
	{
		public Vertex[] Vertices;
		public uint[] Indices;

		public MeshLoader(string filename)
		{
			try
			{
				string[] lines = File.ReadAllLines(filename);

				uint vertexAmount = 0;
				uint faceAmount = 0;
				bool normals=false;
				bool colors=false;

				uint i = 0;
				for (; i < lines.Length; i++)
				{
					if (lines [i].StartsWith ("element vertex ", StringComparison.Ordinal))
						vertexAmount = UInt32.Parse (lines [i].Substring (15));
					if (lines [i].StartsWith ("element face ", StringComparison.Ordinal))
						faceAmount = UInt32.Parse (lines [i].Substring (13));
					normals |= lines [i].StartsWith ("property float nx", StringComparison.Ordinal);
					colors |= lines [i].StartsWith ("property uchar red", StringComparison.Ordinal); 
					if (lines [i].StartsWith ("end_header", StringComparison.Ordinal))
						break;
				}
				i++;

				Vertex[] vertices = new Vertex[vertexAmount];

				String[] numbers;
				for(uint j = 0; j < vertexAmount; j++)
				{
					numbers = lines[i].Split(' ');
					vertices [j] = new Vertex(Convert.ToSingle(numbers[0], CultureInfo.InvariantCulture), Convert.ToSingle(numbers[1], CultureInfo.InvariantCulture), Convert.ToSingle(numbers[2], CultureInfo.InvariantCulture));
					if(normals)
					{
						vertices[j].Normal.X = Convert.ToSingle(numbers[3], CultureInfo.InvariantCulture);
						vertices[j].Normal.Y = Convert.ToSingle(numbers[4], CultureInfo.InvariantCulture);
						vertices[j].Normal.Z = Convert.ToSingle(numbers[5], CultureInfo.InvariantCulture);
					}
					if(colors)
					{
						vertices[j].Red = Byte.Parse(numbers[6])/255f;
						vertices[j].Green = Byte.Parse(numbers[7])/255f;
						vertices[j].Blue = Byte.Parse(numbers[8])/255f;
					}

					i++;
				}

				var indices = new List<uint>();

				for(uint j = 0; j < faceAmount; j++)
				{
					numbers = lines[i].Split(' ');
					if(numbers[0].Equals("3"))
					{
						indices.Add(UInt32.Parse(numbers[1]));
						indices.Add(UInt32.Parse(numbers[2]));
						indices.Add(UInt32.Parse(numbers[3]));
					}
					else if(numbers[0].Equals("4"))
					{
						indices.Add(UInt32.Parse(numbers[1]));
						indices.Add(UInt32.Parse(numbers[2]));
						indices.Add(UInt32.Parse(numbers[3]));

						indices.Add(UInt32.Parse(numbers[1]));
						indices.Add(UInt32.Parse(numbers[3]));
						indices.Add(UInt32.Parse(numbers[4]));
					}

					i++;
				}

				uint[] indicesA = indices.ToArray();

				if(!normals)
					Mesh.CalcNormals(vertices, indicesA);

				MeshDataLoaded(vertices, indicesA);
			}
			catch(Exception e)
			{
				Console.Error.WriteLine ("Failed to load mesh! File: " + filename);
				Console.Error.WriteLine(e.StackTrace);

				throw e; //I'm not handeling this shit
			}
		}

		const int fSize = sizeof(float);
		public MeshLoader(Stream file)
		{
			try
			{
				using(var reader = new BinaryReader(file)){

					UInt32 vertexAmount = reader.ReadUInt32();
					UInt32 faceAmount = reader.ReadUInt32();

					var vertices = new Vertex[vertexAmount];

					for(uint j = 0; j < vertexAmount; j++)
					{
						vertices [j] = new Vertex(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
						vertices [j].Normal.X = reader.ReadSingle();
						vertices [j].Normal.Y = reader.ReadSingle();
						vertices [j].Normal.Z = reader.ReadSingle();
						vertices [j].Red = reader.ReadSingle();
						vertices [j].Green = reader.ReadSingle();
						vertices [j].Blue = reader.ReadSingle();
					}

					var indices = new uint[faceAmount*3];

					for(uint j = 0; j < faceAmount; j++)
					{
						indices[j*3] = reader.ReadUInt32();
						indices[j*3+1] = reader.ReadUInt32();
						indices[j*3+2] = reader.ReadUInt32();
					}

					MeshDataLoaded(vertices, indices);
				}
			}
			catch(Exception e)
			{
				Console.Error.WriteLine ("Failed to load mesh! Stream: " + file);
				Console.Error.WriteLine(e.StackTrace);

				throw e; //I'm not handeling this shit
			}
		}

		public void MeshDataLoaded(Vertex[] vertices, uint[] indices)
		{
			Vertices = vertices;
			Indices = indices;
		}

		public void WriteTo(string filename)
		{
			using (var writer = new StreamWriter (filename, false)) {
				writer.WriteLine (@"ply
				format ascii 1.0
				comment Created by MeshLoader.WriteTo({0})'
				element vertex {1}
				property float x
				property float y
				property float z
				property float nx
				property float ny
				property float nz
				property uchar red
				property uchar green
				property uchar blue
				element face {2}
				property list uchar uint vertex_indices
				end_header", filename, Vertices.Length, Indices.Length / 3);

				foreach (Vertex v in Vertices)
					writer.WriteLine ("{0} {1} {2} {3} {4} {5} {6} {7} {8}", v.X, v.Y, v.Z, v.Normal.X, v.Normal.Y, v.Normal.Z, (byte)(v.Red * 255), (byte)(v.Green * 255), (byte)(v.Blue * 255));
			
				for (int i = 0; i < Indices.Length; i += 3)
					writer.WriteLine ("3 {0} {1} {2}", Indices [i], Indices [i + 1], Indices [i + 2]);

				writer.Flush ();
			}
		}

		public void WriteTo(Stream stream)
		{

		}
	}
}

