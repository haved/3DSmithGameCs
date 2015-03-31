﻿using System;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class PlayerEntity : Entity
	{
		private readonly float walkSpeed = 100f;
		private readonly float friction = 8f;
		private Vector2 speed = new Vector2();

		Mesh mesh;
		Mesh shadow;
		Vector4 shadowColor;
		private float xSize, ySize;
		public PlayerEntity(Mesh mesh, Mesh shadow, Vector4 shadowColor, float xSize, float ySize)
		{
			this.mesh = mesh;
			this.shadow = shadow;
			this.shadowColor = shadowColor;
			this.xSize = xSize/2;
			this.ySize = ySize/2;
		}

		Vector2 accel = new Vector2 (0, 0);
		public override void Update(Scene s)
		{
			accel.X = 0;
			accel.Y = 0;

			if (Keyboard.GetState () [Key.A])
				accel.X -= 1;
			if (Keyboard.GetState () [Key.D])
				accel.X += 1;
			if (Keyboard.GetState () [Key.W])
				accel.Y += 1;
			if (Keyboard.GetState () [Key.S])
				accel.Y -= 1;

			if (accel.Length > 0) {
				accel.Normalize ();
				speed += accel * walkSpeed * Time.delta ();
			}
			float l = speed.Length;
			if (l > 0) {
				speed.Normalize();
				l -= l * friction * Time.delta ();
				speed *= l;
				MoveAsSolid (s, speed.X * Time.delta (), speed.Y * Time.delta ());
			}

			rot.Z = (float)Math.Atan2 (-Input.GetMouseY(),Input.GetMouseX());

			UpdateModelspaceMatrix ();
		}

		public override void Render(Scene s, Matrix4 VP)
		{
			Matrix4 MVP = modelspace * VP;
			ColorShader.GetInstance ().Bind ();
			ColorShader.GetInstance ().SetMVP (MVP);
			ColorShader.GetInstance ().SetColor (shadowColor);
			GL.Disable (EnableCap.DepthTest);
			GL.Enable (EnableCap.Blend);
			shadow.Draw ();
			GL.Disable (EnableCap.Blend);
			GL.Enable (EnableCap.DepthTest);
			BasicShader.GetInstance ().Bind ();
			BasicShader.GetInstance ().SetModelspaceMatrix (modelspace);
			BasicShader.GetInstance ().SetMVP (MVP);
			mesh.Draw ();
		}

		public override bool IsSolid()
		{
			return true;
		}

		public override float GetSolidX1()
		{
			return pos.X - xSize;
		}

		public override float GetSolidY1()
		{
			return pos.Y - ySize;
		}

		public override float GetSolidX2()
		{
			return pos.X + xSize;
		}

		public override float GetSolidY2()
		{
			return pos.Y + ySize;
		}
	}
}

