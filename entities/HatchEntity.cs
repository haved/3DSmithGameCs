using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class HatchEntity : InteractiveEntity, EntityEventListener
	{
		readonly Mesh hatchHole;
		readonly Mesh hatch;
		readonly Matrix4 hatchTranslation;
		float hatchRotation = 0;
		float hatchSpeed = 0;
		public HatchEntity (Mesh hatchHole, Mesh hatch, Vector3 hatchTranslation, float x, float y, float z, float xSize, float ySize) : base(null, hatchHole, x, y, z, xSize, ySize)
		{
			this.hatchHole = hatchHole;
			this.hatch = hatch;
			this.hatchTranslation = Matrix4.CreateTranslation(hatchTranslation);
			this.solid = false;
			this.eventHandler = this;
		}

		public override void Update(Scene s)
		{
			if (hatchRotation < 0 | hatchSpeed < 0) {
				hatchSpeed += Time.delta () * 10;
				hatchRotation += hatchSpeed * Time.delta ();
				if (hatchRotation > 0) {
					hatchRotation = 0;
					hatchSpeed = 0;
				}
			}
		}

		public override void Render(Scene s, Matrix4 VP)
		{
			Matrix4 MVP = modelspace * VP;
			BasicShader.GetInstance ().SetModelspaceMatrix (modelspace);
			BasicShader.GetInstance ().SetMVP (MVP);
			hatchHole.Draw ();
			Matrix4 hatchTransform = Matrix4.CreateRotationY (hatchRotation) * hatchTranslation;
			BasicShader.GetInstance ().SetModelspaceMatrix (hatchTransform*modelspace);
			BasicShader.GetInstance ().SetMVP (hatchTransform*MVP);
			hatch.Draw ();
		}

		public void InteractionPerformed(InteractiveEntity entity, object source)
		{
			if(hatchRotation >= 0)
				hatchSpeed = -6;
		}
	}
}

