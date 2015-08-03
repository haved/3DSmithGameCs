using System;
using OpenTK;

namespace DSmithGameCs
{
	public class ShadowGenShader : Shader, INormalShader
	{
		public static ShadowGenShader Instance { get; private set; }

		int MVP_id;

		public ShadowGenShader ()
		{
			AddVertexShaderFromFile ("../../res/shaders/ShadowGenVS.glslvs");
			AddFragmentShaderFromFile ("../../res/shaders/ShadowGenFS.glslfs");
			Compile ();
			MVP_id = AddUniform ("MVP");
		}

		public void SetModelspaceMatrix(Matrix4 modelspace)
		{
			
		}

		public void SetMVP(Matrix4 MVP)
		{
			SetMatrix4 (MVP_id, MVP);
		}

		public void SetColor(Vector4 color)
		{
			
		}

		public void ResetColor()
		{
			
		}

		public static void MakeInstance()
		{
			if (Instance == null)
				Instance = new ShadowGenShader ();
		}

		public void SetSpecular (float spec, float power)
		{
			
		}

		public void NonSpecular ()
		{
			
		}
	}
}

