using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public static class OrthoRenderEngine
	{
		static float orthoWidth, orthoHeight;

		static Matrix4 orthoMatrix;
		static Mesh flat;
		static Matrix4 allScreen;

		public static void Init()
		{
			flat = new Mesh(new Vertex[]{new Vertex(0, 0, 0, 0, 1, 0, 1, 1, 1), new Vertex(1, 0, 0, 1, 1, 0, 1, 1, 1), new Vertex(1, 1, 0, 1, 0, 0, 1, 1, 1), new Vertex(0, 1, 0, 0, 0, 0, 1, 1, 1)}, new uint[]{0,1,2, 0,2,3}, false);
		}

		public static void OnResize(int width, int height)
		{
			orthoWidth = width;
			orthoHeight = height;
			orthoMatrix = Matrix4.CreateOrthographicOffCenter (0, width, 0, height, -2, 2);
			allScreen = Matrix4.CreateScale (width, height, 1);
		}

		public static float GetCanvasWidth()
		{
			return orthoWidth;
		}

		public static float GetCanvasHeight()
		{
			return orthoHeight;
		}

		public static void DrawColorOnEntireScreen(Vector4 color)
		{
			ColorShader.GetInstance ().Bind ();
			ColorShader.GetInstance ().SetMVP (allScreen*orthoMatrix);
			ColorShader.GetInstance ().SetColor (color);
			flat.Draw ();
			BasicShader.GetInstance ().Bind ();
		}

		public static void DrawColoredTextureOnEntireScreen(Vector4 color, Texture texture)
		{
			TextureShader.GetInstance ().Bind ();
			TextureShader.GetInstance ().SetMVP (allScreen*orthoMatrix);
			TextureShader.GetInstance ().SetColor (color);
			TextureShader.GetInstance ().SetTexture (texture);
			TextureShader.GetInstance ().SetSampleUV (Vector2.Zero);
			TextureShader.GetInstance ().SetSampleSize (Util.X1Y1);
			flat.Draw ();
			BasicShader.GetInstance ().Bind ();
		}

		public static void DrawColoredBox(Vector4 color, float x, float y, float width, float height)
		{
			ColorShader.GetInstance ().Bind ();
			ColorShader.GetInstance ().SetMVP (Matrix4.CreateScale(width, height, 1)*Matrix4.CreateTranslation(x, y, 0)*orthoMatrix);
			ColorShader.GetInstance ().SetColor (color);
			flat.Draw ();
			BasicShader.GetInstance ().Bind ();
		}

		public static void DrawTexturedBox(Texture texture, float x, float y, float width, float height)
		{
			DrawColoredTexturedBox (Util.White, texture, x, y, width, height);
		}

		public static void DrawColoredTexturedBox(Vector4 color, Texture texture, float x, float y, float width, float height)
		{
			TextureShader.GetInstance ().Bind ();
			TextureShader.GetInstance ().SetMVP (Matrix4.CreateScale(width, height, 1)*Matrix4.CreateTranslation(x, y, 0)*orthoMatrix);
			TextureShader.GetInstance ().SetColor (color);
			TextureShader.GetInstance ().SetTexture (texture);
			TextureShader.GetInstance ().SetSampleUV (Vector2.Zero);
			TextureShader.GetInstance ().SetSampleSize (Util.X1Y1);
			flat.Draw ();
			BasicShader.GetInstance ().Bind ();
		}

		public static void DrawTexturedBox(Texture texture, float x, float y, float width, float height, float texX, float texY, float texXSize, float texYSize)
		{
			DrawColoredTexturedBox (Util.White, texture, x, y, width, height, texX, texY, texXSize, texYSize);
		}

		public static void DrawColoredTexturedBox(Vector4 color, Texture texture, float x, float y, float width, float height, float texX, float texY, float texXSize, float texYSize)
		{
			TextureShader.GetInstance ().Bind ();
			TextureShader.GetInstance ().SetMVP (Matrix4.CreateScale(width, height, 1)*Matrix4.CreateTranslation(x, y, 0)*orthoMatrix);
			TextureShader.GetInstance ().SetColor (color);
			TextureShader.GetInstance ().SetTexture (texture);
			TextureShader.GetInstance ().SetSampleUV (texX, texY);
			TextureShader.GetInstance ().SetSampleSize (texXSize, texYSize);
			flat.Draw ();
			BasicShader.GetInstance ().Bind ();
		}

		public static void DrawColoredMesh(Mesh mesh, Vector4 color, float x, float y, float width, float height, float meshX1, float meshY1, float meshWidth, float meshHeight)
		{
			DrawColoredMesh (mesh, Matrix4.Identity, color, x, y, width, height, meshX1, meshY1, meshWidth, meshHeight);
		}

		public static void DrawColoredMesh(Mesh mesh, Matrix4 matrix, Vector4 color, float x, float y, float width, float height, float meshX1, float meshY1, float meshWidth, float meshHeight)
		{
			BasicShader.GetInstance ().SetMVP (matrix*Matrix4.CreateTranslation(-meshX1, -meshY1, 0)*Matrix4.CreateScale(width/meshWidth, height/meshHeight, 1)*Matrix4.CreateTranslation(x, y, 0)*orthoMatrix);
			BasicShader.GetInstance ().SetModelspaceMatrix (Matrix4.Identity);
			BasicShader.GetInstance ().SetColor (color);
			mesh.Draw ();
			BasicShader.GetInstance ().ResetColor ();
		}
	}
}

