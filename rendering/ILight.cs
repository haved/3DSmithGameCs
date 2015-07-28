using OpenTK;

namespace DSmithGameCs
{
	public interface ILight
	{
		INormalShader GetUseShader(Vector3 eyePos);
	}
}

