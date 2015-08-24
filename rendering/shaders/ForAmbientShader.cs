using OpenTK;

namespace DSmithGameCs
{
	public class ForAmbientShader : Shader, INormalShader
	{
		public static ForAmbientShader Instance { get; private set; }

		int MVP_id, colorId, ambientLightId;

		#if TEST_COLOR_RESET
		bool colorReset;
		#endif

		public ForAmbientShader ()
		{
			AddVertexShaderFromFile ("../../res/shaders/forward-ambient.glslvs");
			AddFragmentShaderFromFile ("../../res/shaders/forward-ambient.glslfs");
			Compile ();
			MVP_id = AddUniform ("MVP");
			colorId = AddUniform ("color");
			ambientLightId = AddUniform ("ambientLight");
		}

		public void SetModelspaceMatrix(Matrix4 modelspace){}

		public void SetMVP(Matrix4 MVP)
		{
			SetMatrix4 (MVP_id, MVP);
		}

		public void SetColor(Vector4 color)
		{
			#if TEST_COLOR_RESET
			colorReset = false;
			#endif
			SetVector4 (colorId, color);
		}

		public void SetAmbientLight(Vector4 ambientLight)
		{
			SetVector4 (ambientLightId, ambientLight);
		}

		public void ResetColor()
		{
			#if TEST_COLOR_RESET
			colorReset = true;
			#endif
			SetVector4 (colorId, Util.White);
		}

		#if TEST_COLOR_RESET
		public bool IsColorReset()
		{
			return colorReset;
		}
		#endif

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

