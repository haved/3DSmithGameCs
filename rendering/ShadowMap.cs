using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class ShadowMap : IDisposable
	{
		readonly int frameBuffer;
		int shadowMapSize;
		readonly int shadowMap;
		public int ShadowMapId { get {return shadowMap;} }

		public ShadowMap (int shadowMapSize)
		{
			shadowMap = GL.GenTexture ();
			GL.BindTexture (TextureTarget.Texture2D, shadowMap);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest); 
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareFunc, (int)DepthFunction.Lequal);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)TextureCompareMode.CompareRToTexture);

			ResizeShadowMap (shadowMapSize);

			frameBuffer = GL.GenFramebuffer ();
			GL.BindFramebuffer (FramebufferTarget.Framebuffer, frameBuffer);

			GL.FramebufferTexture2D (FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, shadowMap, 0);


			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		}

		~ShadowMap()
		{
			Dispose ();
		}

		bool disposed;
		public void Dispose()
		{
			if (disposed)
				return;
			disposed = true;

			if (GraphicsContext.CurrentContext != null) {
				GL.DeleteFramebuffer (frameBuffer);
				GL.DeleteTexture (shadowMap);
			}
		}

		public void ResizeShadowMap(int shadowMapSize)
		{
			GL.BindTexture (TextureTarget.Texture2D, shadowMap);
			GL.TexImage2D (TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent24, shadowMapSize, shadowMapSize, 0, PixelFormat.DepthComponent, PixelType.UnsignedByte, (IntPtr)0);

			this.shadowMapSize = shadowMapSize;
		}

		public void UpdateShadowMap(Scene s, Matrix4 VP)
		{
			var viewportBounds = new int[4];
			GL.GetInteger (GetPName.Viewport, viewportBounds);

			GL.BindFramebuffer (FramebufferTarget.Framebuffer, frameBuffer);
			GL.DrawBuffer (DrawBufferMode.None);

			GL.Disable (EnableCap.Blend);
			GL.ClearDepth (1);

			GL.Clear (ClearBufferMask.DepthBufferBit);

			GL.Viewport (0, 0, shadowMapSize, shadowMapSize);

			ShadowGenShader Instance = ShadowGenShader.Instance;
			Instance.Bind ();

			//GL.Disable (EnableCap.CullFace);
			s.RenderWithShader (VP, Instance);
			GL.Enable (EnableCap.CullFace);

			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			GL.DrawBuffer (DrawBufferMode.Back);
			GL.Viewport (viewportBounds[0], viewportBounds[1], viewportBounds[2], viewportBounds[3]);

			/*GL.DepthFunc (DepthFunction.Always);
			OrthoRenderEngine.DrawColoredTextureOnEntireScreen (Util.White, shadowMap);
			GL.DepthFunc (DepthFunction.Never);*/

			GL.Enable (EnableCap.DepthTest);
			GL.Enable (EnableCap.Blend);
		}
	}
}

