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
			orthoMatrix = Matrix4.CreateOrthographicOffCenter (0, width, 0, height, -2, 2) * Matrix4.CreateTranslation(0, 0, 0.9f);
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
		}

		public static void DrawColoredBox(Vector4 color, float x, float y, float width, float height)
		{
			ColorShader.GetInstance ().Bind ();
			ColorShader.GetInstance ().SetMVP (Matrix4.CreateScale(width, height, 1)*Matrix4.CreateTranslation(x, y, 0)*orthoMatrix);
			ColorShader.GetInstance ().SetColor (color);
			flat.Draw ();
		}

		public static void DrawTexturedBox(Texture texture, float x, float y, float width, float height)
		{
			DrawColoredTexturedBox (Util.White, texture.GetTextureID(), x, y, width, height);
		}

		public static void DrawTexturedBox(int texture, float x, float y, float width, float height)
		{
			DrawColoredTexturedBox (Util.White, texture, x, y, width, height);
		}

		public static void DrawColoredTexturedBox(Vector4 color, Texture texture, float x, float y, float width, float height)
		{
			DrawColoredTexturedBox (color, texture.GetTextureID(), x, y, width, height);
		}

		public static void DrawColoredTexturedBox(Vector4 color, int texture, float x, float y, float width, float height)
		{
			TextureShader.GetInstance ().Bind ();
			TextureShader.GetInstance ().SetMVP (Matrix4.CreateScale(width, height, 1)*Matrix4.CreateTranslation(x, y, 0)*orthoMatrix);
			TextureShader.GetInstance ().SetColor (color);
			TextureShader.GetInstance ().SetTexture (texture);
			TextureShader.GetInstance ().SetSampleUV (Vector2.Zero);
			TextureShader.GetInstance ().SetSampleSize (Util.X1Y1);
			flat.Draw ();
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
		}

		public static void DrawColoredMesh(Mesh mesh, Vector4 color, float x, float y, float width, float height)
		{
			DrawColoredMesh (mesh, Matrix4.Identity, color, x, y, width, height);
		}

		public static void DrawColoredMesh(Mesh mesh, Matrix4 matrix, Vector4 color, float x, float y, float width, float height)
		{
			BasicShader.GetInstance ().Bind ();
			BasicShader.GetInstance ().SetMVP (matrix*Matrix4.CreateScale(width/orthoWidth, height/orthoHeight, 0)*
				Matrix4.CreateTranslation((width+2*x)/orthoWidth-1, (height+2*y)/orthoHeight-1, 0));
			BasicShader.GetInstance ().SetModelspaceMatrix (Matrix4.Identity);
			BasicShader.GetInstance ().SetColor (color);
			mesh.Draw ();
		}

		const float barSize = 16;
		public static void DrawExtendedColoredTexturedBox(Texture texture, Vector4 color, float x, float y, float width, float height)
		{
			TextureShader.GetInstance ().Bind ();
			TextureShader.GetInstance ().SetColor (color);
			TextureShader.GetInstance ().SetTexture (texture);

			TextureShader.GetInstance ().SetMVP (Matrix4.CreateScale(width-barSize*2, height-barSize*2, 1)*Matrix4.CreateTranslation(x+barSize, y+barSize, 0)*orthoMatrix);
			TextureShader.GetInstance ().SetSampleUV (1/3f, 1/3f);
			TextureShader.GetInstance ().SetSampleSize (1/3f, 1/3f);
			flat.Draw ();

			//LeftSide
			TextureShader.GetInstance ().SetMVP (Matrix4.CreateScale(barSize, height-barSize*2, 1)*Matrix4.CreateTranslation(x, y+barSize, 0)*orthoMatrix);
			TextureShader.GetInstance ().SetSampleUV (0, 1/3f);
			TextureShader.GetInstance ().SetSampleSize (1/3f, 1/3f);
			flat.Draw ();

			//RightSide
			TextureShader.GetInstance ().SetMVP (Matrix4.CreateScale(barSize, height-barSize*2, 1)*Matrix4.CreateTranslation(x+width-barSize, y+barSize, 0)*orthoMatrix);
			TextureShader.GetInstance ().SetSampleUV (2/3f, 1/3f);
			TextureShader.GetInstance ().SetSampleSize (1/3f, 1/3f);
			flat.Draw ();

			//Bottom
			TextureShader.GetInstance ().SetMVP (Matrix4.CreateScale(width-barSize*2, barSize, 1)*Matrix4.CreateTranslation(x+barSize, y, 0)*orthoMatrix);
			TextureShader.GetInstance ().SetSampleUV (1/3f, 1/3f);
			TextureShader.GetInstance ().SetSampleSize (1/3f, -1/3f);
			flat.Draw ();

			//Top
			TextureShader.GetInstance ().SetMVP (Matrix4.CreateScale(width-barSize*2, barSize, 1)*Matrix4.CreateTranslation(x+barSize, y+height-barSize, 0)*orthoMatrix);
			TextureShader.GetInstance ().SetSampleUV (1/3f, 1);
			TextureShader.GetInstance ().SetSampleSize (1/3f, -1/3f);
			flat.Draw ();

			//BottomLeftCorner
			TextureShader.GetInstance ().SetMVP (Matrix4.CreateScale(barSize, barSize, 1)*Matrix4.CreateTranslation(x, y, 0)*orthoMatrix);
			TextureShader.GetInstance ().SetSampleUV (0, 1/3f);
			TextureShader.GetInstance ().SetSampleSize (1/3f, -1/3f);
			flat.Draw ();

			//BottomRightCorner
			TextureShader.GetInstance ().SetMVP (Matrix4.CreateScale(barSize, barSize, 1)*Matrix4.CreateTranslation(x+width-barSize, y, 0)*orthoMatrix);
			TextureShader.GetInstance ().SetSampleUV (2/3f, 1/3f);
			TextureShader.GetInstance ().SetSampleSize (1/3f, -1/3f);
			flat.Draw ();

			//TopLeftCorner
			TextureShader.GetInstance ().SetMVP (Matrix4.CreateScale(barSize, barSize, 1)*Matrix4.CreateTranslation(x, y+height-barSize, 0)*orthoMatrix);
			TextureShader.GetInstance ().SetSampleUV (0, 1);
			TextureShader.GetInstance ().SetSampleSize (1/3f, -1/3f);
			flat.Draw ();

			//TopRightCorner
			TextureShader.GetInstance ().SetMVP (Matrix4.CreateScale(barSize, barSize, 1)*Matrix4.CreateTranslation(x+width-barSize, y+height-barSize, 0)*orthoMatrix);
			TextureShader.GetInstance ().SetSampleUV (2/3f, 1);
			TextureShader.GetInstance ().SetSampleSize (1/3f, -1/3f);
			flat.Draw ();
		}
	}
}

