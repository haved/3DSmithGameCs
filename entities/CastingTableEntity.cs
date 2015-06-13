using System;
using System.Drawing;
using OpenTK;

namespace DSmithGameCs
{
	public class CastingTableEntity : InteractiveEntity, IEntityEventListener
	{
		readonly Smith2DGame game;
		readonly Matrix4 castModelspace;

		readonly Mesh fill;
		readonly Mesh fall;
		readonly Matrix4 fillMatrix;

		public CastingTableEntity (Smith2DGame game, Mesh m, Mesh fill, Mesh fall, float x, float y, float height, float xSize, float ySize) : base(m, x, y, 0, xSize, ySize)
		{
			this.game = game;
			this.fill = fill;
			this.fall = fall;
			EventHandler = this;
			castModelspace = Matrix4.CreateScale (1.5f) * Matrix4.CreateTranslation (0, 0, height);
			fillMatrix = Matrix4.CreateTranslation (0, 0, height-0.08f);
		}

		public override void Update(Scene s)
		{
			UpdateDialog ();
			if (Input.PourKeyPressed & game.TooltipHelper.GetOwner () == this) {
				if (game.GameStats.CurrentCast != null & game.GameStats.FoundryAlloy != null && game.GameStats.FoundryAlloy.GetAmount () >= game.GameStats.CurrentCast.GetVolume () - 0.005f &
																	game.GameStats.CastFilling <= 0 & game.GameStats.FoundryTemprature > game.GameStats.FoundryAlloy.GetMeltingPoint()) {
					game.GameStats.CastMetal = BasicMetal.Zinc.Id;
					game.GameStats.CastFilling = 0.01f;
					game.GameStats.OldFoundryAmount = game.GameStats.FoundryAlloy.GetAmount ();
					game.GameStats.CastingTemprature = game.GameStats.FoundryTemprature;
				}
			}

			if (game.GameStats.CastFilling > 0 & game.GameStats.CastFilling < 1) {
				game.GameStats.CastFilling += Time.Delta();
				if (game.GameStats.CastFilling >= 1) {
					game.GameStats.CastFilling = 1;
				}
				game.GameStats.FoundryAlloy.SetAmount (game.GameStats.OldFoundryAmount-game.GameStats.CurrentCast.GetVolume()*game.GameStats.CastFilling);
			}

			if (game.GameStats.CastingTemprature > 25)
				game.GameStats.CastingTemprature -= Time.Delta ()*150;
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
						game.TooltipHelper.Writer.DrawString (Localization.GetLocalization("ui.tooltip.nocast"), 0, 0, Color.White);
					else {
						game.TooltipHelper.Writer.DrawString (game.GameStats.CurrentCast.GetTooltipName (), 0, 0, Color.White);
						game.TooltipHelper.Writer.DrawString (Localization.GetLocalization("ui.tooltip.volume:") + game.GameStats.CurrentCast.GetVolume() + " " + Localization.GetLocalization("ui.tooltip.unit.ingot" + ((int)game.GameStats.CurrentCast.GetVolume() == 1 ? "" : ".plural")), 0, 20, Color.Green);
						y += 20;
						if (game.GameStats.CastFilling > 0) {
							if (game.GameStats.CastFilling >= 1)
								if(game.GameStats.CastingTemprature > 25)
									game.TooltipHelper.Writer.DrawString ("The casting is too hot (" + (int)game.GameStats.CastingTemprature + "°C)", 0, 40, Color.Red);
								else	
									game.TooltipHelper.Writer.DrawString ("Hit " + Input.GetKeyName(Input.INTERACTKEY) + " to pick up the casting", 0, 40, Color.Green);
							else
								game.TooltipHelper.Writer.DrawString ("Casting...", 0, 40, Color.Red);
							y += 20;
						}
						else if (game.GameStats.FoundryAlloy.GetAmount () >= game.GameStats.CurrentCast.GetVolume ()-0.005f) {
							y += 20;
							if (game.GameStats.FoundryTemprature < game.GameStats.FoundryAlloy.GetMeltingPoint ()) {
								game.TooltipHelper.Writer.DrawString ("The foundry is too cold", 0, 40, Color.Red);
							} else {
								game.TooltipHelper.Writer.DrawString ("Hit " + Input.GetKeyName (Input.POURKEY) + " to pour", 0, 40, Color.Red);
								game.TooltipHelper.Writer.DrawString ("Foundry contents:", 0, 60, Color.Green);
								y += 20;
								for (int i = 0; i < game.GameStats.FoundryAlloy.MetalTypeAmount; i++) {
									BasicMetal m = game.GameStats.FoundryAlloy [i];
									game.TooltipHelper.Writer.DrawString ((int)(game.GameStats.FoundryAlloy.GetMetalAmount (i) * 100 + .5f) / 100f + " " + m.GetName () + " (molten)", 10, y, Util.GetColorFromVector (m.GetColor ()));
									y += 20;
								}
							}
						} else {
							game.TooltipHelper.Writer.DrawString (Localization.GetLocalization("ui.tooltip.notenoughmoltenmetal"), 0, 40, Color.Red);
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
			BasicShader.GetInstance ().Bind ();
			BasicShader.GetInstance ().SetModelspaceMatrix(Modelspace);
			BasicShader.GetInstance ().SetMVP(Modelspace * VP);
			Draw (s);
			if (game.GameStats.CurrentCast != null) {
				BasicShader.GetInstance ().SetModelspaceMatrix(castModelspace*Modelspace);
				BasicShader.GetInstance ().SetMVP(castModelspace*Modelspace * VP);
				BasicShader.GetInstance ().SetColor (game.GameStats.CurrentCast.GetColor());
				game.GameStats.CurrentCast.GetMesh ().Draw ();
				if (game.GameStats.CastFilling > 0) {
					Matrix4 fallModelspace = fillMatrix*Modelspace;
					Matrix4 fillModelspace = Matrix4.CreateScale (1, 1, game.GameStats.CastFilling * game.GameStats.CurrentCast.FillHeight)*fallModelspace;

					INormalShader shader;
					if (game.GameStats.CastingTemprature > 25) {
						shader = LiquidShader.GetInstance ();
						shader.Bind ();
						LiquidShader.GetInstance ().UseTexture ();
						LiquidShader.GetInstance ().AutoPan ();
						LiquidShader.GetInstance ().SetEmission (Util.DefaultEmission);
					} else
						shader = BasicShader.GetInstance ();

					shader.SetModelspaceMatrix(fillModelspace);
					shader.SetMVP (fillModelspace*VP);
					shader.SetColor (BasicMetal.Metals[game.GameStats.CastMetal].GetColor());
					fill.Draw ();
					if (game.GameStats.CastFilling < 1) {
						LiquidShader.GetInstance ().SetModelspaceMatrix(fallModelspace);
						LiquidShader.GetInstance ().SetMVP (fallModelspace*VP);
						if (!(shader is LiquidShader)) {
							LiquidShader.GetInstance ().Bind ();
							LiquidShader.GetInstance ().SetColor (BasicMetal.Metals[game.GameStats.CastMetal].GetColor ());
							LiquidShader.GetInstance ().UseTexture ();
							LiquidShader.GetInstance ().AutoPan ();
							LiquidShader.GetInstance ().SetEmission (Util.DefaultEmission);
						}
						fall.Draw ();
					}
				}
			}
			if (game.TooltipHelper.GetOwner () == this)
				game.TooltipHelper.RenderNormalDialog (Input.OrthoMouseX, Input.OrthoMouseY, Util.White60);
		}

		#region EntityEventListener implementation

		public void InteractionPerformed (InteractiveEntity entity, object source)
		{
			Inventory playerInv = game.GameStats.PlayerInventory;
			if (game.GameStats.CastFilling > 0) {
				if (game.GameStats.CastingTemprature > 25)
					return; //The cast is too hot to take out of the cast
				if (game.GameStats.CastFilling >= 1) {
					Item i = game.GameStats.CurrentCast.CreateItem (game, game.GameStats.CastMetal);
					if (i == null)
						return;
					if (playerInv.CanFitItem (i)) {
						playerInv.AddItem (i);
						game.GameStats.CastFilling = 0;
					}
					else
						playerInv.InventoryTooFull (i);
				}
			} else {
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
		}

		#endregion
	}
}