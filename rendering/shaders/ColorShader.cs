﻿using System;
using OpenTK;

namespace DSmithGameCs
{
	public class ColorShader : Shader
	{
		public static ColorShader Instance { get; private set; }

		int MVP_id, colorId;

		public ColorShader ()
		{
			AddVertexShaderFromFile ("../../res/shaders/ColorVS.glslvs");
			AddFragmentShaderFromFile ("../../res/shaders/ColorFS.glslfs");
			Compile ();
			MVP_id = AddUniform ("MVP");
			colorId = AddUniform("color");
			SetColor (new Vector4(1, 1, 1, 1));
		}

		public void SetColor(Vector4 color)
		{
			SetVector4 (colorId, color);
		}

		public void SetMVP(Matrix4 MVP)
		{
			SetMatrix4 (MVP_id, MVP);
		}

		public static void MakeInstance()
		{
			if (Instance == null)
				Instance = new ColorShader ();
		}
	}
}

