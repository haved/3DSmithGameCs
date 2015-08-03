using System;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class PlayerEntity : Entity
	{
		readonly float walkSpeed = 100f;
		readonly float friction = 8f;
		Vector2 speed;

		Mesh mesh;
		Vector4 shadowColor;
		float xSize, ySize;
		public PlayerEntity(float x, float y, Mesh mesh, Vector4 shadowColor, float xSize, float ySize)
		{
			Pos.X = x;
			Pos.Y = y;
			this.mesh = mesh;
			this.shadowColor = shadowColor;
			this.xSize = xSize/2;
			this.ySize = ySize/2;
		}

		public override void DisposeEntity()
		{
			mesh.Dispose ();
		}

		public override void Update(Scene s)
		{
			Move (s);
			if (Input.InteractKeyPressed)
				Interact (s);
		}

		Vector2 accel = new Vector2 (0, 0);
		void Move(Scene s)
		{
			accel.X = 0;
			accel.Y = 0;

			if (Input.LeftKey)
				accel.X -= 1;
			if (Input.RightKey)
				accel.X += 1;
			if (Input.UpKey)
				accel.Y += 1;
			if (Input.DownKey)
				accel.Y -= 1;

			/*if (touchControlls && Input.MouseDown && accel.Length <= 0) { //Wonkey touch controlls
				accel.X = Input.RelativeMouseX;
				accel.Y = -Input.RelativeMouseY;
			}*/

			if (accel.Length > 0) {
				accel.Normalize ();
				speed += accel * walkSpeed * Time.Delta ();
			}
			float l = speed.Length;
			if (l > 0.01f) {
				speed.Normalize ();
				l -= l * friction * Time.Delta ();
				speed *= l;
				MoveAsSolid (s, speed.X * Time.Delta (), speed.Y * Time.Delta ());
			} else {
				speed.X = 0;
				speed.Y = 0;
			}

			Rot.Z = (float)Math.Atan2 (-Input.RelativeMouseY,Input.RelativeMouseX);

			UpdateModelspaceMatrix ();
		}

		public void LookAt(float x, float y)
		{
			Rot.Z = (float)Math.Atan2 (y-Pos.Y,x-Pos.X);
		}

		void Interact(Scene s)
		{
			foreach (Entity e in s.GetEntities()) {
				var interactiveEntity = e as InteractiveEntity;
				if(interactiveEntity != null)
				{
					if (IsLookingAt (interactiveEntity)) {
						interactiveEntity.Interact (this);
						break;
					}
				}
			}
		}

		public bool IsLookingAt(InteractiveEntity e)
		{
			return e.IsInField (Pos.Xy + new Vector2 ((float)Math.Cos (Rot.Z), (float)Math.Sin (Rot.Z)) * 3.5f);
		}

		public override void Draw(Scene s)
		{
			mesh.Draw ();
		}

		public override bool IsSolid()
		{
			return true;
		}

		public override float GetSolidX1()
		{
			return Pos.X - xSize;
		}

		public override float GetSolidY1()
		{
			return Pos.Y - ySize;
		}

		public override float GetSolidX2()
		{
			return Pos.X + xSize;
		}

		public override float GetSolidY2()
		{
			return Pos.Y + ySize;
		}
	}
}

