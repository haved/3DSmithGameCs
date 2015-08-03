using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class ForShadowDirShader : Shader, INormalShader
	{
		public static ForShadowDirShader Instance { get; private set; }

		int modelspaceId, MVP_id, colorId, lightColorId, lightDirectionId, lightIntensityId, shadowMapId, shadowMapMatrixId;

		public ForShadowDirShader ()
		{
			AddVertexShaderFromFile ("../../res/shaders/forward-shadowdir.glslvs");
			AddFragmentShaderFromFile ("../../res/shaders/forward-shadowdir.glslfs");
			Compile ();
			modelspaceId = AddUniform ("modelspaceMatrix");
			MVP_id = AddUniform ("MVP");
			colorId = AddUniform ("color");
			lightColorId = AddUniform ("lightColor");
			lightDirectionId = AddUniform ("lightDirection");
			lightIntensityId = AddUniform ("lightIntensity");
			shadowMapId = AddUniform ("shadowMap");
			shadowMapMatrixId = AddUniform ("shadowMapMatrix");

			Bind ();
			SetColor (Util.White);
			SetInteger (shadowMapId, 0);
		}

		public void SetModelspaceMatrix(Matrix4 modelspace)
		{
			SetMatrix4 (modelspaceId, modelspace);
		}

		public void SetMVP(Matrix4 MVP)
		{
			SetMatrix4 (MVP_id, MVP);
		}

		public void SetColor(Vector4 color)
		{
			SetVector4 (colorId, color);
		}

		public void SetLightColor(Vector3 color)
		{
			SetVector3 (lightColorId, color);
		}

		public void SetLightDirection(Vector3 dir)
		{
			SetVector3 (lightDirectionId, dir);
		}

		public void SetLightIntensity(float intensity)
		{
			SetFloat (lightIntensityId, intensity);
		}

		public void SetShadowMap(int shadowMap)
		{
			GL.BindTexture (TextureTarget.Texture2D, shadowMap);
		}

		public void SetShadowMapMatrix(ref Matrix4 matrix)
		{
			SetMatrix4 (shadowMapMatrixId, matrix);
		}

		public void ResetColor()
		{
			SetVector4 (colorId, Util.White);
		}

		public static void MakeInstance()
		{
			if (Instance == null)
				Instance = new ForShadowDirShader ();
		}

		public void SetSpecular (float spec, float power)
		{
			//throw new System.NotImplementedException ();
		}

		public void NonSpecular ()
		{
			//throw new System.NotImplementedException ();
		}
	}
}

