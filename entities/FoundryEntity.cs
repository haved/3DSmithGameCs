using System;
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
				game.GameStats.FoundryTemprature += Time.Delta () * game.GameStats.CoalPercent;
				game.GameStats.FoundryTemprature = game.GameStats.FoundryTemprature > wantedTemprature ? wantedTemprature : game.GameStats.FoundryTemprature;
			} else if (game.GameStats.FoundryTemprature > wantedTemprature)
				game.GameStats.FoundryTemprature -= (game.GameStats.FoundryTemprature) * Time.Delta ();

			for (int i = 0; i < game.GameStats.FoundryIngots.Capacity; i++) {
				IngotItem ingot = game.GameStats.FoundryIngots [i];
				if (ingot == null)
					continue;
				game.GameStats.FoundryAlloy.AddMetal (ingot.GetMetal (), ingot.Melt (game.GameStats.FoundryTemprature));
				if (ingot.GetSolidProgress () <= 0)
					game.GameStats.FoundryIngots.RemoveAt (i);
			}
		}

		public override void Render(Scene s, Matrix4 VP)
		{
			BasicShader.GetInstance ().SetModelspaceMatrix(modelspace);
			BasicShader.GetInstance ().SetMVP(modelspace * VP);
			Draw (s);
			for (int i = 0; i < game.GameStats.FoundryIngots.Capacity; i++)
				if (game.GameStats.FoundryIngots [i] != null && game.GameStats.FoundryIngots [i].GetSolidProgress () > 0.2f)
					game.GameStats.FoundryIngots [i].RenderMesh (Matrix4.CreateScale (1, 1, game.GameStats.FoundryIngots [i].GetSolidProgress ()) * IngotMatrices [i] * modelspace, VP);

			if (game.GameStats.FoundryAlloy.GetAmount () < 0.01f)
				return;
			
			Matrix4 m = Matrix4.CreateScale (1, 1, game.GameStats.FoundryAlloy.GetAmount ()) * liquidTransform * modelspace;
			BasicShader.GetInstance ().SetModelspaceMatrix (m);
			BasicShader.GetInstance ().SetMVP (m*VP);
			BasicShader.GetInstance ().SetColor (game.GameStats.FoundryAlloy.GetColor ());
			molten.Draw ();
			BasicShader.GetInstance ().ResetColor ();
		}

		#region EntityEventListener implementation

		public void InteractionPerformed (InteractiveEntity entity, object source)
		{
			if (game.GameStats.PlayerInventory.HasSelectedItem ()) {
				var ingotItem = game.GameStats.PlayerInventory.GetSelectedItem () as IngotItem;
				if(ingotItem != null)
				if (game.GameStats.FoundryIngots.Count < game.GameStats.FoundryIngots.Capacity) {
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

