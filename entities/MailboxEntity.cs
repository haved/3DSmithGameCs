using System;
using OpenTK;

namespace DSmithGameCs
{
	public class MailboxEntity : InteractiveEntity, IEntityEventListener
	{
		readonly Smith2DGame game;
		readonly Mesh box, lid;
		readonly Matrix4 lidOffset;

		float lidRotation = 0;
		float lidSpeed = 0;
		public MailboxEntity (Smith2DGame game, Mesh box, Mesh lid, Matrix4 lidOffset, float x, float y, float z, float xSize, float ySize) : base(null,x,y,z,xSize,ySize)
		{
			this.game = game;
			this.box = box;
			this.lid = lid;
			this.lidOffset = lidOffset;
			EventHandler = this;
		}

		public override void DisposeEntity()
		{
			box.Dispose ();
			lid.Dispose ();
		}

		public override void Update(Scene s)
		{
			lidRotation += lidSpeed * Time.Delta ()*3;
			if (lidRotation >= 0) {
				lidRotation = 0;
				lidSpeed = 0;
			} else
				lidSpeed += Time.Delta ()*2;
		}

		public override void Render(Scene s, Matrix4 VP)
		{
			Matrix4 MVP = Modelspace * VP;
			BasicShader.Instance.Bind ();
			BasicShader.Instance.ResetColor ();
			BasicShader.Instance.SetModelspaceMatrix (Modelspace);
			BasicShader.Instance.SetMVP (MVP);
			box.Draw ();
			Matrix4 lidMatrix = Matrix4.CreateRotationX (lidRotation) * lidOffset;
			BasicShader.Instance.SetModelspaceMatrix (lidMatrix*Modelspace);
			BasicShader.Instance.SetMVP (lidMatrix*MVP);
			lid.Draw ();
		}

		#region EntityEventListener implementation

		public void InteractionPerformed (InteractiveEntity entity, object source)
		{
			if (lidRotation >= 0)
				lidSpeed = -1.6f;
		}

		#endregion
	}
}

