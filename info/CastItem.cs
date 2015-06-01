﻿using System;
using OpenTK;

namespace DSmithGameCs
{
	public class CastItem : Item
	{
		readonly Mesh mesh;
		readonly Vector4 color;
		readonly string name;
		readonly float volume;
		readonly float height;

		public CastItem (Mesh mesh, Vector4 color, string name, float volume, float height)
		{
			this.mesh = mesh;
			this.color = color;
			this.name = name;
			this.volume = volume;
			this.height = height;
		}

		protected static readonly Matrix4 ItemMatrix = Matrix4.CreateRotationZ(Util.PI/2)*Matrix4.CreateRotationX(0.1f)*Matrix4.CreateRotationY(-0.1f)*Matrix4.CreateTranslation(0, 0, -2.8f)
			*Matrix4.CreatePerspectiveFieldOfView(0.85f, 1, 0.1f, 4);
		public override void RenderItem(float x, float y, float width, float height)
		{
			OrthoRenderEngine.DrawColoredMesh (mesh, ItemMatrix, color, x+4, y+4, width-8, height-8, -1, -2, 2, 4);
		}

		public Mesh GetMesh()
		{
			return mesh;
		}

		public Vector4 GetColor()
		{
			return color;
		}

		public override uint GetSize()
		{
			return 2;
		}

		public override string GetTooltipName()
		{
			return name + " Cast";
		}

		public float GetVolume()
		{
			return volume;
		}

		public float FillHeight
		{
			get { return height; }
		}

		public override Vector4 GetTooltipColor()
		{
			return color;
		}
	}
}

