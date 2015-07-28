using System;
using OpenTK;

namespace DSmithGameCs
{
	public abstract class Entity : IDisposable
	{
		bool dead = false;

		public Vector3 Pos;
		public Vector3 Rot;
		public Matrix4 Modelspace = Matrix4.Identity;

		~Entity()
		{
			Dispose ();
		}

		public abstract void Update(Scene s);

		public void UpdateModelspaceMatrix()
		{
			Modelspace = Matrix4.CreateRotationX (Rot.X) * Matrix4.CreateRotationY (Rot.Y) * Matrix4.CreateRotationZ (Rot.Z) * Matrix4.CreateTranslation (Pos);
		}

		protected Matrix4 MVP;
		public virtual void PreRender(Scene s, Matrix4 VP)
		{
			MVP = Modelspace * VP;
		}
		public virtual void PostRender(Scene s, Matrix4 VP){}

		public virtual void Render(Scene s, Matrix4 VP, INormalShader shader)
		{
			shader.ResetColor ();
			shader.SetModelspaceMatrix(Modelspace);
			shader.SetMVP(MVP);
			Draw (s);
		}

		public virtual void Draw(Scene s){}

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

			Pos.X += xMove;
			Pos.Y += yMove;
			UpdateModelspaceMatrix ();
		}

		public virtual bool IsSolid()
		{
			return false;
		}

		public virtual float GetSolidX1()
		{
			return Pos.X-1;
		}

		public virtual float GetSolidY1()
		{
			return Pos.Y-1;
		}

		public virtual float GetSolidX2()
		{
			return Pos.X+1;
		}

		public virtual float GetSolidY2()
		{
			return Pos.Y+1;
		}

		bool disposed;
		public void Dispose()
		{
			if (!disposed) {
				DisposeEntity ();
				disposed = true;
			}
		}

		public abstract void DisposeEntity();
	}
}

