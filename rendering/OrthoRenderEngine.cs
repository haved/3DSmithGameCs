using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public static class OrthoRenderEngine
	{
		public static Vector4 White = new Vector4 (1, 1, 1, 1);
		public static Vector2 X1Y1 = new Vector2 (1, 1);

		static float orthoWidth, orthoHeight;

		static Matrix4 orthoMatrix;
		static Mesh flat;
		static Matrix4 allScreen;

		public static void Init()
		{
			flat = new Mesh(new Vertex[]{new Vertex(0, 0, 0, 0, 0, 0, 1, 1, 1), new Vertex(1, 0, 0, 1, 0, 0, 1, 1, 1), new Vertex(1, 1, 0, 1, 1, 0, 1, 1, 1), new Vertex(0, 1, 0, 0, 1, 0, 1, 1, 1)}, new uint[]{0,2,1, 0,3,2}, false);
		}

		public static void OnResize(int width, int height)
		{
			orthoWidth = width;
			orthoHeight = height;
			orthoMatrix = Matrix4.CreateOrthographicOffCenter (0, width, height, 0, 0, 1);
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
			TextureShader.GetInstance ().SetSampleSize (X1Y1);
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
			DrawColoredTexturedBox (White, texture, x, y, width, height);
		}

		public static void DrawColoredTexturedBox(Vector4 color, Texture texture, float x, float y, float width, float height)
		{
			TextureShader.GetInstance ().Bind ();
			TextureShader.GetInstance ().SetMVP (Matrix4.CreateScale(width, height, 1)*Matrix4.CreateTranslation(x, y, 0)*orthoMatrix);
			TextureShader.GetInstance ().SetColor (color);
			TextureShader.GetInstance ().SetTexture (texture);
			TextureShader.GetInstance ().SetSampleUV (Vector2.Zero);
			TextureShader.GetInstance ().SetSampleSize (X1Y1);
			flat.Draw ();
			BasicShader.GetInstance ().Bind ();
		}

		public static void DrawTexturedBox(Texture texture, float x, float y, float width, float height, float texX, float texY, float texXSize, float texYSize)
		{
			DrawColoredTexturedBox (White, texture, x, y, width, height, texX, texY, texXSize, texYSize);
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
	}
}

