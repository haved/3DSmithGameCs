using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class TextWriter
	{
		readonly Font UsedFont;
		Bitmap bitmap;
		Graphics gfx;
		readonly int textureID;

		public TextWriter(Font font, int width, int height)
		{
			UsedFont = font;

			bitmap = new Bitmap (width, height);
			gfx = Graphics.FromImage (bitmap);
			textureID = GL.GenTexture ();
			GL.BindTexture (TextureTarget.Texture2D, textureID);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
		}

		~TextWriter()
		{
			if(GraphicsContext.CurrentContext != null)
				GL.DeleteTexture (textureID);
			bitmap.Dispose ();
		}

		public void Resize(int width, int height)
		{
			bitmap.Dispose ();
			bitmap = new Bitmap (width, height);
			gfx = Graphics.FromImage (bitmap);

			GL.BindTexture (TextureTarget.Texture2D, textureID);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
		}

		public float GetLineHeight()
		{
			return UsedFont.Height;
		}

		public float GetLineWidth(string text)
		{
			return gfx.MeasureString (text, UsedFont).Width;
		}

		bool textureChanged;

		public void Clear()
		{
			gfx.Clear (Color.Transparent);
			textureChanged = true;
		}

		SolidBrush brush = new SolidBrush (Color.Black);
		public void DrawString(string text, float x, float y, Color color)
		{
			brush.Color = color;
			gfx.DrawString (text, UsedFont, brush, x, y);
			textureChanged = true;
		}

		public int GetTextureID()
		{
			if (textureChanged) {
				Console.Out.WriteLine ("Texture changed!");
				BitmapData data = bitmap.LockBits (new Rectangle (0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				GL.BindTexture (TextureTarget.Texture2D, textureID);
				GL.TexImage2D (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0,
					OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0); 
				bitmap.UnlockBits (data);
				textureChanged = false;
			}

			return textureID;
		}

		public int Width{ get { return bitmap.Width; } }
		public int Height{ get { return bitmap.Height; } }
	}
}

