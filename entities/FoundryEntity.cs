using System;
using System.Drawing;
using System.Collections.Generic;
using OpenTK;

namespace DSmithGameCs
{
	public class FoundryEntity : MeshEntity, IInteractiveEntity
	{
		const float heatupSpeed = 1;
		const float cooldownSpeed = 0.1f;
		const float maxTemp = 1927;
		const float neutralSpace = 15; //Hopw much higher than the wanted temperature the actual temperature has to be for the temperature to change

		readonly PointLight light = new PointLight(new Vector3(1.2f, 0.4f, 0), Vector3.Zero, 8, 20, 0.1f, 0.5f, 1.4f);

		readonly Smith2DGame game;
		readonly Matrix4[] IngotMatrices;
		readonly Mesh molten;
		readonly Mesh fall;
		readonly Matrix4 liquidTransform;
		readonly Vector3 lightPos;
		public FoundryEntity (Smith2DGame game, Mesh m, Mesh molten, Matrix4 liquidTransform, Mesh fall, Vector3 lightPos, float x, float y, Matrix4[] ingotMatrices, float xSize, float ySize): base(m, x, y, 0, xSize, ySize)
		{
			this.game = game;
			this.molten = molten;
			this.liquidTransform = liquidTransform;
			this.fall = fall;
			this.lightPos = lightPos;
			IngotMatrices = ingotMatrices;
		}

		public override void DisposeEntity()
		{
			base.DisposeEntity ();
			molten.Dispose ();
			fall.Dispose ();
			light.Dispose ();
		}

		public override void UpdateModelspaceMatrix ()
		{
			base.UpdateModelspaceMatrix ();
			light.Position = Pos + lightPos;
		}

		public override void OnAddedToScene (Scene s)
		{
			base.OnAddedToScene (s);
			s.AddLight (light);
		}

		public override void OnRemovedFromScene (Scene s)
		{
			base.OnRemovedFromScene (s);
			s.RemoveLight (light);
		}

		public override void Update(Scene s)
		{
			float wantedTemprature = game.GameStats.AirQuality / 100 * maxTemp;
			if (game.GameStats.CoalPercent < 10)
				wantedTemprature *= game.GameStats.CoalPercent / 10;
			game.GameStats.CoalPercent -= Time.Delta ()/4;
			float lowestPossibleAirQuality = (100 - game.GameStats.CoalPercent) / 6 + 15;
			game.GameStats.AirQuality -= game.GameStats.AirQuality /10 * Time.Delta ();
				game.GameStats.AirQuality = game.GameStats.AirQuality < lowestPossibleAirQuality ? lowestPossibleAirQuality : game.GameStats.AirQuality;

			/*if (game.GameStats.FoundryTemprature < wantedTemprature) {
				game.GameStats.FoundryTemprature += Time.Delta () * game.GameStats.CoalPercent * heatupSpeed;
				game.GameStats.FoundryTemprature = game.GameStats.FoundryTemprature > wantedTemprature ? wantedTemprature : game.GameStats.FoundryTemprature;
			} else if (game.GameStats.FoundryTemprature > wantedTemprature+neutralSpace)
				game.GameStats.FoundryTemprature -= (game.GameStats.FoundryTemprature) * Time.Delta ()*cooldownSpeed;*/

			game.GameStats.FoundryTemprature -= (game.GameStats.FoundryTemprature-wantedTemprature) * Time.Delta() * (game.GameStats.FoundryTemprature < wantedTemprature ? heatupSpeed : cooldownSpeed);

			for (int i = 0; i < game.GameStats.FoundryIngots.Capacity; i++) {
				IngotItem ingot = game.GameStats.FoundryIngots [i];
				if (ingot == null)
					continue;
				game.GameStats.FoundryAlloy.AddMetal (ingot.MetalID, ingot.Purity, ingot.Melt (game.GameStats.FoundryTemprature));
				if (ingot.GetSolidProgress () <= 0)
					game.GameStats.FoundryIngots.RemoveAt (i);
			}

			UpdateDialog ();
		}

		float prevWidth=0;
		void UpdateDialog()
		{
			if (game.Player.IsLookingAt (this)) {
				game.TooltipHelper.ClaimIfPossible (this);
				if (game.TooltipHelper.GetOwner () == this) {
					TextWriter writer = game.TooltipHelper.Writer;
					int lineCount = 0;
					var lineHeight = writer.GetLineHeight ();
					float maxWidth=0;

					writer.Clear ();

					maxWidth = Math.Max(maxWidth, game.TooltipHelper.Writer.GetWidthDrawString (Localization.GetLocalization("ui.tooltip.heat:"), 0, 0, Color.Red) + 
						game.TooltipHelper.Writer.GetWidthDrawStringEnd ((int)game.GameStats.FoundryTemprature + "*C", prevWidth, 0, Color.White));
					maxWidth = Math.Max(maxWidth, game.TooltipHelper.Writer.GetWidthDrawString (Localization.GetLocalization("ui.tooltip.coal:"), 0, lineHeight, Color.Black) + 
						game.TooltipHelper.Writer.GetWidthDrawStringEnd ((int)game.GameStats.CoalPercent + "%", prevWidth, lineHeight, Color.White));
					maxWidth = Math.Max(maxWidth, game.TooltipHelper.Writer.GetWidthDrawString (Localization.GetLocalization("ui.tooltip.oxygen:"), 0, lineHeight*2, Color.Aqua) + 
						game.TooltipHelper.Writer.GetWidthDrawStringEnd ((int)game.GameStats.AirQuality + "%", prevWidth, lineHeight*2, Color.White));

					lineCount += 3;

					writer.Resize ((int) maxWidth, (int)(lineCount*lineHeight));

					prevWidth = maxWidth;
				}
			} else if (game.TooltipHelper.GetOwner () == this)
				game.TooltipHelper.UnClaim ();

			/*if (game.Player.IsLookingAt (this)) {
				if (game.TooltipHelper.ClaimIfPossible (this)) {
					game.TooltipHelper.Writer.Resize (300, 20 * 16 + 10);
					prevVal = -1;
				}
				if(game.TooltipHelper.GetOwner()==this && prevVal != (prevVal=(int)(game.GameStats.FoundryTemprature+game.GameStats.CoalPercent+game.GameStats.AirQuality)))
				{
					float lineSpacing = game.TooltipHelper.Writer.GetLineHeight ();
					game.TooltipHelper.Writer.Clear ();
					game.TooltipHelper.Writer.DrawString ("Heat: " + (int)game.GameStats.FoundryTemprature + "°C", 0, 0, Color.Red);
					game.TooltipHelper.Writer.DrawString ("Coal: " + (int)game.GameStats.CoalPercent + "%", 0, lineSpacing, Color.White);
					game.TooltipHelper.Writer.DrawString ("Oxygen: " + (int)game.GameStats.AirQuality + "%", 0, lineSpacing*2, Color.Aqua);
					game.TooltipHelper.Writer.DrawString ("Ingots:", 0, lineSpacing*3.5f, Color.Green);
					float y = lineSpacing*4.5f;
					for (int i = 0; i < game.GameStats.FoundryAlloy.MetalCount; i++)
					{
						KnownMetal m = game.GameStats.FoundryAlloy [i];
						game.TooltipHelper.Writer.DrawString ((int)(game.GameStats.FoundryAlloy.GetMetalAmount(i)*100+.5f)/100f+ " " + m.Name + " (molten)", 10, y, Util.GetColorFromVector(m.Color));
						y += lineSpacing;
					}
					for (int i = 0; i < game.GameStats.FoundryIngots.Capacity; i++) {
						IngotItem item = game.GameStats.FoundryIngots[i];
						if (item == null)
							continue;
						game.TooltipHelper.Writer.DrawString ((int)(item.GetSolidProgress()*100+.5f)/100f+ " " + item.Metal.Name + " (solid)", 10, y, Util.GetColorFromVector(item.Metal.Color));
						y += lineSpacing;
					}
					if (game.GameStats.FoundryAlloy.Amount > 0) {
						game.TooltipHelper.Writer.DrawString (Localization.GetLocalization ("ui.tooltip.purity:") + (int)(game.GameStats.FoundryAlloy.Purity * 100+0.5f) + "%", 10, y, Util.GetColorFromVector (game.GameStats.FoundryAlloy.GetColor ()));
						y += lineSpacing;
					}

					if ((int)y+10 != game.TooltipHelper.Writer.Height)
						game.TooltipHelper.Writer.Resize (300, (int)y+10);
						
				}
			} else if (game.TooltipHelper.GetOwner () == this)
				game.TooltipHelper.UnClaim ();*/
		}

		public override void Render(Scene s, Matrix4 VP, INormalShader shader)
		{
			base.Render (s, VP, shader);
			for (int i = 0; i < game.GameStats.FoundryIngots.Capacity; i++)
				if (game.GameStats.FoundryIngots [i] != null && game.GameStats.FoundryIngots [i].GetSolidProgress () > 0.2f)
					game.GameStats.FoundryIngots [i].RenderMesh (Matrix4.CreateScale (1, 1, game.GameStats.FoundryIngots [i].GetSolidProgress ()) * IngotMatrices [i] * Modelspace, VP, shader);
			if (game.GameStats.FoundryIngots.Count > 0)
				;//shader.ResetColor (); //Reset the color afterwards
		}

		public override void PostRender(Scene s, Matrix4 VP)
		{
			if (game.GameStats.FoundryAlloy.Amount >= 0.01f) {
				Matrix4 m = Matrix4.CreateScale (1, 1, game.GameStats.FoundryAlloy.Amount) * liquidTransform * Modelspace;
				ISimpleShader shader;
				if (game.GameStats.FoundryTemprature > game.GameStats.FoundryAlloy.GetMeltingPoint ()) {
					shader = LiquidShader.Instance;
					shader.Bind ();
					LiquidShader.Instance.UseTexture ();
					LiquidShader.Instance.AutoPan ();
					LiquidShader.Instance.SetEmission (Util.DefaultEmission);
				} else {
					shader = BasicShader.Instance;
					shader.Bind ();
				}

				shader.SetModelspaceMatrix (m);
				shader.SetMVP (m * VP);
				shader.SetColor (game.GameStats.FoundryAlloy.GetColor ());
				molten.Draw ();
			}

			if (game.GameStats.CastFilling > 0 & game.GameStats.CastFilling < 1 & game.GameStats.CastMetal > -1) {
				LiquidShader shader = LiquidShader.Instance;
				shader.Bind ();
				shader.SetModelspaceMatrix(Modelspace);
				shader.SetMVP (Modelspace*VP);
				shader.SetColor (KnownMetal.GetColor(game.GameStats.CastMetal));
				shader.UseTexture ();
				shader.AutoPan ();
				shader.SetEmission (Util.DefaultEmission);
				fall.Draw ();
			}

			if (game.TooltipHelper.GetOwner () == this)
				game.TooltipHelper.RenderNormalDialog (Input.OrthoMouseX, Input.OrthoMouseY, Util.White60);
		}

		#region EntityEventListener implementation

		public void InteractionPerformed (object source)
		{
			if (game.GameStats.PlayerInventory.HasSelectedItem ()) {
				var ingotItem = game.GameStats.PlayerInventory.GetSelectedItem () as IngotItem;
				if(ingotItem != null)
				if (game.GameStats.FoundryIngots.Count+game.GameStats.FoundryAlloy.Amount + ingotItem.GetSize() <= game.GameStats.FoundryIngots.Capacity+0.01f) {
					game.GameStats.FoundryIngots.Add(ingotItem);
					game.GameStats.PlayerInventory.RemoveItem (game.GameStats.PlayerInventory.GetSelectedItemIndex ());
				}
			}
		}

		#endregion
	}

	public static class FoundryMeshInfo
	{
		public const int IngotAmount = 4;

		public static Matrix4[] CreateIngotMatrices()
		{
			Matrix4 ingotMatrix = Matrix4.CreateScale (1.4f);
			var output = new Matrix4[IngotAmount];
			output [0] = ingotMatrix*Matrix4.CreateScale(0.98f)*Matrix4.CreateRotationZ(0.02f)*Matrix4.CreateTranslation (-1.5f, 2.25f, 2.5f);
			output [1] = ingotMatrix*Matrix4.CreateScale(1.02f)*Matrix4.CreateRotationZ(-0.01f)*Matrix4.CreateTranslation (-1.5f, 0.76f, 2.5f);
			output [2] = ingotMatrix*Matrix4.CreateScale(0.98f)*Matrix4.CreateRotationZ(0.03f)*Matrix4.CreateTranslation (-1.5f, -0.74f, 2.5f);
			output [3] = ingotMatrix*Matrix4.CreateScale(1.01f)*Matrix4.CreateRotationZ(-0.02f)*Matrix4.CreateTranslation (-1.5f, -2.26f, 2.5f);
			return output;
		}
	}
}

