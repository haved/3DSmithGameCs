using System;
using OpenTK;

namespace DSmithGameCs
{
	public class CastingTableEntity : InteractiveEntity, EntityEventListener
	{
		readonly Smith2DGame game;
		readonly Matrix4 castModelspace;

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
			Inventory playerInv = game.GameStats.PlayerInventory;
			if (game.GameStats.CurrentCast == null & playerInv.HasSelectedItem ()) {
				CastItem cast = playerInv.GetSelectedItem () as CastItem;
				if (cast != null) {
					game.GameStats.CurrentCast = cast;
					playerInv.RemoveItem (playerInv.GetSelectedItemIndex ());
				}
			} else if (game.GameStats.CurrentCast != null && !playerInv.HasSelectedItem ()) {
				if (playerInv.CanFitItem (game.GameStats.CurrentCast)) {
					playerInv.AddItem (game.GameStats.CurrentCast);
					game.GameStats.CurrentCast = null;
				} else {
					playerInv.InventoryTooFull (game.GameStats.CurrentCast);
				}
			}
		}

		#endregion
	}
}

