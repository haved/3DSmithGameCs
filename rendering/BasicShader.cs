using System;

namespace Smith2D
{
	public class BasicShader : Shader
	{
		private static BasicShader instance = new BasicShader();

		public BasicShader ()
		{
			AddVertexShaderFromFile ("../../res/BasicVS.glslvs");
			AddFragmentShaderFromFile ("../../res/BasicFS.glslfs");
			Compile ();
			AddUniform ("projectionMatrix");
			AddUniform ("viewMatrix");
			AddUniform ("modelspaceMatrix");
		}

		public static BasicShader GetInstance()
		{
			return instance;
		}
	}
}

