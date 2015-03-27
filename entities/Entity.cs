using System;
using OpenTK;

namespace Smith2D
{
	public abstract class Entity
	{
		bool dead = false;

		public Vector3 pos = new Vector3();
		public Vector3 rot = new Vector3();
		protected Matrix4 modelspace = Matrix4.Identity;

		public abstract void Update(Scene s);

		public void UpdateModelspaceMatrix()
		{
			modelspace = Matrix4.CreateRotationZ (rot.Z) * Matrix4.CreateRotationX (rot.X) * Matrix4.CreateRotationY (rot.Y) * Matrix4.CreateTranslation (pos);
		}

		public void Render(Scene s, Matrix4 VP)
		{
			BasicShader.GetInstance ().SetMatrix4 ("modelspaceMatrix", modelspace);
			BasicShader.GetInstance ().SetMatrix4 ("MVP", modelspace * VP);
			Draw (s);
		}

		public abstract void Draw(Scene s);

		public void Kill()
		{
			dead = true;
		}

		public bool IsDead()
		{
			return dead;
		}

		public void MoveAsSolid(Scene s, float xMove, float yMove)
		{
			foreach (Entity e in s.GetEntities()) {
				if (e.IsSolid () && e != this) {
					if (GetSolidX1 () + xMove < e.GetSolidX2 () & GetSolidX2 () > e.GetSolidX1 () & GetSolidY1 () < e.GetSolidY2 () & GetSolidY2 () > e.GetSolidY1 ())
						xMove = e.GetSolidX2 () - GetSolidX1 ()+0.01f;
					if (GetSolidY1 () + yMove < e.GetSolidY2 () & GetSolidY2 () > e.GetSolidY1 () & GetSolidX1 () < e.GetSolidX2 () & GetSolidX2 () > e.GetSolidX1 ())
						yMove = e.GetSolidY2 () - GetSolidY1 ()+0.01f;
					if (GetSolidX2 () + xMove > e.GetSolidX1 () & GetSolidX1 () < e.GetSolidX2 () & GetSolidY1 () < e.GetSolidY2 () & GetSolidY2 () > e.GetSolidY1 ())
						xMove = e.GetSolidX1 () - GetSolidX2 ()-0.01f;
					if (GetSolidY2 () + yMove > e.GetSolidY1 () & GetSolidY1 () < e.GetSolidY2 () & GetSolidX1 () < e.GetSolidX2 () & GetSolidX2 () > e.GetSolidX1 ())
						yMove = e.GetSolidY1 () - GetSolidY2 ()-0.01f;
				}
			}

			pos.X += xMove;
			pos.Y += yMove;
			UpdateModelspaceMatrix ();
		}

		public virtual bool IsSolid()
		{
			return false;
		}

		public virtual float GetSolidX1()
		{
			return pos.X-1;
		}

		public virtual float GetSolidY1()
		{
			return pos.Y-1;
		}

		public virtual float GetSolidX2()
		{
			return pos.X+1;
		}

		public virtual float GetSolidY2()
		{
			return pos.Y+1;
		}
	}
}

