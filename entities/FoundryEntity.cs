﻿using System;
using System.Drawing;
using OpenTK;

namespace DSmithGameCs
{
	public class FoundryEntity : MeshEntity, IInteractiveEntity
	{
		readonly PointLight light = new PointLight(new Vector3(1.2f, 0.4f, 0), Vector3.Zero, 8, 20, 0.1f, 0.5f, 1.4f);

		readonly Smith2DGame game;
		readonly Matrix4[] IngotMatrices;
		readonly Mesh molten;
		readonly Matrix4 liquidTransform;
		readonly Vector3 lightPos;
		public FoundryEntity (Smith2DGame game, Mesh m, Mesh molten, Matrix4 liquidTransform, Vector3 lightPos, float x, float y, Matrix4[] ingotMatrices, float xSize, float ySize): base(m, x, y, 0, xSize, ySize)
		{
			this.game = game;
			this.molten = molten;
			this.liquidTransform = liquidTransform;
			this.lightPos = lightPos;
			IngotMatrices = ingotMatrices;
		}

		public override void DisposeEntity()
		{
			base.DisposeEntity ();
			molten.Dispose ();
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

		const float foundryHeatingSpeed = 1f;//0.4f;
		const float maxTemp = 1927; //The temprature of coal under perfect air conditions
		public override void Update(Scene s)
		{
			float wantedTemprature = game.GameStats.AirQuality / 100 * maxTemp;
			if (game.GameStats.CoalPercent < 10)
				wantedTemprature *= game.GameStats.CoalPercent / 10;
			game.GameStats.CoalPercent -= Time.Delta ()/4;
			float lowestPossibleAirQuality = (100 - game.GameStats.CoalPercent) / 6 + 15;
			game.GameStats.AirQuality -= game.GameStats.AirQuality /10 * Time.Delta ();
				game.GameStats.AirQuality = game.GameStats.AirQuality < lowestPossibleAirQuality ? lowestPossibleAirQuality : game.GameStats.AirQuality;

			if (game.GameStats.FoundryTemprature < wantedTemprature) {
				game.GameStats.FoundryTemprature += Time.Delta () * game.GameStats.CoalPercent * foundryHeatingSpeed;
				game.GameStats.FoundryTemprature = game.GameStats.FoundryTemprature > wantedTemprature ? wantedTemprature : game.GameStats.FoundryTemprature;
			} else if (game.GameStats.FoundryTemprature > wantedTemprature)
				game.GameStats.FoundryTemprature -= (game.GameStats.FoundryTemprature) * Time.Delta ()*0.2f;

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

		int prevVal;
		void UpdateDialog()
		{
			if (game.Player.IsLookingAt (this)) {
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
				game.TooltipHelper.UnClaim ();
		}

		public override void Render(Scene s, Matrix4 VP, INormalShader shader)
		{
			base.Render (s, VP, shader);
			for (int i = 0; i < game.GameStats.FoundryIngots.Capacity; i++)
				if (game.GameStats.FoundryIngots [i] != null && game.GameStats.FoundryIngots [i].GetSolidProgress () > 0.2f)
					game.GameStats.FoundryIngots [i].RenderMesh (Matrix4.CreateScale (1, 1, game.GameStats.FoundryIngots [i].GetSolidProgress ()) * IngotMatrices [i] * Modelspace, VP, shader);
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

