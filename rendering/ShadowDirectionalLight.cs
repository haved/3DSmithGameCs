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

			GL.FramebufferTexture2D (FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, shadowMap, 0);

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
			GL.TexImage2D (TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, shadowMapSize, shadowMapSize, 0, PixelFormat.DepthComponent, PixelType.UnsignedByte, (IntPtr)0);

			this.shadowMapSize = shadowMapSize;
		}

		static Vector3 stdPos=new Vector3(0, 0, 20);
		static Matrix4 orthoMatrix = Matrix4.CreateOrthographic(30, 30, 0.1f, 30);
		public INormalShader GetUseShader(Scene s, Vector3 eyePos)
		{
			GL.Disable (EnableCap.Blend);
			VP = Matrix4.LookAt (stdPos, stdPos+LightDirection, Vector3.UnitY) * orthoMatrix;

			var viewportBounds = new int[4];
			GL.GetInteger (GetPName.Viewport, viewportBounds);

			GL.BindFramebuffer (FramebufferTarget.Framebuffer, frameBuffer);
			GL.BindTexture (TextureTarget.Texture2D, shadowMap);
			GL.DrawBuffer (DrawBufferMode.None);
			GL.Viewport (0, 0, shadowMapSize, shadowMapSize);
			GL.Disable (EnableCap.AlphaTest);

			GL.Clear (ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

			ShadowGenShader Instance = ShadowGenShader.Instance;
			Instance.Bind ();

			s.RenderWithShader (VP, Instance);

			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			GL.DrawBuffer (DrawBufferMode.Back);
			GL.Viewport (viewportBounds[0], viewportBounds[1], viewportBounds[2], viewportBounds[3]);
			GL.Enable (EnableCap.AlphaTest);

			GL.DepthFunc (DepthFunction.Always);
			OrthoRenderEngine.DrawColoredTextureOnEntireScreen (Util.White, shadowMap);
			GL.DepthFunc (DepthFunction.Never);

			GL.Enable (EnableCap.Blend);

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

