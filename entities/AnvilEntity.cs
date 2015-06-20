using System;
using OpenTK;

namespace DSmithGameCs
{
	public class AnvilEntity : InteractiveEntity, IEntityEventListener, IView
	{
		readonly Smith2DGame game;
		readonly float height;
		CoalStripTable table;

		public AnvilEntity(Smith2DGame game, Mesh m, float x, float y, float z, float xSize, float ySize, float height) : base(m, x, y, z, xSize, ySize){
			this.game = game;
			this.height = height;
			EventHandler = this;
		}

		public void InteractionPerformed(InteractiveEntity entity, object source)
		{
			table.InteractionPerformed (entity, source);
		}

		//Stuff related to the view
		Transition transition = new Transition();
		IView parentView;
		BladeItem blade;
		int hotspot = -1;
		float temperature = 25;
		float panAngle = 0;
		public void OnViewUsed(IView prevView)
		{
			transition.SetStart (prevView);
		}

		public bool ShouldUpdateScene ()
		{
			return false;
		}

		public void UpdateView (Scene s)
		{
			transition.UpdateTransition (Time.Delta()*2);

			if (Input.LeftKey) {
				if (!Input.RightKey)
					panAngle -= (panAngle + 0.4f) * Time.Delta () * 3;
			}
			else if (Input.RightKey)
				panAngle -= (panAngle - 0.4f) * Time.Delta () * 3;
			else
				panAngle -= panAngle * Time.Delta () * 3;

			if (Input.MousePressed) {
				game.SetView (table);
				table.OnTableUsed (hotspot, temperature);
			}
			else if (Input.CloseKeyPressed)
				game.SetView (parentView);
		}

		public bool ShouldRenderScene ()
		{
			return true;
		}
			
		public Vector3 GetEyePos()
		{
			return transition.GetEyePos (Pos+new Vector3((float)Math.Sin(panAngle)*10,0,(float)Math.Cos(panAngle)*10));
		}

		public Vector3 GetEyeTarget()
		{
			return transition.GetEyeTarget (Pos);
		}

		public Vector3 GetEyeUp()
		{
			return transition.GetEyeUp (Vector3.UnitY);
		}

		public void RenderView (Matrix4 VP, Scene s)
		{
			blade.RenderBlade (VP, Pos.X, Pos.Y- (hotspot<0?-0.6f:blade.Type.Points[hotspot]*blade.Type.MeshScale), height, Util.PI/2, hotspot, temperature);
		}

		public void SetCoalStripTable(CoalStripTable table)
		{
			this.table = table;
		}

		public void OnAnvilUsed(IView parentView, BladeItem blade, int hotspot, float temperature)
		{
			this.parentView = parentView;
			this.blade = blade;
			this.hotspot = hotspot;
			this.temperature = temperature;
		}
	}
}

