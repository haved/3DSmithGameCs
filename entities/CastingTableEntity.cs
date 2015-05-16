using System;
using OpenTK;

namespace DSmithGameCs
{
	public class CastingTableEntity : InteractiveEntity, EntityEventListener
	{
		Smith2DGame game;
		CastItem cast;

		Matrix4 castModelspace;

		public CastingTableEntity (Smith2DGame game, Mesh m, float x, float y, float height, float xSize, float ySize) : base(m, x, y, 0, xSize, ySize)
		{
			this.game = game;
			EventHandler = this;
			castModelspace = Matrix4.CreateScale(1.5f)*Matrix4.CreateTranslation (0, 0, height)*Matrix4.CreateRotationZ((float)Math.PI/2);
		}

		public void SetCast(CastItem cast)
		{
			this.cast = cast;
		}

		public override void Render(Scene s, Matrix4 VP)
		{
			BasicShader.GetInstance ().SetModelspaceMatrix(modelspace);
			BasicShader.GetInstance ().SetMVP(modelspace * VP);
			Draw (s);
			if (cast != null) {
				BasicShader.GetInstance ().SetModelspaceMatrix(castModelspace*modelspace);
				BasicShader.GetInstance ().SetMVP(castModelspace*modelspace * VP);
				BasicShader.GetInstance ().SetColor (cast.GetColor());
				cast.GetMesh ().Draw ();
				BasicShader.GetInstance ().ResetColor ();
			}
		}

		#region EntityEventListener implementation

		public void InteractionPerformed (InteractiveEntity entity, object source)
		{
			Console.Out.WriteLine ("Yo!");
		}

		#endregion
	}
}

