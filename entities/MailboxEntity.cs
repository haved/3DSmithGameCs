﻿using System;
using OpenTK;

namespace DSmithGameCs
{
	public class MailboxEntity : MeshEntity, IInteractiveEntity
	{
		readonly Smith2DGame game;
		readonly Mesh box, lid;
		readonly Matrix4 lidOffset;

		float lidRotation = 0;
		float lidSpeed = 0;
		public MailboxEntity (Smith2DGame game, Mesh box, Mesh lid, Matrix4 lidOffset, float x, float y, float z, float xRot, float yRot, float zRot, float xSize, float ySize) : base(null,x,y,z,xRot,yRot,zRot,xSize,ySize)
		{
			this.game = game;
			this.box = box;
			this.lid = lid;
			this.lidOffset = lidOffset;
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

		Matrix4 lidModelMatrix;
		public override void PreRender(Scene s, Matrix4 VP)
		{
			base.PreRender (s, VP);
			Matrix4 lidMatrix = Matrix4.CreateRotationX (lidRotation) * lidOffset;
			lidModelMatrix = lidMatrix * Modelspace;
		}

		public override void Render(Scene s, Matrix4 VP, INormalShader shader)
		{
			shader.SetModelspaceMatrix (Modelspace);
			shader.SetMVP (Modelspace * VP);
			box.Draw ();
			shader.SetModelspaceMatrix (lidModelMatrix);
			shader.SetMVP (lidModelMatrix * VP);
			lid.Draw ();
		}

		#region EntityEventListener implementation

		public void InteractionPerformed (object source)
		{
			if (lidRotation >= 0)
				lidSpeed = -1.4f;
		}

		#endregion
	}
}

