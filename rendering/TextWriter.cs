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
		public class Text
		{
			public Brush UsedColor;
			public PointF Position;
			public string Chars;

			public Text(String text, PointF position, Color color)
			{
				Chars = text;
				Position = position;
				UsedColor = new SolidBrush(color);
			}
		}

		readonly Font UsedFont;
		readonly List<Text> lines;
		Bitmap bitmap;
		readonly int textureID;

		public TextWriter(Font font, int width, int height)
		{
			UsedFont = font;
			lines = new List<Text> ();

			bitmap = new Bitmap (width, height);
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
		}

		public void Resize(int width, int height)
		{
			bitmap.Dispose ();
			bitmap = new Bitmap (width, height);

			GL.BindTexture (TextureTarget.Texture2D, textureID);
			GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, bitmap.Width, bitmap.Height,
				OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
		}

		public void AddLine(string s, PointF pos, Color color)
		{
			lines.Add (new Text(s, pos, color));
		}

		public void AddLine(Text text)
		{
			lines.Add (text);
		}

		public Text GetLine(int index)
		{
			return lines [index];
		}

		public void RenderLines()
		{
			using (Graphics gfx = Graphics.FromImage (bitmap)) {
				gfx.Clear(Color.Transparent);
				foreach (Text line in lines) {
					gfx.DrawString (line.Chars, UsedFont, line.UsedColor, line.Position);
				}
			}

			BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			GL.BindTexture (TextureTarget.Texture2D, textureID);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0,
				OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0); 
			bitmap.UnlockBits(data);
		}

		public int TextureID { get { return textureID; } }

		public int Width{ get { return bitmap.Width; } }
		public int Height{ get { return bitmap.Height; } }
	}
}

