using OpenTK;

namespace DSmithGameCs
{
	public class DoorEntity : MeshEntity, IInteractiveEntity
	{
		readonly Smith2DGame game;
		readonly Mesh door;
		readonly Matrix4 doorTranslation;

		public DoorEntity (Smith2DGame game, Mesh doorFrame, Mesh door, Matrix4 doorTranslation, float x, float y, float z, float xRot, float yRot, float zRot, float xSize, float ySize) : base(doorFrame, x, y, z, xRot, yRot, zRot, xSize, ySize)
		{
			this.game = game;
			this.door = door;
			this.doorTranslation = doorTranslation;
		}

		public override void DisposeEntity()
		{
			base.DisposeEntity ();
			door.Dispose ();
		}

		float doorRotation = 0;
		float doorSpeed = 0;
		public override void Update (Scene s)
		{
			doorRotation += doorSpeed * Time.Delta ()*3;
			if (doorRotation >= 0) {
				doorRotation = 0;
				doorSpeed = 0;
			} else
				doorSpeed += Time.Delta ()*2;
		}

		Matrix4 doorModelspace;
		public override void PreRender(Scene s, Matrix4 VP)
		{
			doorModelspace = Matrix4.CreateRotationZ(doorRotation) * doorTranslation * Modelspace;
		}

		public override void Render (Scene s, Matrix4 VP, INormalShader shader)
		{
			base.Render (s, VP, shader);

			shader.SetModelspaceMatrix (doorModelspace);
			shader.SetMVP (doorModelspace*VP);
			door.Draw ();
		}

		public void InteractionPerformed (object source)
		{
			if (doorRotation >= 0)
				doorSpeed = -1.6f;
		}

		public override bool IsInField(Vector2 point)
		{
			return ((point - Pos.Xy) * new Vector2 (1 / XSize, 0.1f / YSize)).Length < 1.4f;
		}
	}
}

