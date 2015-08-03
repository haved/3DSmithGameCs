using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class ShadowDirectionalLight : ILight
	{
		public Vector3 LightColor;
		public float LightIntensity;
		public Vector3 LightDirection;

		readonly int frameBuffer;
		int shadowMapSize;
		readonly int shadowMap;
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

			ResizeShadowMap (shadowMapSize);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest); 
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareFunc, (int)DepthFunction.Lequal);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)TextureCompareMode.None);

			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
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

			if (GraphicsContext.CurrentContext != null) {
				GL.DeleteFramebuffer (frameBuffer);
				GL.DeleteTexture (shadowMap);
			}

			disposed = true;
		}

		public void ResizeShadowMap(int shadowMapSize)
		{
			GL.BindTexture (TextureTarget.Texture2D, shadowMap);
			GL.TexImage2D (TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, shadowMapSize, shadowMapSize, 0, PixelFormat.DepthComponent, PixelType.Float, (IntPtr)0);

			this.shadowMapSize = shadowMapSize;
		}

		public INormalShader GetUseShader(Scene s, Vector3 eyePos)
		{
			var viewportBounds = new int[4];
			GL.GetInteger (GetPName.Viewport, viewportBounds);

			Console.Out.WriteLine ("{0}, {1}, {2}, {3}", viewportBounds[0], viewportBounds[1], viewportBounds[2], viewportBounds[3]);

			GL.BindFramebuffer (FramebufferTarget.Framebuffer, frameBuffer);
			GL.Viewport (0, 0, shadowMapSize, shadowMapSize);

			GL.Clear (ClearBufferMask.DepthBufferBit);

			ForAmbientShader Instance = ForAmbientShader.Instance;
			Instance.Bind ();
			Instance.ResetColor ();

			s.RenderWithShader (VP, Instance);

			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			GL.Viewport (viewportBounds[0], viewportBounds[1], viewportBounds[2], viewportBounds[3]);

			OrthoRenderEngine.DrawColoredTextureOnEntireScreen (Util.White, shadowMap);

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

