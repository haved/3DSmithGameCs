using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class BladeMesh : IDisposable
	{
		int vbo;
		int ibo;
		int indicesCount;

		public BladeMesh(string flatSword, string sharpSword)
		{
			try {
			var loader = new BladeMeshLoader (flatSword, sharpSword);
			LoadMeshData (loader.Vertices, loader.Indices);
			}
			catch(Exception e) {
				Console.Error.WriteLine ("The blade mesh will not contain any data");
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

			GL.EnableVertexAttribArray( 3 );
			GL.EnableVertexAttribArray( 4 );
		}

		public void Draw()
		{
			//GL.EnableVertexAttribArray( 0 );
			//GL.EnableVertexAttribArray( 1 );
			//GL.EnableVertexAttribArray( 2 );
			//GL.EnableVertexAttribArray( 3 );
			//GL.EnableVertexAttribArray( 4 );

			GL.BindBuffer (BufferTarget.ArrayBuffer, vbo);

			GL.VertexAttribPointer (0, 3, VertexAttribPointerType.Float, false, BladeVertex.VERTEXBYTEAMOUNT, 0);
			GL.VertexAttribPointer (1, 3, VertexAttribPointerType.Float, false, BladeVertex.VERTEXBYTEAMOUNT, 3 * sizeof(float));
			GL.VertexAttribPointer (2, 3, VertexAttribPointerType.Float, false, BladeVertex.VERTEXBYTEAMOUNT, 6 * sizeof(float));
			GL.VertexAttribPointer (3, 3, VertexAttribPointerType.Float, false, BladeVertex.VERTEXBYTEAMOUNT, 9 * sizeof(float));
			GL.VertexAttribPointer (4, 3, VertexAttribPointerType.Float, false, BladeVertex.VERTEXBYTEAMOUNT, 12 * sizeof(float));

			GL.BindBuffer (BufferTarget.ElementArrayBuffer, ibo);
			GL.DrawElements (PrimitiveType.Triangles, indicesCount, DrawElementsType.UnsignedInt, IntPtr.Zero);

			//GL.DisableVertexAttribArray( 0 );
			//GL.DisableVertexAttribArray( 1 );
			//GL.DisableVertexAttribArray( 2 );
			//GL.DisableVertexAttribArray( 3 );
			//GL.DisableVertexAttribArray( 4 );
		}

		public static void CalcFlatNormals(BladeVertex[] vertices, uint[] indices)
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

		public static void CalcSharpNormals(BladeVertex[] vertices, uint[] indices)
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