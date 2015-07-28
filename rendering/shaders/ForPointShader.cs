using OpenTK;

namespace DSmithGameCs
{
	public class ForPointShader : Shader, INormalShader
	{
		public static ForPointShader Instance { get; private set; }

		int modelspaceId, MVP_id, colorId, eyePosId, specularIntensityId, specularPowerId,
					pLightColor, pLightPosition, pLightIntensity, pLightRange, pLightAttenConst, pLightAttenLine, pLightAttenExpo;

		public ForPointShader ()
		{
			AddVertexShaderFromFile ("../../res/shaders/forward-point.glslvs");
			AddFragmentShaderFromFile ("../../res/shaders/forward-point.glslfs");
			Compile ();
			modelspaceId = AddUniform ("modelspaceMatrix");
			MVP_id = AddUniform ("MVP");
			colorId = AddUniform ("color");
			eyePosId = AddUniform ("eyePos");
			specularIntensityId = AddUniform ("specularIntensity");
			specularPowerId = AddUniform ("specularPower");

			pLightColor = AddUniform ("pointLight.lightColor");
			pLightPosition = AddUniform ("pointLight.position");
			pLightIntensity = AddUniform ("pointLight.intensity");
			pLightRange = AddUniform ("pointLight.range");
			pLightAttenConst = AddUniform ("pointLight.atten_constant");
			pLightAttenLine = AddUniform ("pointLight.atten_linear");
			pLightAttenExpo = AddUniform ("pointLight.atten_exponent");

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

		public void SetEyePos(Vector3 position)
		{
			SetVector3 (eyePosId, position);
		}

		public void SetSpecular(float intensity, float power)
		{
			SetFloat (specularIntensityId, intensity);
			SetFloat (specularPowerId, power);
		}

		public void NonSpecular()
		{
			SetFloat (specularIntensityId, 0);
		}

		public void SetLightColor(Vector3 color)
		{
			SetVector3 (pLightColor, color);
		}

		public void SetLightPosition(Vector3 color)
		{
			SetVector3 (pLightPosition, color);
		}

		public void SetLightIntensity(float intensity)
		{
			SetFloat (pLightIntensity, intensity);
		}

		public void SetLightRange(float range)
		{
			SetFloat (pLightRange, range);
		}

		public void SetLightAttenuationConstant(float attenConst)
		{
			SetFloat (pLightAttenConst, attenConst);
		}

		public void SetLightAttenuationLinear(float attenLine)
		{
			SetFloat (pLightAttenLine, attenLine);
		}

		public void SetLightAttenuationExpo(float attenExpo)
		{
			SetFloat (pLightAttenExpo, attenExpo);
		}

		public void ResetColor()
		{
			SetVector4 (colorId, Util.White);
		}

		public static void MakeInstance()
		{
			if (Instance == null)
				Instance = new ForPointShader ();
		}
	}
}

