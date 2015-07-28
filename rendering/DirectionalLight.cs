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

		public INormalShader GetUseShader()
		{
			ForDirectionalShader.Instance.Bind ();
			ForDirectionalShader.Instance.SetLightColor (LightColor);
			ForDirectionalShader.Instance.SetLightIntensity (LightIntensity);
			ForDirectionalShader.Instance.SetLightDirection (LightDirection);
			return ForDirectionalShader.Instance;
		}
	}
}

