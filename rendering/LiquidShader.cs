using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class LiquidShader : Shader, INormalShader
	{
		public static LiquidShader Instance { get; private set; }

		readonly int modelspaceID, MVP_id, colorId, emissionId, textureId, pan0Id, pan1Id;
		readonly Texture liquidTexture;

		public LiquidShader (Texture liquidTexture)
		{
			AddVertexShaderFromFile ("../../res/shaders/LiquidVS.glslvs");
			AddFragmentShaderFromFile ("../../res/shaders/LiquidFS.glslfs");
			Compile ();
			modelspaceID = AddUniform("modelspaceMatrix");
			MVP_id = AddUniform ("MVP");
			colorId = AddUniform ("color");
			emissionId = AddUniform ("emission");
			textureId = AddUniform ("diffuse");
			pan0Id = AddUniform ("pan0");
			pan1Id = AddUniform ("pan1");
			SetColor (Util.White);
			this.liquidTexture = liquidTexture;
			SetInteger (textureId, 0);
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
			SetColor (Util.White);
		}

		public void SetEmission(Vector4 color)
		{
			SetVector4 (emissionId, color);
		}

		public void SetPan0(Vector2 pan)
		{
			SetVector2 (pan0Id, pan);
		}

		public void SetPan1(Vector2 pan)
		{
			SetVector2 (pan1Id, pan);
		}

		public void UseTexture()
		{
			GL.BindTexture (TextureTarget.Texture2D, liquidTexture.GetTextureID ());
		}

		Vector2 pan;
		public void AutoPan()
		{
			pan.X = Time.CurrentTimeMillis() * 0.00001f;
			pan.Y = Time.CurrentTimeMillis() * 0.00002f;
			SetPan0 (pan);
			pan.X = (float)Math.Sin (Time.CurrentTime () * 0.1f);
			pan.Y = (float)Math.Cos (Time.CurrentTime () * 0.12f + 1);
			SetPan1 (pan);
		}

		public static void MakeInstance(Texture texture)
		{
			if (Instance == null)
				Instance = new LiquidShader (texture);
		}
	}
}

