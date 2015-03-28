using System;

namespace DSmithGameCs
{
	public class BasicShader : Shader
	{
		private static readonly BasicShader instance = new BasicShader();

		public BasicShader ()
		{
			AddVertexShaderFromFile ("../../res/BasicVS.glslvs");
			AddFragmentShaderFromFile ("../../res/BasicFS.glslfs");
			Compile ();
			AddUniform ("modelspaceMatrix");
			AddUniform ("MVP");
		}

		public static BasicShader GetInstance()
		{
			return instance;
		}
	}
}

