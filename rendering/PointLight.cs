using System;
using OpenTK;

namespace DSmithGameCs
{
	public class PointLight : ILight
	{
		public Vector3 LightColor;
		public Vector3 Position;
		public float Intensity;
		public float Range;
		public float AttenuationConstant;
		public float AttenuationLinear;
		public float AttenuationExponent;

		public PointLight (Vector3 lightColor, Vector3 position, float intensity, float range, float attenConst, float attenLine, float attenExpo)
		{
			LightColor = lightColor;
			Position = position;
			Intensity = intensity;
			Range = range;
			AttenuationConstant = attenConst;
			AttenuationLinear = attenLine;
			AttenuationExponent = attenExpo;
		}

		public INormalShader GetUseShader(Vector3 eyePos)
		{
			ForPointShader shader = ForPointShader.Instance;
			shader.Bind ();
			shader.SetEyePos (eyePos);
			shader.SetLightColor (LightColor);
			shader.SetLightPosition (Position);
			shader.SetLightIntensity (Intensity);
			shader.SetLightRange (Range);
			shader.SetLightAttenuationConstant (AttenuationConstant);
			shader.SetLightAttenuationLinear (AttenuationLinear);
			shader.SetLightAttenuationExpo (AttenuationExponent);
			shader.NonSpecular ();
			return shader;
		}
	}
}