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

		public Matrix4 VP;

		readonly ShadowMap shadowMap;

		public ShadowDirectionalLight (Vector3 color, float intensity, Vector3 direction, int shadowMapSize)
		{
			LightColor = color;
			LightIntensity = intensity;
			LightDirection = direction;

			shadowMap = new ShadowMap (shadowMapSize);
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

			disposed = true;

			shadowMap.Dispose();
		}

		static readonly Vector3 stdPos=new Vector3(0, 0, 20);
		static readonly Matrix4 orthoMatrix = Matrix4.CreateOrthographic(30, 30, 0.1f, 22);
		public INormalShader GetUseShader(Scene s, Vector3 eyePos)
		{
			VP = Matrix4.LookAt (stdPos, stdPos+LightDirection, Vector3.UnitY) * orthoMatrix;

			shadowMap.UpdateShadowMap (s, VP);

			ForShadowDirShader shader = ForShadowDirShader.Instance;
			shader.Bind ();
			shader.SetLightColor (LightColor);
			shader.SetLightIntensity (LightIntensity);
			shader.SetLightDirection (LightDirection);
			shader.SetShadowMap (shadowMap.ShadowMapId);
			shader.SetShadowMapMatrix (ref VP);
			return shader;
		}
	}
}

