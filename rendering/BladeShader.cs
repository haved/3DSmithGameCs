using System;
using OpenTK;

namespace DSmithGameCs
{
	public class BladeShader : Shader, INormalShader
	{
		static BladeShader instance;

		int modelspaceID, MVP_id, colorId, hotspotMinId, hotspotMaxId, hotspotEmissionId;

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
			SetColor (Util.White);
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

		public static void MakeInstance()
		{
			if (instance == null)
				instance = new BladeShader ();
		}

		public static BladeShader GetInstance()
		{
			return instance;
		}
	}
}

