using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class ShadowDirectionalLight : ILight, IDisposable
	{
		public Vector3 LightColor;
		public float LightIntensity;
		public Vector3 LightDirection;

		public int frameBuffer;
		public int shadowMap;
		public Matrix4 VP;

		public ShadowDirectionalLight (Vector3 color, float intensity, Vector3 direction, int shadowMapSize)
		{
			LightColor = color;
			LightIntensity = intensity;
			LightDirection = direction;

			frameBuffer = GL.GenFramebuffer ();
			GL.BindFramebuffer (FramebufferTarget.Framebuffer, frameBuffer);

			shadowMap = GL.GenTexture ();
			GL.BindTexture (TextureTarget.Texture2D, shadowMap);

			GL.TexImage2D (TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, shadowMapSize, shadowMapSize, 0, PixelFormat.DepthComponent, PixelType.Float, (IntPtr)0);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest); 
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

		}

		~ShadowDirectionalLight()
		{
			Dispose ();
		}

		bool disposed;
		public void Dispose()
		{
			if (disposed)
				return;

			GL.DeleteFramebuffer (frameBuffer);
			GL.DeleteTexture (shadowMap);

			disposed = true;
		}

		public INormalShader GetUseShader(Scene s, Vector3 eyePos)
		{
			

			ForShadowDirShader shader = ForShadowDirShader.Instance;
			shader.Bind ();
			shader.SetLightColor (LightColor);
			shader.SetLightIntensity (LightIntensity);
			shader.SetLightDirection (LightDirection);
			shader.SetShadowMap (shadowMap);
			shader.SetShadowMapMatrix (ref VP);
			return shader;
		}
	}
}

