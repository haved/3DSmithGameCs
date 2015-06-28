using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class Mesh
	{
		int vbo;
		int ibo;
		int indicesCount;

		public Mesh(string filename)
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
					CalcNormals(vertices, indicesA);

				LoadMeshData(vertices, indicesA);
			}
			catch(Exception e)
			{
				Console.Error.WriteLine ("Failed to load mesh!");
				Console.Error.WriteLine(e.StackTrace);
			}
		}

		public Mesh (Vertex[] vertices, uint[] indices) : this(vertices, indices, true){}

		public Mesh (Vertex[] vertices, uint[] indices, bool calcNormals)
		{
			if(calcNormals)
				CalcNormals (vertices, indices);
			LoadMeshData (vertices, indices);
		}

		~Mesh()
		{
			if (GraphicsContext.CurrentContext != null) {
				GL.DeleteBuffer (vbo);
				GL.DeleteBuffer (ibo);
			}
			Console.Out.WriteLine ("Deleted vbo:" + vbo);
		}

		void LoadMeshData (Vertex[] vertices, uint[] indices)
		{
			vbo = GL.GenBuffer ();
			Console.Out.WriteLine ("Assigned vbo:" + vbo);
			ibo = GL.GenBuffer ();
			indicesCount = indices.Length;

			GL.BindBuffer (BufferTarget.ArrayBuffer, vbo);
			GL.BufferData (BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Vertex.VERTEXBYTEAMOUNT), 
				Vertex.VertexArrayToFloatArray(vertices), BufferUsageHint.StaticDraw);

			GL.BindBuffer (BufferTarget.ElementArrayBuffer, ibo);
			GL.BufferData (BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)), indices, BufferUsageHint.StaticDraw);
		}

		public void Draw()
		{
			GL.EnableVertexAttribArray( 0 );
			GL.EnableVertexAttribArray( 1 );
			GL.EnableVertexAttribArray( 2 );

			GL.BindBuffer (BufferTarget.ArrayBuffer, vbo);

			GL.VertexAttribPointer (0, 3, VertexAttribPointerType.Float, false, Vertex.VERTEXBYTEAMOUNT, 0);
			GL.VertexAttribPointer (1, 3, VertexAttribPointerType.Float, false, Vertex.VERTEXBYTEAMOUNT, 3 * sizeof(float));
			GL.VertexAttribPointer (2, 3, VertexAttribPointerType.Float, false, Vertex.VERTEXBYTEAMOUNT, 6 * sizeof(float));

			GL.BindBuffer (BufferTarget.ElementArrayBuffer, ibo);
			GL.DrawElements (PrimitiveType.Triangles, indicesCount, DrawElementsType.UnsignedInt, IntPtr.Zero);

			GL.DisableVertexAttribArray( 0 );
			GL.DisableVertexAttribArray( 1 );
			GL.DisableVertexAttribArray( 2 );
		}

		static void CalcNormals(Vertex[] vertices, uint[] indices)
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
	}
}

