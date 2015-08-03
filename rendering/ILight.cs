using System;
using OpenTK;

namespace DSmithGameCs
{
	public interface ILight : IDisposable
	{
		INormalShader GetUseShader(Scene s, Vector3 eyePos);
	}
}

