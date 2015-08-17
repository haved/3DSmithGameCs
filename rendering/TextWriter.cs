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
			Resize(width, height);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
		}

		~TextWriter()
		{
			if(GraphicsContext.CurrentContext != null)
				GL.DeleteTexture (textureID);
			bitmap.Dispose ();
		}

		public void DrawStandardTooltip(string[] lines, Color[] colors, string[] values)
		{
			int lineCount = Math.Max (lines.Length, values.Length);

			var widths = new float[lineCount, 2];
			float width = 0;
			float height = GetLineHeight () * lineCount;

			for (int i = 0; i < lineCount; i++) {
				if (i < lines.Length && lines[i]!=null)
					widths [i,0] = GetLineWidth (lines[i]);
				if (i < values.Length && values [i] != null)
					widths [i,1] = GetLineWidth (values [i]);
				if (widths [i,0] + widths [i,1] > width)
					width = widths [i,0] + widths [i,1];
			}

			Clear ();
			Resize ((int)width, (int)height);

			Color current = Color.Black;
			for (int i = 0; i < lineCount; i++) {
				if (i < colors.Length && colors [i] != default(Color))
					current = colors [i];
				if (widths [i,0] > 0)
					DrawString (lines [i], 0, i * GetLineHeight (), current);
				if (widths[i,1]>0)
					DrawString (values [i], width-widths[i,1], i * GetLineHeight (), Color.White);
			}
		}

		public void Resize(int width, int height)
		{
			if (width == bitmap.Width / 2 & height == bitmap.Height / 2)
				return;
			bitmap.Dispose ();
			bitmap = new Bitmap (width*2, height*2);
			gfx = Graphics.FromImage (bitmap);

			GL.BindTexture (TextureTarget.Texture2D, textureID);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
		}

		public float GetLineHeight()
		{
			return UsedFont.Height/2;
		}

		public float GetLineWidth(string text)
		{
			return gfx.MeasureString (text, UsedFont).Width/2;
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
			gfx.DrawString (text, UsedFont, brush, x*2, y*2);
			textureChanged = true;
		}

		public float GetWidthDrawString(string text, float x, float y, Color color)
		{
			DrawString (text, x, y, color);
			return GetLineWidth (text);
		}

		public int GetTextureID()
		{
			if (textureChanged) {
				//Console.Out.WriteLine ("Texture changed!");
				BitmapData data = bitmap.LockBits (new Rectangle (0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				GL.BindTexture (TextureTarget.Texture2D, textureID);
				GL.TexImage2D (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0,
					OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0); 
				bitmap.UnlockBits (data);
				textureChanged = false;
			}

			return textureID;
		}

		public int Width{ get { return bitmap.Width/2; } }
		public int Height{ get { return bitmap.Height/2; } }
	}
}

