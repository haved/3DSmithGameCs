using System;
using OpenTK;

namespace DSmithGameCs
{
	public class BellowEntity : InteractiveEntity, EntityEventListener
	{
		const float maxBellowSize = 1;
		const float minBellowSize = 0.3f;
		float bellowSize = maxBellowSize;
		float bellowSpeed = 0;

		readonly Smith2DGame game;
		readonly Matrix4 tip;

		public BellowEntity (Smith2DGame game, Mesh m, Matrix4 tip, float x, float y, float z, float xSize, float ySize) : base(m, x, y, z, xSize, ySize)
		{
			this.game = game;
			this.tip = tip;
			EventHandler = this;
		}

		public override void Update(Scene s)
		{
			bellowSize += bellowSpeed * Time.Delta ();
			if (bellowSize > maxBellowSize & bellowSpeed > 0)
				bellowSpeed -= bellowSpeed * Time.Delta () * 6;
			if (bellowSize < minBellowSize)
				bellowSpeed += Time.Delta () * 4;
		}

		public override void Render(Scene s, Matrix4 VP)
		{
			Matrix4 newModelspace = Matrix4.CreateScale (1, 1, bellowSize) * tip * modelspace;
			BasicShader.GetInstance ().SetModelspaceMatrix (modelspace);
			BasicShader.GetInstance ().SetMVP (newModelspace * VP);
			base.Draw (s);
		}

		#region EntityEventListener implementation

		public void InteractionPerformed (InteractiveEntity entity, object source)
		{
			if(bellowSize >= maxBellowSize) 
				bellowSpeed = -1;
		}

		#endregion
	}
}

