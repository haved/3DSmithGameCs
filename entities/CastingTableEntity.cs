using System;
using System.Drawing;
using OpenTK;

namespace DSmithGameCs
{
	public class CastingTableEntity : MeshEntity, IInteractiveEntity
	{
		readonly Smith2DGame game;

		readonly float tableWidth;
		readonly float tableHeight;

		readonly Mesh fill;

		Matrix4 castModelspace;
		Matrix4 fillMatrix;

		public CastingTableEntity (Smith2DGame game, Mesh m, Mesh fill, float x, float y, float tableWidth, float tableHeight, float xSize, float ySize) : base(m, x, y, 0, xSize, ySize)
		{
			this.game = game;
			this.tableWidth = tableWidth;
			this.tableHeight = tableHeight;
			this.fill = fill;
		}

		public override void DisposeEntity()
		{
			base.DisposeEntity ();
			fill.Dispose ();
		}

		short prevCastItem = -1;
		public void UpdateCastMatrix()
		{
			if (game.GameStats.CurrentCast.CastInfoID != prevCastItem) {
				prevCastItem = game.GameStats.CurrentCast.CastInfoID;
				float width = game.GameStats.CurrentCast.Info.Width;
				float pos = (tableWidth - width) / 2;
				castModelspace = Matrix4.CreateTranslation (pos, 0, tableHeight);
				fillMatrix = Matrix4.CreateScale(width, 1, 1) * Matrix4.CreateTranslation (pos, 0, tableHeight - 0.08f);
			}
		}

		public override void Update(Scene s)
		{
			UpdateDialog ();
			if (Input.PourKeyPressed & game.TooltipHelper.GetOwner () == this) {
				if (game.GameStats.CurrentCast != null & game.GameStats.FoundryAlloy != null && game.GameStats.FoundryAlloy.Amount >= game.GameStats.CurrentCast.GetVolume () - 0.005f &
																	game.GameStats.CastFilling <= 0 & game.GameStats.FoundryTemprature > game.GameStats.FoundryAlloy.GetMeltingPoint()) {
					Alloy input = game.GameStats.FoundryAlloy;

					float bestPurity = 0;
					KnownMetal bestMetal = null;

					foreach(KnownMetal metal in KnownMetal.Metals)
					{
						if (metal == null)
							continue;
						float newPurity = metal.GetPurityFrom (input);
						if (newPurity > metal.GetMinimumPurity() & newPurity > bestPurity) {
							bestPurity = newPurity;
							bestMetal = metal;
						}
					}

					if (bestMetal != null) {
						game.GameStats.CastMetal = bestMetal.Id;
						game.GameStats.CastMetalPurity = bestPurity;

						game.GameStats.CastFilling = 0.01f;
						game.GameStats.OldFoundryAmount = game.GameStats.FoundryAlloy.Amount;
						game.GameStats.CastingTemprature = game.GameStats.FoundryTemprature;
					} else {
						game.ErrortipHelper.ShowError (Localization.GetLocalization("ui.error.notanalloy"), 200, 500, 2, false);
					}
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
						else if (game.GameStats.FoundryAlloy.Amount >= game.GameStats.CurrentCast.GetVolume ()-0.005f) {
							y += 20;
							if (game.GameStats.FoundryTemprature < game.GameStats.FoundryAlloy.GetMeltingPoint ()) {
								game.TooltipHelper.Writer.DrawString ("The foundry is too cold", 0, 40, Color.Red);
							} else {
								game.TooltipHelper.Writer.DrawString ("Hit " + Input.GetKeyName (Input.POURKEY) + " to pour", 0, 40, Color.Red);
								game.TooltipHelper.Writer.DrawString ("Foundry contents:", 0, 60, Color.Green);
								y += 20;
								for (int i = 0; i < game.GameStats.FoundryAlloy.MetalCount; i++) {
									KnownMetal m = game.GameStats.FoundryAlloy [i];
									game.TooltipHelper.Writer.DrawString ((int)(game.GameStats.FoundryAlloy.GetMetalFraction (i) * 100 + .5f) / 100f + " " + m.Name + " (molten)", 10, y, Util.GetColorFromVector (m.Color));
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

		public override void Render(Scene s, Matrix4 VP, INormalShader shader)
		{
			shader.SetModelspaceMatrix(Modelspace);
			shader.SetMVP(Modelspace * VP);
			Draw (s);
			if (game.GameStats.CurrentCast != null) {
				UpdateCastMatrix ();
				shader.SetModelspaceMatrix(castModelspace*Modelspace);
				shader.SetMVP(castModelspace * Modelspace * VP);
				shader.SetColor (game.GameStats.CurrentCast.GetColor());
				game.GameStats.CurrentCast.GetMesh ().Draw ();
				shader.ResetColor ();
			}
		}

		public override void PostRender(Scene s, Matrix4 VP)
		{
			if (game.GameStats.CastFilling > 0 && game.GameStats.CurrentCast != null && game.GameStats.CastMetal > -1) {
				UpdateCastMatrix ();
				Matrix4 fillModelspace = Matrix4.CreateScale (1, 1, game.GameStats.CastFilling * game.GameStats.CurrentCast.FillHeight)*fillMatrix*Modelspace;

				ISimpleShader shader;
				if (game.GameStats.CastingTemprature > 25) {
					shader = LiquidShader.Instance;
					shader.Bind ();
					LiquidShader.Instance.UseTexture ();
					LiquidShader.Instance.AutoPan ();
					LiquidShader.Instance.SetEmission (Util.DefaultEmission);
				} else {
					shader = BasicShader.Instance;
					shader.Bind ();
				}

				shader.SetModelspaceMatrix(fillModelspace);
				shader.SetMVP (fillModelspace*VP);
				shader.SetColor (KnownMetal.GetColor(game.GameStats.CastMetal));
				fill.Draw ();
			}

			if (game.TooltipHelper.GetOwner () == this)
				game.TooltipHelper.RenderNormalDialog (Input.OrthoMouseX, Input.OrthoMouseY, Util.White60);
		}

		#region EntityEventListener implementation

		public void InteractionPerformed (object source)
		{
			Inventory playerInv = game.GameStats.PlayerInventory;
			if (game.GameStats.CastFilling > 0) {
				if (game.GameStats.CastingTemprature > 25)
					return; //The cast is too hot to take out of the cast
				if (game.GameStats.CastFilling >= 1) {
					Item i = game.GameStats.CurrentCast.CreateItem (game, game.GameStats.CastMetal, game.GameStats.CastMetalPurity);
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
						UpdateCastMatrix ();
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