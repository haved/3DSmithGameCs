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
			GL.TexImage2D (TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent24, shadowMapSize, shadowMapSize, 0, PixelFormat.DepthComponent, PixelType.UnsignedByte, (IntPtr)0);

			this.shadowMapSize = shadowMapSize;
		}

		static readonly Vector3 stdPos=new Vector3(0, 0, 20);
		static readonly Matrix4 orthoMatrix = Matrix4.CreateOrthographic(30, 30, 0.1f, 22);
		public INormalShader GetUseShader(Scene s, Vector3 eyePos)
		{
			VP = Matrix4.LookAt (stdPos, stdPos+LightDirection, Vector3.UnitY) * orthoMatrix;

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

			s.RenderWithShader (VP, Instance);

			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			GL.DrawBuffer (DrawBufferMode.Back);
			GL.Viewport (viewportBounds[0], viewportBounds[1], viewportBounds[2], viewportBounds[3]);

			/*GL.DepthFunc (DepthFunction.Always);
			OrthoRenderEngine.DrawColoredTextureOnEntireScreen (Util.White, shadowMap);
			GL.DepthFunc (DepthFunction.Never);*/

			GL.Enable (EnableCap.DepthTest);
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

