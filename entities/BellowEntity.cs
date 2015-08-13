using System.Drawing;
using OpenTK;

namespace DSmithGameCs
{
	public abstract class BellowEntity : ColliderEntity, IInteractiveEntity
	{
		protected readonly Smith2DGame Game;
		readonly Mesh bellow;
		readonly Mesh rig;
		readonly Matrix4 bellowTransform;

		protected BellowEntity (Smith2DGame game, Mesh bellow, Matrix4 bellowTransform, Mesh rig, float x, float y, float z, float xSize, float ySize) : base(x, y, z, xSize, ySize)
		{
			Game = game;
			this.bellow = bellow;
			this.bellowTransform = bellowTransform;
			this.rig = rig;
		}

		public override void DisposeEntity()
		{
			bellow.Dispose ();
			rig.Dispose ();
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
				ImproveOxygen(Time.Delta () * 20);
			if (bellowSize > maxBellowSize & bellowSpeed > 0)
				bellowSpeed -= bellowSpeed * Time.Delta () * 6;
			if (bellowSize < minBellowSize)
				bellowSpeed += Time.Delta () * 4;

			if (Game.Player.IsLookingAt (this)) {
				if (Game.TooltipHelper.ClaimIfPossible (this) || (Game.TooltipHelper.GetOwner () == this & prevAirQuality != (prevAirQuality=(int)GetOxygenPrecent()))) {
					Game.TooltipHelper.Writer.DrawStandardTooltip (new []{ Localization.GetLocalization( "ui.tooltip.oxygen:" ) }, new []{ Color.Aqua }, new []{prevAirQuality + "%" });
					prevAirQuality = -1;
				}
			} else if (Game.TooltipHelper.GetOwner () == this)
				Game.TooltipHelper.UnClaim ();
		}

		public abstract void ImproveOxygen(float f);

		public abstract float GetOxygenPrecent();

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
			shader.SetModelspaceMatrix (Modelspace);
			shader.SetMVP (Modelspace * VP);
			rig.Draw ();
		}

		public override void PostRender(Scene s, Matrix4 VP)
		{
			if (Game.TooltipHelper.GetOwner () == this)
				Game.TooltipHelper.RenderNormalDialog (Input.OrthoMouseX, Input.OrthoMouseY, Util.White60);
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

