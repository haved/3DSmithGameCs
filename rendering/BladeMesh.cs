using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class BladeMesh : IDisposable
	{
		public struct BladeVertex
		{
			public const int VERTEXFLOATAMOUNT = 15;
			public const int VERTEXBYTEAMOUNT = VERTEXFLOATAMOUNT*sizeof(float);

			public float X, Y, Z, Red, Green, Blue;
			public Vector3 Normal;
			public float SharpX, SharpY, SharpZ;
			public Vector3 SharpNormal;

			public BladeVertex(float x, float y, float z, float sharpX, float sharpY, float sharpZ)
			{
				X = x;
				Y = y;
				Z = z;
				SharpX = sharpX;
				SharpY = sharpY;
				SharpZ = sharpZ;
				Red=Green=Blue=1;
				Normal=SharpNormal=Vector3.UnitX;
			}

			public BladeVertex(float x, float y, float z, float nx, float ny, float nz, 
				float red, float green, float blue, float sharpX, float sharpY, float sharpZ, 
				float sharpNX, float sharpNY, float sharpNZ)
			{
				X = x;
				Y = y;
				Z = z;
				Normal.X = nx;
				Normal.Y = ny;
				Normal.Z = nz;
				Red = red;
				Green = green;
				Blue = blue;
				SharpX = sharpX;
				SharpY = sharpY;
				SharpZ = sharpZ;
				SharpNormal.X = sharpNX;
				SharpNormal.Y = sharpNY;
				SharpNormal.Z = sharpNZ;
			}

			public static float[] BladeVertexArrayToFloatArray(BladeVertex[] vertices)
			{
				var output = new float[vertices.Length*VERTEXFLOATAMOUNT];

				for (int i = 0; i < vertices.Length; i++) {
					output [i * VERTEXFLOATAMOUNT+0] = vertices [i].X;
					output [i * VERTEXFLOATAMOUNT+1] = vertices [i].Y;
					output [i * VERTEXFLOATAMOUNT+2] = vertices [i].Z;
					output [i * VERTEXFLOATAMOUNT+3] = vertices [i].Normal.X;
					output [i * VERTEXFLOATAMOUNT+4] = vertices [i].Normal.Y;
					output [i * VERTEXFLOATAMOUNT+5] = vertices [i].Normal.Z;
					output [i * VERTEXFLOATAMOUNT+6] = vertices [i].Red;
					output [i * VERTEXFLOATAMOUNT+7] = vertices [i].Green;
					output [i * VERTEXFLOATAMOUNT+8] = vertices [i].Blue;
					output [i * VERTEXFLOATAMOUNT+9] = vertices [i].SharpX;
					output [i * VERTEXFLOATAMOUNT+10] = vertices [i].SharpY;
					output [i * VERTEXFLOATAMOUNT+11] = vertices [i].SharpZ;
					output [i * VERTEXFLOATAMOUNT+12] = vertices [i].SharpNormal.X;
					output [i * VERTEXFLOATAMOUNT+13] = vertices [i].SharpNormal.Y;
					output [i * VERTEXFLOATAMOUNT+14] = vertices [i].SharpNormal.Z;
				}

				return output;
			}
		}

		int vbo;
		int ibo;
		int indicesCount;

		public BladeMesh(string flatSword, string sharpSword)
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
					CalcFlatNormals(vertices, indicesA);

				if(!sharpNormals)
					CalcSharpNormals(vertices, indicesA);

				LoadMeshData(vertices, indicesA);
			}
			catch(Exception e)
			{
				Console.Error.WriteLine ("Failed to load mesh!");
				Console.Error.WriteLine(e.StackTrace);
			}
		}

		~BladeMesh()
		{
			Dispose ();
		}

		bool disposed;
		public void Dispose()
		{
			if (GraphicsContext.CurrentContext != null & !disposed) {
				GL.DeleteBuffer (vbo);
				GL.DeleteBuffer (ibo);
				disposed = true;
			}
		}

		void LoadMeshData (BladeVertex[] vertices, uint[] indices)
		{
			vbo = GL.GenBuffer ();
			ibo = GL.GenBuffer ();
			indicesCount = indices.Length;

			GL.BindBuffer (BufferTarget.ArrayBuffer, vbo);
			GL.BufferData (BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * BladeVertex.VERTEXBYTEAMOUNT), 
				BladeVertex.BladeVertexArrayToFloatArray(vertices), BufferUsageHint.StaticDraw);

			GL.BindBuffer (BufferTarget.ElementArrayBuffer, ibo);
			GL.BufferData (BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)), indices, BufferUsageHint.StaticDraw);
		}

		public void Draw()
		{
			GL.EnableVertexAttribArray( 0 );
			GL.EnableVertexAttribArray( 1 );
			GL.EnableVertexAttribArray( 2 );
			GL.EnableVertexAttribArray( 3 );
			GL.EnableVertexAttribArray( 4 );

			GL.BindBuffer (BufferTarget.ArrayBuffer, vbo);

			GL.VertexAttribPointer (0, 3, VertexAttribPointerType.Float, false, BladeVertex.VERTEXBYTEAMOUNT, 0);
			GL.VertexAttribPointer (1, 3, VertexAttribPointerType.Float, false, BladeVertex.VERTEXBYTEAMOUNT, 3 * sizeof(float));
			GL.VertexAttribPointer (2, 3, VertexAttribPointerType.Float, false, BladeVertex.VERTEXBYTEAMOUNT, 6 * sizeof(float));
			GL.VertexAttribPointer (3, 3, VertexAttribPointerType.Float, false, BladeVertex.VERTEXBYTEAMOUNT, 9 * sizeof(float));
			GL.VertexAttribPointer (4, 3, VertexAttribPointerType.Float, false, BladeVertex.VERTEXBYTEAMOUNT, 12 * sizeof(float));

			GL.BindBuffer (BufferTarget.ElementArrayBuffer, ibo);
			GL.DrawElements (PrimitiveType.Triangles, indicesCount, DrawElementsType.UnsignedInt, IntPtr.Zero);

			GL.DisableVertexAttribArray( 0 );
			GL.DisableVertexAttribArray( 1 );
			GL.DisableVertexAttribArray( 2 );
			GL.DisableVertexAttribArray( 3 );
			GL.DisableVertexAttribArray( 4 );
		}

		static void CalcFlatNormals(BladeVertex[] vertices, uint[] indices)
		{
			for(int i = 0; i < indices.Length; i += 3)
			{
				uint i0 = indices[i];
				uint i1 = indices[i + 1];
				uint i2 = indices[i + 2];

				var v1 = new Vector3(vertices[i1].X - vertices[i0].X, vertices[i1].Y - vertices[i0].Y, vertices[i1].Z - vertices[i0].Z);
				var v2 = new Vector3(vertices[i2].X - vertices[i0].X, vertices[i2].Y - vertices[i0].Y, vertices[i2].Z - vertices[i0].Z);

				Vector3 normal = Vector3.Cross(v1, v2).Normalized();

				vertices[i0].Normal = vertices[i0].Normal + normal;
				vertices[i1].Normal = vertices[i1].Normal + normal;
				vertices[i2].Normal = vertices[i2].Normal + normal;
			}

			for (int i = 0; i < vertices.Length; i++)
				vertices [i].Normal.Normalize ();
		}

		static void CalcSharpNormals(BladeVertex[] vertices, uint[] indices)
		{
			for(int i = 0; i < indices.Length; i += 3)
			{
				uint i0 = indices[i];
				uint i1 = indices[i + 1];
				uint i2 = indices[i + 2];

				var v1 = new Vector3(vertices[i1].SharpX - vertices[i0].SharpX, vertices[i1].SharpY - vertices[i0].SharpY, vertices[i1].SharpZ - vertices[i0].SharpZ);
				var v2 = new Vector3(vertices[i2].SharpX - vertices[i0].SharpX, vertices[i2].SharpY - vertices[i0].SharpY, vertices[i2].SharpZ - vertices[i0].SharpZ);

				Vector3 normal = Vector3.Cross(v1, v2).Normalized();

				vertices[i0].SharpNormal = vertices[i0].SharpNormal + normal;
				vertices[i1].SharpNormal = vertices[i1].SharpNormal + normal;
				vertices[i2].SharpNormal = vertices[i2].SharpNormal + normal;
			}

			for (int i = 0; i < vertices.Length; i++)
				vertices [i].SharpNormal.Normalize ();
		}
	}
}