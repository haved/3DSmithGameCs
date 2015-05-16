﻿using System;
using OpenTK;

namespace DSmithGameCs
{
	public class CastItem : Item
	{
		readonly Mesh mesh;
		readonly Vector4 color;

		public CastItem (Mesh mesh, Vector4 color)
		{
			this.mesh = mesh;
			this.color = color;
		}

		public override void Render(float x, float y, float width, float height)
		{
			OrthoRenderEngine.DrawColoredMesh (mesh, Matrix4.CreateRotationY(0.2f), color, x+4, y+4, width-8, height-8, -1, -2, 2, 4);
		}

		public override uint GetSize()
		{
			return 2;
		}
	}
}

