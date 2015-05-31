using System.Drawing;
using OpenTK;

namespace DSmithGameCs
{
	public class CastingTableEntity : InteractiveEntity, IEntityEventListener
	{
		readonly Smith2DGame game;
		readonly Matrix4 castModelspace;

		public CastingTableEntity (Smith2DGame game, Mesh m, float x, float y, float height, float xSize, float ySize) : base(m, x, y, 0, xSize, ySize)
		{
			this.game = game;
			EventHandler = this;
			castModelspace = Matrix4.CreateScale (1.5f) * Matrix4.CreateTranslation (0, 0, height);
		}

		public override void Update(Scene s)
		{
			UpdateDialog ();
		}

		int prevVal;
		void UpdateDialog()
		{
			if (game.Player.IsLookingAt (this)) {
				if (game.TooltipHelper.ClaimIfPossible (this)) {
					game.TooltipHelper.Writer.Resize (300, 20 * 16 + 10);
					prevVal = -1;
				}
				if(game.TooltipHelper.GetOwner()==this && prevVal != (prevVal=(int)game.GameStats.FoundryTemprature))
				{
					game.TooltipHelper.Writer.Clear ();
					int y = 20;
					if (game.GameStats.CurrentCast == null)
						game.TooltipHelper.Writer.DrawString ("No cast in table", 0, 0, Color.White);
					else {
						game.TooltipHelper.Writer.DrawString (game.GameStats.CurrentCast.GetTooltipName (), 0, 0, Color.White);
						game.TooltipHelper.Writer.DrawString ("Volume: " + game.GameStats.CurrentCast.GetVolume() + " Ingots", 0, 20, Color.Green);
						y += 20;
						if (game.GameStats.FoundryAlloy.GetAmount () >= game.GameStats.CurrentCast.GetVolume ()) {
							y += 20;
							if (game.GameStats.FoundryTemprature < game.GameStats.FoundryAlloy.GetMeltingPoint ()) {
								game.TooltipHelper.Writer.DrawString ("The foundry is too cold", 0, 40, Color.Red);
							} else {
								game.TooltipHelper.Writer.DrawString ("Hit " + Input.GetKeyName (Input.POURKEY) + " to pour", 0, 40, Color.Red);
								game.TooltipHelper.Writer.DrawString ("Foundry contents:", 0, 60, Color.Green);
								y += 20;
								for (int i = 0; i < game.GameStats.FoundryAlloy.MetalTypeAmount; i++) {
									IMetal m = game.GameStats.FoundryAlloy [i];
									game.TooltipHelper.Writer.DrawString ((int)(game.GameStats.FoundryAlloy.GetMetalAmount (i) * 100 + .5f) / 100f + " " + m.GetName () + " (molten)", 10, y, Util.GetColorFromVector (m.GetColor ()));
									y += 20;
								}
							}
						} else {
							game.TooltipHelper.Writer.DrawString ("Not enough molten metal!", 0, 40, Color.Red);
							y += 20;
						}
					}
					if (y+10 != game.TooltipHelper.Writer.Height)
						game.TooltipHelper.Writer.Resize (300, y+10);

				}
			} else if (game.TooltipHelper.GetOwner () == this)
				game.TooltipHelper.UnClaim ();
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
			if (game.TooltipHelper.GetOwner () == this)
				game.TooltipHelper.RenderNormalDialog (Input.OrthoMouseX, Input.OrthoMouseY, Util.White60);
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

