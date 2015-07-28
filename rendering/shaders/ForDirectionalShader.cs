using OpenTK;

namespace DSmithGameCs
{
	public class ForDirectionalShader : Shader, INormalShader
	{
		public static ForDirectionalShader Instance { get; private set; }

		int modelspaceId, MVP_id, colorId, lightColorId, lightDirectionId, lightIntensityId;

		public ForDirectionalShader ()
		{
			AddVertexShaderFromFile ("../../res/shaders/forward-directional.glslvs");
			AddFragmentShaderFromFile ("../../res/shaders/forward-directional.glslfs");
			Compile ();
			modelspaceId = AddUniform ("modelspaceMatrix");
			MVP_id = AddUniform ("MVP");
			colorId = AddUniform ("color");
			lightColorId = AddUniform ("lightColor");
			lightDirectionId = AddUniform ("lightDirection");
			lightIntensityId = AddUniform ("lightIntensity");

			SetColor (Util.White);
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

		public void ResetColor()
		{
			SetVector4 (colorId, Util.White);
		}

		public static void MakeInstance()
		{
			if (Instance == null)
				Instance = new ForDirectionalShader ();
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

