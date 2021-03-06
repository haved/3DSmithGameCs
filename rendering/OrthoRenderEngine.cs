﻿using OpenTK;

namespace DSmithGameCs
{
	public static class OrthoRenderEngine
	{
		static float orthoWidth, orthoHeight;

		static Matrix4 orthoMatrix;
		public static Mesh Flat;
		static Matrix4 allScreen;

		public static void Init()
		{
			Flat = new Mesh(new Vertex[]{new Vertex(0, 0, 0, 0, 1, 0, 1, 1, 1), new Vertex(1, 0, 0, 1, 1, 0, 1, 1, 1), new Vertex(1, 1, 0, 1, 0, 0, 1, 1, 1), new Vertex(0, 1, 0, 0, 0, 0, 1, 1, 1)}, new uint[]{0,1,2, 0,2,3}, false);
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
			ColorShader.Instance.Bind ();
			ColorShader.Instance.SetMVP (allScreen*orthoMatrix);
			ColorShader.Instance.SetColor (color);
			Flat.Draw ();
		}

		public static void DrawColoredTextureOnEntireScreen(Vector4 color, int texture)
		{
			TextureShader.Instance.Bind ();
			TextureShader.Instance.SetMVP (allScreen*orthoMatrix);
			TextureShader.Instance.SetColor (color);
			TextureShader.Instance.SetTexture (texture);
			TextureShader.Instance.SetSampleUV (Vector2.Zero);
			TextureShader.Instance.SetSampleSize (Util.X1Y1);
			Flat.Draw ();
		}

		public static void DrawColoredBox(Vector4 color, float x, float y, float width, float height)
		{
			ColorShader.Instance.Bind ();
			ColorShader.Instance.SetMVP (Matrix4.CreateScale(width, height, 1)*Matrix4.CreateTranslation(x, y, 0)*orthoMatrix);
			ColorShader.Instance.SetColor (color);
			Flat.Draw ();
		}

		public static void DrawTexturedBox(int texture, float x, float y, float width, float height)
		{
			DrawColoredTexturedBox (Util.White, texture, x, y, width, height);
		}

		public static void DrawColoredTexturedBox(Vector4 color, int texture, float x, float y, float width, float height)
		{
			TextureShader.Instance.Bind ();
			TextureShader.Instance.SetMVP (Matrix4.CreateScale(width, height, 1)*Matrix4.CreateTranslation(x, y, 0)*orthoMatrix);
			TextureShader.Instance.SetColor (color);
			TextureShader.Instance.SetTexture (texture);
			TextureShader.Instance.SetSampleUV (Vector2.Zero);
			TextureShader.Instance.SetSampleSize (Util.X1Y1);
			Flat.Draw ();
		}

		public static void DrawTexturedBox(int texture, float x, float y, float width, float height, float texX, float texY, float texXSize, float texYSize)
		{
			DrawColoredTexturedBox (Util.White, texture, x, y, width, height, texX, texY, texXSize, texYSize);
		}

		public static void DrawColoredTexturedBox(Vector4 color, int texture, float x, float y, float width, float height, float texX, float texY, float texXSize, float texYSize)
		{
			TextureShader.Instance.Bind ();
			TextureShader.Instance.SetMVP (Matrix4.CreateScale(width, height, 1)*Matrix4.CreateTranslation(x, y, 0)*orthoMatrix);
			TextureShader.Instance.SetColor (color);
			TextureShader.Instance.SetTexture (texture);
			TextureShader.Instance.SetSampleUV (texX, texY);
			TextureShader.Instance.SetSampleSize (texXSize, texYSize);
			Flat.Draw ();
		}

		public static void DrawColoredMesh(Mesh mesh, Vector4 color, float x, float y, float width, float height)
		{
			DrawColoredMesh (mesh, Matrix4.Identity, color, x, y, width, height);
		}

		public static readonly Matrix4 DefaultModelspace = Matrix4.CreateRotationX(0.1f);
		public static void DrawColoredMesh(Mesh mesh, Matrix4 matrix, Vector4 color, float x, float y, float width, float height)
		{
			BasicShader.Instance.Bind ();
			BasicShader.Instance.SetMVP (matrix*Matrix4.CreateScale(width/orthoWidth, height/orthoHeight, 0.9f)*
				Matrix4.CreateTranslation((width+2*x)/orthoWidth-1, (height+2*y)/orthoHeight-1, 0));
			BasicShader.Instance.SetModelspaceMatrix (DefaultModelspace);
			BasicShader.Instance.SetColor (color);
			mesh.Draw ();
		}

		public static Matrix4 GetMVPForMesh(Matrix4 matrix, float x, float y, float width, float height)
		{
			return matrix * Matrix4.CreateScale (width / orthoWidth, height / orthoHeight, 0.9f) *
			Matrix4.CreateTranslation ((width + 2 * x) / orthoWidth - 1, (height + 2 * y) / orthoHeight - 1, 0);
		}

		const float barSize = 16;
		public static void DrawExtendedColoredTexturedBox(int texture, Vector4 color, float x, float y, float width, float height)
		{
			TextureShader.Instance.Bind ();
			TextureShader.Instance.SetColor (color);
			TextureShader.Instance.SetTexture (texture);

			TextureShader.Instance.SetMVP (Matrix4.CreateScale(width-barSize*2, height-barSize*2, 1)*Matrix4.CreateTranslation(x+barSize, y+barSize, 0)*orthoMatrix);
			TextureShader.Instance.SetSampleUV (1/3f, 1/3f);
			TextureShader.Instance.SetSampleSize (1/3f, 1/3f);
			Flat.Draw ();

			//LeftSide
			TextureShader.Instance.SetMVP (Matrix4.CreateScale(barSize, height-barSize*2, 1)*Matrix4.CreateTranslation(x, y+barSize, 0)*orthoMatrix);
			TextureShader.Instance.SetSampleUV (0, 1/3f);
			TextureShader.Instance.SetSampleSize (1/3f, 1/3f);
			Flat.Draw ();

			//RightSide
			TextureShader.Instance.SetMVP (Matrix4.CreateScale(barSize, height-barSize*2, 1)*Matrix4.CreateTranslation(x+width-barSize, y+barSize, 0)*orthoMatrix);
			TextureShader.Instance.SetSampleUV (2/3f, 1/3f);
			TextureShader.Instance.SetSampleSize (1/3f, 1/3f);
			Flat.Draw ();

			//Bottom
			TextureShader.Instance.SetMVP (Matrix4.CreateScale(width-barSize*2, barSize, 1)*Matrix4.CreateTranslation(x+barSize, y, 0)*orthoMatrix);
			TextureShader.Instance.SetSampleUV (1/3f, 1/3f);
			TextureShader.Instance.SetSampleSize (1/3f, -1/3f);
			Flat.Draw ();

			//Top
			TextureShader.Instance.SetMVP (Matrix4.CreateScale(width-barSize*2, barSize, 1)*Matrix4.CreateTranslation(x+barSize, y+height-barSize, 0)*orthoMatrix);
			TextureShader.Instance.SetSampleUV (1/3f, 1);
			TextureShader.Instance.SetSampleSize (1/3f, -1/3f);
			Flat.Draw ();

			//BottomLeftCorner
			TextureShader.Instance.SetMVP (Matrix4.CreateScale(barSize, barSize, 1)*Matrix4.CreateTranslation(x, y, 0)*orthoMatrix);
			TextureShader.Instance.SetSampleUV (0, 1/3f);
			TextureShader.Instance.SetSampleSize (1/3f, -1/3f);
			Flat.Draw ();

			//BottomRightCorner
			TextureShader.Instance.SetMVP (Matrix4.CreateScale(barSize, barSize, 1)*Matrix4.CreateTranslation(x+width-barSize, y, 0)*orthoMatrix);
			TextureShader.Instance.SetSampleUV (2/3f, 1/3f);
			TextureShader.Instance.SetSampleSize (1/3f, -1/3f);
			Flat.Draw ();

			//TopLeftCorner
			TextureShader.Instance.SetMVP (Matrix4.CreateScale(barSize, barSize, 1)*Matrix4.CreateTranslation(x, y+height-barSize, 0)*orthoMatrix);
			TextureShader.Instance.SetSampleUV (0, 1);
			TextureShader.Instance.SetSampleSize (1/3f, -1/3f);
			Flat.Draw ();

			//TopRightCorner
			TextureShader.Instance.SetMVP (Matrix4.CreateScale(barSize, barSize, 1)*Matrix4.CreateTranslation(x+width-barSize, y+height-barSize, 0)*orthoMatrix);
			TextureShader.Instance.SetSampleUV (2/3f, 1);
			TextureShader.Instance.SetSampleSize (1/3f, -1/3f);
			Flat.Draw ();
		}
	}
}

