using System;
using OpenTK;

namespace DSmithGameCs
{
	public class BasicShader : Shader
	{
		private static BasicShader instance;

		private int modelspaceID, MVP_id;

		public BasicShader ()
		{
			AddVertexShaderFromFile ("../../res/shaders/BasicVS.glslvs");
			AddFragmentShaderFromFile ("../../res/shaders/BasicFS.glslfs");
			Compile ();
			modelspaceID = AddUniform("modelspaceMatrix");
			MVP_id = AddUniform ("MVP");
		}

		public void SetModelspaceMatrix(Matrix4 modelspace)
		{
			SetMatrix4 (modelspaceID, modelspace);
		}

		public void SetMVP(Matrix4 MVP)
		{
			SetMatrix4 (MVP_id, MVP);
		}
			
		public static BasicShader MakeInstance()
		{
			if (instance == null)
				instance = new BasicShader ();
			return instance;
		}

		public static BasicShader GetInstance()
		{
			return instance;
		}
	}
}

