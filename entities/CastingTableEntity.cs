using System;
using OpenTK;

namespace DSmithGameCs
{
	public class CastingTableEntity : InteractiveEntity, EntityEventListener
	{
		Smith2DGame game;

		Matrix4 castModelspace;

		public CastingTableEntity (Smith2DGame game, Mesh m, float x, float y, float height, float xSize, float ySize) : base(m, x, y, 0, xSize, ySize)
		{
			this.game = game;
			EventHandler = this;
			castModelspace = Matrix4.CreateScale(1.5f)*Matrix4.CreateTranslation (0, 0, height)*Matrix4.CreateRotationZ((float)Math.PI/2);
		}

		public override void Render(Scene s, Matrix4 VP)
		{
			BasicShader.GetInstance ().SetModelspaceMatrix(modelspace);
			BasicShader.GetInstance ().SetMVP(modelspace * VP);
			Draw (s);
			if (game.GameStats.CurrentCast != null) {
				BasicShader.GetInstance ().SetModelspaceMatrix(castModelspace*modelspace);
				BasicShader.GetInstance ().SetMVP(castModelspace*modelspace * VP);
				BasicShader.GetInstance ().SetColor (game.GameStats.CurrentCast.GetColor());
				game.GameStats.CurrentCast.GetMesh ().Draw ();
				BasicShader.GetInstance ().ResetColor ();
			}
		}

		#region EntityEventListener implementation

		public void InteractionPerformed (InteractiveEntity entity, object source)
		{
			if (game.GameStats.CurrentCast == null & game.GameStats.PlayerInventory.HasSelectedItem ()) {
				CastItem cast = game.GameStats.PlayerInventory.GetSelectedItem () as CastItem;
				if (cast != null) {
					game.GameStats.CurrentCast = cast;
					game.GameStats.PlayerInventory.RemoveItem (game.GameStats.PlayerInventory.GetSelectedItemIndex ());
				}
			} else if (game.GameStats.CurrentCast != null && !game.GameStats.PlayerInventory.HasSelectedItem () & game.GameStats.PlayerInventory.CanFitItem(game.GameStats.CurrentCast)) {
				game.GameStats.PlayerInventory.AddItem (game.GameStats.CurrentCast);
				game.GameStats.CurrentCast = null;
			}
		}

		#endregion
	}
}

