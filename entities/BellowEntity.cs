using System;
using System.Drawing;
using OpenTK;

namespace DSmithGameCs
{
	public class BellowEntity : InteractiveEntity, IEntityEventListener
	{
		const float maxBellowSize = 1;
		const float minBellowSize = 0.4f;
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

		Matrix4 newModelspace;
		public override void PreRender(Scene s, Matrix4 VP)
		{
			newModelspace = Matrix4.CreateScale (1, 1, bellowSize) * tip * Modelspace;
		}

		public override void Render(Scene s, Matrix4 VP, INormalShader shader)
		{
			shader.ResetColor ();
			shader.SetModelspaceMatrix (newModelspace);
			shader.SetMVP (newModelspace*VP);
			Draw (s);
		}

		public override void PostRender(Scene s, Matrix4 VP)
		{
			if (game.TooltipHelper.GetOwner () == this)
				game.TooltipHelper.RenderNormalDialog (Input.OrthoMouseX, Input.OrthoMouseY, Util.White60);
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

