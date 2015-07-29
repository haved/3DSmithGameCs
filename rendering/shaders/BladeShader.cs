using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class BladeShader : Shader, ISimpleShader
	{
		public static BladeShader Instance { get; private set; }

		int modelspaceID, MVP_id, colorId, sharpnessMapId, heatMapId;

		public BladeShader ()
		{
			AddVertexShaderFromFile ("../../res/shaders/BladeVS.glslvs");
			AddFragmentShaderFromFile ("../../res/shaders/BladeFS.glslfs");
			Compile ();
			modelspaceID = AddUniform("modelspaceMatrix");
			MVP_id = AddUniform ("MVP");
			colorId = AddUniform ("color");
			sharpnessMapId = AddUniform ("sharpnessMap");
			heatMapId = AddUniform ("heatMap");
			SetColor (Util.White);
			Bind ();
			SetInteger (sharpnessMapId, 0);
			SetInteger (heatMapId, 1);
			GL.Enable (EnableCap.Texture1D);
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

		/*public void SetSharpnessMap(int texture)
		{
			//GL.ActiveTexture(TextureUnit.Texture0); //This is default
			//SetInteger (diffuseId, 0); //This is done in the constructor
			GL.BindTexture(TextureTarget.Texture1D, texture);
		}*/

		public void SetMaps(int sharpnessMap, int heatMap)
		{
			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture1D, heatMap);
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture1D, sharpnessMap);
		}

		public void SetMaps(int sharpnessMap)
		{
			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture1D, 0);
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture1D, sharpnessMap);
		}

		public static void MakeInstance()
		{
			if (Instance == null)
				Instance = new BladeShader ();
		}
	}
}

