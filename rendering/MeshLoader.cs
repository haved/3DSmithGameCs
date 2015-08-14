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
				var buffer = new byte[fSize*9];
				file.Read(buffer, 0, sizeof(uint)*2);
				uint vertexAmount = BitConverter.ToUInt32(buffer, 0);
				UInt32 faceAmount = BitConverter.ToUInt32(buffer, sizeof(UInt32));

				var vertices = new Vertex[vertexAmount];

				for(uint j = 0; j < vertexAmount; j++)
				{
					file.Read(buffer, 0, fSize*9);
					vertices [j] = new Vertex(BitConverter.ToSingle(buffer, 0), BitConverter.ToSingle(buffer, fSize), BitConverter.ToSingle(buffer, fSize*2));
					vertices [j].Normal.X = BitConverter.ToSingle(buffer, fSize*3);
					vertices [j].Normal.X = BitConverter.ToSingle(buffer, fSize*4);
					vertices [j].Normal.X = BitConverter.ToSingle(buffer, fSize*5);
					vertices [j].Normal.X = BitConverter.ToSingle(buffer, fSize*6);
					vertices [j].Normal.X = BitConverter.ToSingle(buffer, fSize*7);
					vertices [j].Normal.X = BitConverter.ToSingle(buffer, fSize*8);
				}

				var indices = new uint[faceAmount*3];

				for(uint j = 0; j < faceAmount; j++)
				{
					file.Read(buffer, 0, sizeof(UInt32)*3);

					indices[j*3] = BitConverter.ToUInt32(buffer, 0);
					indices[j*3+1] = BitConverter.ToUInt32(buffer, sizeof(UInt32));
					indices[j*3+2] = BitConverter.ToUInt32(buffer, sizeof(UInt32)*2);
				}

				MeshDataLoaded(vertices, indices);
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
	}
}

