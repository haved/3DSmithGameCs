﻿using System;
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
		private int vbo;
		private int ibo;
		private int indicesCount;

		public Mesh(string filename)
		{
			try
			{
				string[] lines = System.IO.File.ReadAllLines(filename);

				uint vertexAmount = 0;
				uint faceAmount = 0;
				bool normals=false;
				bool colors=false;

				uint i = 0;
				for (; i < lines.Length; i++)
				{
					if(lines[i].StartsWith("element vertex "))
						vertexAmount = UInt32.Parse(lines[i].Substring(15));
					if(lines[i].StartsWith("element face "))
						faceAmount = UInt32.Parse(lines[i].Substring(13));
					if(lines[i].StartsWith("property float nx"))
						normals = true;
					if(lines[i].StartsWith("property uchar red"))
						colors = true; 
					if(lines[i].StartsWith("end_header"))
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
						vertices[j].normal.X = Convert.ToSingle(numbers[3], CultureInfo.InvariantCulture);
						vertices[j].normal.Y = Convert.ToSingle(numbers[4], CultureInfo.InvariantCulture);
						vertices[j].normal.Z = Convert.ToSingle(numbers[5], CultureInfo.InvariantCulture);
					}
					if(colors)
					{
						vertices[j].red = Byte.Parse(numbers[6])/255f;
						vertices[j].green = Byte.Parse(numbers[7])/255f;
						vertices[j].blue = Byte.Parse(numbers[8])/255f;
					}

					i++;
				}

				List<uint> indices = new List<uint>();

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

		public Mesh (Vertex[] vertices, uint[] indices)
		{
			CalcNormals (vertices, indices);
			LoadMeshData (vertices, indices);
		}

		~Mesh()
		{
			if (GraphicsContext.CurrentContext != null) {
				GL.DeleteBuffer (vbo);
				GL.DeleteBuffer (ibo);
			}
		}

		private void LoadMeshData (Vertex[] vertices, uint[] indices)
		{
			vbo = GL.GenBuffer ();
			ibo = GL.GenBuffer ();
			indicesCount = indices.Length;

			GL.BindBuffer (BufferTarget.ArrayBuffer, vbo);
			GL.BufferData (BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Vertex.VERTEX_BYTE_AMOUNT), 
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

			GL.VertexAttribPointer (0, 3, VertexAttribPointerType.Float, false, Vertex.VERTEX_BYTE_AMOUNT, 0);
			GL.VertexAttribPointer (1, 3, VertexAttribPointerType.Float, false, Vertex.VERTEX_BYTE_AMOUNT, 3 * sizeof(float));
			GL.VertexAttribPointer (2, 3, VertexAttribPointerType.Float, false, Vertex.VERTEX_BYTE_AMOUNT, 6 * sizeof(float));

			GL.BindBuffer (BufferTarget.ElementArrayBuffer, ibo);
			GL.DrawElements (PrimitiveType.Triangles, indicesCount, DrawElementsType.UnsignedInt, IntPtr.Zero);

			GL.DisableVertexAttribArray( 0 );
			GL.DisableVertexAttribArray( 1 );
			GL.DisableVertexAttribArray( 2 );
		}

		private void CalcNormals(Vertex[] vertices, uint[] indices)
		{
			for(int i = 0; i < indices.Length; i += 3)
			{
				uint i0 = indices[i];
				uint i1 = indices[i + 1];
				uint i2 = indices[i + 2];

				Vector3 v1 = new Vector3(vertices[i1].x - vertices[i0].x, vertices[i1].y - vertices[i0].y, vertices[i1].z - vertices[i0].z);
				Vector3 v2 = new Vector3(vertices[i2].x - vertices[i0].x, vertices[i2].y - vertices[i0].y, vertices[i2].z - vertices[i0].z);

				Vector3 normal = Vector3.Cross(v1, v2).Normalized();

				vertices[i0].normal = vertices[i0].normal + normal;
				vertices[i1].normal = vertices[i1].normal + normal;
				vertices[i2].normal = vertices[i2].normal + normal;
			}

			for (int i = 0; i < vertices.Length; i++)
				vertices [i].normal.Normalize ();
		}
	}
}

