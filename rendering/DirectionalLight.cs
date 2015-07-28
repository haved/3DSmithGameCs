using OpenTK;

namespace DSmithGameCs
{
	public class DirectionalLight : ILight
	{
		public Vector3 LightColor;
		public float LightIntensity;
		public Vector3 LightDirection;

		public DirectionalLight (Vector3 color, float intensity, Vector3 direction)
		{
			LightColor = color;
			LightIntensity = intensity;
			LightDirection = direction;
		}

		public INormalShader GetUseShader(Vector3 eyePos)
		{
			ForDirectionalShader shader = ForDirectionalShader.Instance;
			shader.Bind ();
			shader.SetLightColor (LightColor);
			shader.SetLightIntensity (LightIntensity);
			shader.SetLightDirection (LightDirection);
			return shader;
		}
	}
}

