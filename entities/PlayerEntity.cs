using System;
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
		public PlayerEntity(float x, float y, Mesh mesh, Mesh shadow, Vector4 shadowColor, float xSize, float ySize)
		{
			this.Pos.X = x;
			this.Pos.Y = y;
			this.mesh = mesh;
			this.shadow = shadow;
			this.shadowColor = shadowColor;
			this.xSize = xSize/2;
			this.ySize = ySize/2;
		}

		public override void DisposeEntity()
		{
			mesh.Dispose ();
			shadow.Dispose ();
		}

		public override void Update(Scene s)
		{
			Move (s);
			if (Input.InteractKeyPressed)
				Interact (s);
		}

		Vector2 accel = new Vector2 (0, 0);
		private void Move(Scene s)
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
			
		public override void PreRender(Scene s, Matrix4 VP)
		{
			base.PreRender (s, VP);
			ColorShader.Instance.Bind ();
			ColorShader.Instance.SetMVP (MVP);
			ColorShader.Instance.SetColor (shadowColor);
			GL.Disable (EnableCap.DepthTest);
			shadow.Draw ();
			GL.Enable (EnableCap.DepthTest);
		}
		public override void Render(Scene s, Matrix4 VP, INormalShader shader)
		{
			shader.ResetColor ();
			shader.SetModelspaceMatrix (Modelspace);
			shader.SetMVP (MVP);
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

