using System;
using OpenTK;

namespace DSmithGameCs
{
	public class IngotItem : Item
	{
		readonly IngotItemType type;

		public IngotItem (IngotItemType type)
		{
			this.type = type;
		}

		protected static readonly Matrix4 ItemMatrix = Matrix4.CreateRotationX(-1f)*Matrix4.CreateRotationY(0.6f)*Matrix4.CreateRotationZ(0.4f)*Matrix4.CreateTranslation(0.1f, 0, -2)
			*Matrix4.CreatePerspectiveFieldOfView(0.6f, 16f/9, 0.1f, 4);
		public override void RenderItem(float x, float y, float width, float height)
		{
			OrthoRenderEngine.DrawColoredMesh (type.GetSolidMesh(), ItemMatrix, type.GetColor(), x+4, y+4, width-8, height-8, -1, -2, 2, 4);
		}

		public void RenderMesh(Matrix4 location, Matrix4 VP)
		{
			BasicShader.GetInstance ().SetColor (type.GetColor ());
			BasicShader.GetInstance ().SetModelspaceMatrix (location);
			BasicShader.GetInstance ().SetMVP (location*VP);
			type.GetSolidMesh ().Draw();
			BasicShader.GetInstance ().ResetColor ();
		}

		public override uint GetSize()
		{
			return 1;
		}
	}
}

