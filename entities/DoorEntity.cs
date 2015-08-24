using System;
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

		public override void Render (Scene s, Matrix4 VP, INormalShader shader)
		{
			base.Render (s, VP, shader);
			door.Draw ();
		}

		public void InteractionPerformed (object source)
		{
			//TODO: open door.
		}
	}
}

