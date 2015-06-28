using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class BladeShader : Shader, INormalShader
	{
		public static BladeShader Instance { get; private set; }

		int modelspaceID, MVP_id, colorId, hotspotMinId, hotspotMaxId, hotspotEmissionId, sharpnessMapId;

		public BladeShader ()
		{
			AddVertexShaderFromFile ("../../res/shaders/BladeVS.glslvs");
			AddFragmentShaderFromFile ("../../res/shaders/BladeFS.glslfs");
			Compile ();
			modelspaceID = AddUniform("modelspaceMatrix");
			MVP_id = AddUniform ("MVP");
			colorId = AddUniform ("color");
			hotspotMinId = AddUniform ("hotspotMin");
			hotspotMaxId = AddUniform ("hotspotMax");
			hotspotEmissionId = AddUniform("hotspotEmission");
			sharpnessMapId = AddUniform ("sharpnessMap");
			SetColor (Util.White);
			SetInteger (sharpnessMapId, 0);
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

		public void SetHotspotMin(float f)
		{
			SetFloat (hotspotMinId, f);
		}

		public void SetHotspotMax(float f)
		{
			SetFloat (hotspotMaxId, f);
		}

		public void SetHotspotEmission(Vector3 color)
		{
			SetVector3 (hotspotEmissionId, color);
		}

		public void SetSharpnessMap(int texture)
		{
			//GL.ActiveTexture(TextureUnit.Texture0); //This is default
			//SetInteger (diffuseId, 0); //This is done in the constructor
			GL.BindTexture(TextureTarget.Texture1D, texture);
		}

		public static void MakeInstance()
		{
			if (Instance == null)
				Instance = new BladeShader ();
		}
	}
}

