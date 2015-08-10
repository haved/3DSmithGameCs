using System;
using System.Drawing;
using OpenTK;

namespace DSmithGameCs
{
	public class BigBellowEntity : ColliderEntity, IInteractiveEntity
	{
		readonly Smith2DGame game;
		readonly Mesh bellow;
		readonly Matrix4 bellowTransform;

		public BigBellowEntity (Smith2DGame game, Mesh bellow, Matrix4 bellowTransform, float x, float y, float z, float xSize, float ySize) : base(x, y, z, xSize, ySize)
		{
			this.game = game;
			this.bellow = bellow;
			this.bellowTransform = bellowTransform;
		}

		public override void DisposeEntity()
		{
			bellow.Dispose ();
		}

		const float maxBellowSize = 1;
		const float minBellowSize = 0.4f;
		float bellowSize = maxBellowSize;
		float bellowSpeed = 0;

		int prevAirQuality;
		public override void Update(Scene s)
		{
			bellowSize += bellowSpeed * Time.Delta ();
			if(bellowSpeed < 0)
				game.GameStats.AirQuality += Time.Delta () * 20;
			if (bellowSize > maxBellowSize & bellowSpeed > 0)
				bellowSpeed -= bellowSpeed * Time.Delta () * 6;
			if (bellowSize < minBellowSize)
				bellowSpeed += Time.Delta () * 4;

			if (game.Player.IsLookingAt (this)) {
				if (game.TooltipHelper.ClaimIfPossible (this) || (game.TooltipHelper.GetOwner () == this & prevAirQuality != (prevAirQuality=(int)game.GameStats.AirQuality))) {
					game.TooltipHelper.Writer.DrawStandardTooltip (new []{ Localization.GetLocalization( "ui.tooltip.oxygen:" ) }, new []{ Color.Aqua }, new []{prevAirQuality + "%" });
					prevAirQuality = -1;
				}
			} else if (game.TooltipHelper.GetOwner () == this)
				game.TooltipHelper.UnClaim ();
		}

		Matrix4 bellowModelspace;
		public override void PreRender(Scene s, Matrix4 VP)
		{
			bellowModelspace = Matrix4.CreateScale (1, 1, bellowSize) * bellowTransform * Modelspace;
		}

		public override void Render(Scene s, Matrix4 VP, INormalShader shader)
		{
			shader.ResetColor ();
			shader.SetModelspaceMatrix (bellowModelspace);
			shader.SetMVP (bellowModelspace*VP);
			bellow.Draw ();
		}

		public override void PostRender(Scene s, Matrix4 VP)
		{
			if (game.TooltipHelper.GetOwner () == this)
				game.TooltipHelper.RenderNormalDialog (Input.OrthoMouseX, Input.OrthoMouseY, Util.White60);
		}

		#region EntityEventListener implementation

		public void InteractionPerformed (object source)
		{
			if(bellowSize >= maxBellowSize) 
				bellowSpeed = -1;
		}

		#endregion
	}
}

