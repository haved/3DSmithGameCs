using OpenTK;

namespace DSmithGameCs
{
	public class ForAmbientShader : Shader, INormalShader
	{
		public static ForAmbientShader Instance { get; private set; }

		int MVP_id, colorId, ambientLightId;

		public ForAmbientShader ()
		{
			AddVertexShaderFromFile ("../../res/shaders/forward-ambient.glslvs");
			AddFragmentShaderFromFile ("../../res/shaders/forward-ambient.glslfs");
			Compile ();
			MVP_id = AddUniform ("MVP");
			colorId = AddUniform ("color");
			ambientLightId = AddUniform ("ambientLight");
			SetColor (Util.White);
		}

		public void SetModelspaceMatrix(Matrix4 modelspace){}

		public void SetMVP(Matrix4 MVP)
		{
			SetMatrix4 (MVP_id, MVP);
		}

		public void SetColor(Vector4 color)
		{
			SetVector4 (colorId, color);
		}

		public void SetAmbientLight(Vector4 ambientLight)
		{
			SetVector4 (ambientLightId, ambientLight);
		}

		public void ResetColor()
		{
			SetVector4 (colorId, Util.White);
		}

		public static void MakeInstance()
		{
			if (Instance == null)
				Instance = new ForAmbientShader ();
		}

		public void SetSpecular (float spec, float power)
		{
			//throw new System.NotImplementedException ();
		}

		public void NonSpecular ()
		{
			//throw new System.NotImplementedException ();
		}
	}
}

