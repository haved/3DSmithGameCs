using OpenTK;

namespace DSmithGameCs
{
	public interface ILight
	{
		INormalShader GetUseShader(Scene s, Vector3 eyePos);
	}
}

