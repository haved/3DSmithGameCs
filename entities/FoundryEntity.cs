using System;
using OpenTK;

namespace DSmithGameCs
{
	public class FoundryEntity : InteractiveEntity, EntityEventListener
	{
		readonly Smith2DGame game;
		readonly Matrix4[] IngotMatrices;
		public FoundryEntity (Smith2DGame game, Mesh m, float x, float y, Matrix4[] ingotMatrices, float xSize, float ySize): base(m, x, y, 0, xSize, ySize)
		{
			this.game = game;
			this.EventHandler = this;
			this.IngotMatrices = ingotMatrices;
		}

		public override void Render(Scene s, Matrix4 VP)
		{
			BasicShader.GetInstance ().SetModelspaceMatrix(modelspace);
			BasicShader.GetInstance ().SetMVP(modelspace * VP);
			Draw (s);
			for(int i = 0; i < game.GameStats.FoundryContents.Length; i++)
				if (game.GameStats.FoundryContents[i] != null)
					game.GameStats.FoundryContents [i].RenderMesh (IngotMatrices[i]*modelspace, VP);
		}

		#region EntityEventListener implementation

		public void InteractionPerformed (InteractiveEntity entity, object source)
		{
			if (game.GameStats.PlayerInventory.HasSelectedItem ()) {
				var ingotItem = game.GameStats.PlayerInventory.GetSelectedItem () as IngotItem;
				if(ingotItem != null)
					for (int i = 0; i < game.GameStats.FoundryContents.Length; i++) {
						if (game.GameStats.FoundryContents [i] == null) {
							game.GameStats.FoundryContents [i] = ingotItem;
							game.GameStats.PlayerInventory.RemoveItem (game.GameStats.PlayerInventory.GetSelectedItemIndex ());
							break;
						}
					}
			}
		}

		#endregion
	}

	public static class FoundryMeshInfo
	{
		public const uint IngotAmount = 4;

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

