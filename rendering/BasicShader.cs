using System;
using OpenTK;

namespace DSmithGameCs
{
	public class BasicShader : Shader, INormalShader
	{
		static BasicShader instance;

		int modelspaceID, MVP_id, colorId;

		public BasicShader ()
		{
			AddVertexShaderFromFile ("../../res/shaders/BasicVS.glslvs");
			AddFragmentShaderFromFile ("../../res/shaders/BasicFS.glslfs");
			Compile ();
			modelspaceID = AddUniform("modelspaceMatrix");
			MVP_id = AddUniform ("MVP");
			colorId = AddUniform ("color");
			SetColor (Util.White);
		}

		public override void Bind()
		{
			base.Bind ();
		}

		public void SetModelspaceMatrix(Matrix4 modelspace)
		{
			SetMatrix4 (modelspaceID, modelspace);
		}

		public void SetMVP(Matrix4 MVP)
		{
			SetMatrix4 (MVP_id, MVP);
		}

		public void SetColor(Vector4 color)
		{
			SetVector4 (colorId, color);
		}

		public void ResetColor()
		{
			SetVector4 (colorId, Util.White);
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

