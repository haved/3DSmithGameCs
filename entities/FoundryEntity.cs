using System;
using System.Drawing;
using OpenTK;

namespace DSmithGameCs
{
	public class FoundryEntity : InteractiveEntity, IEntityEventListener
	{
		readonly Smith2DGame game;
		readonly Matrix4[] IngotMatrices;
		readonly Mesh molten;
		readonly Matrix4 liquidTransform;
		public FoundryEntity (Smith2DGame game, Mesh m, Mesh molten, Matrix4 liquidTransform, float x, float y, Matrix4[] ingotMatrices, float xSize, float ySize): base(m, x, y, 0, xSize, ySize)
		{
			this.game = game;
			this.molten = molten;
			this.liquidTransform = liquidTransform;
			this.EventHandler = this;
			this.IngotMatrices = ingotMatrices;
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
				game.GameStats.FoundryAlloy.AddMetal (ingot.GetMetal (), ingot.Melt (game.GameStats.FoundryTemprature));
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
					game.TooltipHelper.Writer.Clear ();
					game.TooltipHelper.Writer.DrawString ("Heat: " + (int)game.GameStats.FoundryTemprature + "°C", 0, 0, Color.Red);
					game.TooltipHelper.Writer.DrawString ("Coal: " + (int)game.GameStats.CoalPercent + "%", 0, 20, Color.White);
					game.TooltipHelper.Writer.DrawString ("Oxygen: " + (int)game.GameStats.AirQuality + "%", 0, 40, Color.Aqua);
					game.TooltipHelper.Writer.DrawString ("Ingots:", 0, 65, Color.Green);
					int y = 85;
					for (int i = 0; i < game.GameStats.FoundryAlloy.MetalTypeAmount; i++)
					{
						IMetal m = game.GameStats.FoundryAlloy [i];
						game.TooltipHelper.Writer.DrawString ((int)(game.GameStats.FoundryAlloy.GetMetalAmount(i)*100+.5f)/100f+ " " + m.GetName() + " (molten)", 10, y, Util.GetColorFromVector(m.GetColor()));
						y += 20;
					}
					for (int i = 0; i < game.GameStats.FoundryIngots.Capacity; i++) {
						IngotItem item = game.GameStats.FoundryIngots[i];
						if (item == null)
							continue;
						game.TooltipHelper.Writer.DrawString ((int)(item.GetSolidProgress()*100+.5f)/100f+ " " + item.GetMetal().GetName() + " (solid)", 10, y, Util.GetColorFromVector(item.GetMetal().GetColor()));
						y += 20;
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
			BasicShader.GetInstance ().ResetColor ();
			BasicShader.GetInstance ().SetModelspaceMatrix(Modelspace);
			BasicShader.GetInstance ().SetMVP(Modelspace * VP);
			Draw (s);
			for (int i = 0; i < game.GameStats.FoundryIngots.Capacity; i++)
				if (game.GameStats.FoundryIngots [i] != null && game.GameStats.FoundryIngots [i].GetSolidProgress () > 0.2f)
					game.GameStats.FoundryIngots [i].RenderMesh (Matrix4.CreateScale (1, 1, game.GameStats.FoundryIngots [i].GetSolidProgress ()) * IngotMatrices [i] * Modelspace, VP);

			if (game.GameStats.FoundryAlloy.GetAmount () >= 0.01f) {
				Matrix4 m = Matrix4.CreateScale (1, 1, game.GameStats.FoundryAlloy.GetAmount ()) * liquidTransform * Modelspace;
				INormalShader shader;
				if (game.GameStats.FoundryTemprature > game.GameStats.FoundryAlloy.GetMeltingPoint ()) {
					shader = LiquidShader.GetInstance ();
					shader.Bind ();
					LiquidShader.GetInstance ().UseTexture ();
					LiquidShader.GetInstance ().AutoPan ();
					LiquidShader.GetInstance ().SetEmission (Util.DefaultEmission);
				} else
					shader = BasicShader.GetInstance ();

				shader.SetModelspaceMatrix (m);
				shader.SetMVP (m * VP);
				shader.SetColor (game.GameStats.FoundryAlloy.GetColor ());
				molten.Draw ();
			}

			if (game.TooltipHelper.GetOwner () == this)
				game.TooltipHelper.RenderNormalDialog (Input.OrthoMouseX, Input.OrthoMouseY, Util.White60);
		}

		#region EntityEventListener implementation

		public void InteractionPerformed (InteractiveEntity entity, object source)
		{
			if (game.GameStats.PlayerInventory.HasSelectedItem ()) {
				var ingotItem = game.GameStats.PlayerInventory.GetSelectedItem () as IngotItem;
				if(ingotItem != null)
				if (game.GameStats.FoundryIngots.Count+game.GameStats.FoundryAlloy.GetAmount() + 1 <= game.GameStats.FoundryIngots.Capacity) {
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
			Matrix4 ingotMatrix = Matrix4.CreateRotationZ((float)Math.PI/2)*Matrix4.CreateScale (1.4f);
			Matrix4[] output = new Matrix4[IngotAmount];
			output [0] = ingotMatrix*Matrix4.CreateTranslation (2.45f, 1.26f, 3);
			output [1] = ingotMatrix*Matrix4.CreateTranslation (1.65f, -1.65f, 3);
			output [2] = ingotMatrix*Matrix4.CreateTranslation (0.44f, 1.26f, 3);
			output [3] = ingotMatrix*Matrix4.CreateTranslation (-0.83f, -1.65f, 3);
			return output;
		}
	}
}

