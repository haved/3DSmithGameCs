using System;
using OpenTK;

namespace DSmithGameCs
{
	public interface INormalShader : ISimpleShader
	{
		void SetSpecular(float spec, float power);
		void NonSpecular();
		void ResetColor();
	}
}

