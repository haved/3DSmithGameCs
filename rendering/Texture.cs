using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace DSmithGameCs
{
	public class Texture
	{
		int textureID;

		public Texture (string path)
		{
			textureID = GL.GenTexture ();

			GL.BindTexture(TextureTarget.Texture2D, textureID);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

			Bitmap bmp = new Bitmap(path);
			BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
				OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

			bmp.UnlockBits(bmp_data);
		}

		~Texture()
		{
			if (GraphicsContext.CurrentContext != null)
				GL.DeleteTexture (textureID);
		}

		public int GetTextureID()
		{
			return textureID;
		}
	}
}

