using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class TextureShader : Shader
	{
		private static TextureShader instance;

		private int MVP_id, colorId, diffuseId, sampleUV_id, sampleSizeId;

		public TextureShader ()
		{
			AddVertexShaderFromFile ("../../res/shaders/TextureVS.glslvs");
			AddFragmentShaderFromFile ("../../res/shaders/TextureFS.glslfs");
			Compile ();
			MVP_id = AddUniform ("MVP");
			colorId = AddUniform("color");
			diffuseId = AddUniform ("diffuse");
			sampleUV_id = AddUniform ("sampleUV");
			sampleSizeId = AddUniform ("sampleSize");
			SetColor (new Vector4(1, 1, 1, 1));
			SetSampleSize (new Vector2(1, 1));
			SetInteger (diffuseId, 0);
		}

		public void SetColor(Vector4 color)
		{
			SetVector4 (colorId, color);
		}

		public void ResetColor()
		{
			SetColor (Util.White);
		}

		public void SetMVP(Matrix4 MVP)
		{
			SetMatrix4 (MVP_id, MVP);
		}

		public void SetTexture(Texture texture)
		{
			//GL.ActiveTexture(TextureUnit.Texture0); //This is default
			//SetInteger (diffuseId, 0); //This is done in the constructor
			GL.BindTexture(TextureTarget.Texture2D, texture.GetTextureID());
		}

		public void SetTexture(int texture)
		{
			//GL.ActiveTexture(TextureUnit.Texture0); //This is default
			//SetInteger (diffuseId, 0); //This is done in the constructor
			GL.BindTexture(TextureTarget.Texture2D, texture);
		}

		public void SetSampleUV(float texX, float texY)
		{
			SetVector2 (sampleUV_id, texX, texY);
		}

		public void SetSampleSize(float texXSize, float texYSize)
		{
			SetVector2 (sampleSizeId, texXSize, texYSize);
		}

		public void SetSampleUV(Vector2 sampleUV)
		{
			SetVector2 (sampleUV_id, sampleUV);
		}

		public void SetSampleSize(Vector2 sampleSize)
		{
			SetVector2 (sampleSizeId, sampleSize);
		}

		public static void MakeInstance()
		{
			if (instance == null)
				instance = new TextureShader ();
		}

		public static TextureShader GetInstance()
		{
			return instance;
		}
	}
}

