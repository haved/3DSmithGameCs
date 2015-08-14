using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class Mesh : IDisposable
	{
		int vbo;
		int ibo;
		int indicesCount;

		public Mesh(string filename)
		{
			try {
			var loader = new MeshLoader (filename);
			LoadMeshData (loader.Vertices, loader.Indices);
			}
			catch{
				Console.Error.WriteLine ("The mesh will not contain any data");
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

		void LoadMeshData (Vertex[] vertices, uint[] indices)
		{
			vbo = GL.GenBuffer ();
			ibo = GL.GenBuffer ();
			indicesCount = indices.Length;

			GL.BindBuffer (BufferTarget.ArrayBuffer, vbo);
			GL.BufferData (BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Vertex.VERTEXBYTEAMOUNT), 
				Vertex.VertexArrayToFloatArray(vertices), BufferUsageHint.StaticDraw);

			GL.BindBuffer (BufferTarget.ElementArrayBuffer, ibo);
			GL.BufferData (BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)), indices, BufferUsageHint.StaticDraw);

			GL.EnableVertexAttribArray (0);
			GL.EnableVertexAttribArray (1);
			GL.EnableVertexAttribArray (2);
		}

		public void Draw()
		{
			//GL.EnableVertexAttribArray( 0 );
			//GL.EnableVertexAttribArray( 1 );
			//GL.EnableVertexAttribArray( 2 );

			GL.BindBuffer (BufferTarget.ArrayBuffer, vbo);

			GL.VertexAttribPointer (0, 3, VertexAttribPointerType.Float, false, Vertex.VERTEXBYTEAMOUNT, 0);
			GL.VertexAttribPointer (1, 3, VertexAttribPointerType.Float, false, Vertex.VERTEXBYTEAMOUNT, 3 * sizeof(float));
			GL.VertexAttribPointer (2, 3, VertexAttribPointerType.Float, false, Vertex.VERTEXBYTEAMOUNT, 6 * sizeof(float));

			GL.BindBuffer (BufferTarget.ElementArrayBuffer, ibo);
			GL.DrawElements (PrimitiveType.Triangles, indicesCount, DrawElementsType.UnsignedInt, IntPtr.Zero);

			//GL.DisableVertexAttribArray( 0 );
			//GL.DisableVertexAttribArray( 1 );
			//GL.DisableVertexAttribArray( 2 );
		}

		public static void CalcNormals(Vertex[] vertices, uint[] indices)
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

