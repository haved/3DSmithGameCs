using System;
using OpenTK;

namespace DSmithGameCs
{
	public class AnvilEntity : InteractiveEntity, IEntityEventListener, IView
	{
		readonly Smith2DGame game;
		CoalStripTable table;

		public AnvilEntity(Smith2DGame game, Mesh m, float x, float y, float z, float xSize, float ySize) : base(m, x, y, z, xSize, ySize){
			this.game = game;
			EventHandler = this;
		}

		public void InteractionPerformed(InteractiveEntity entity, object source)
		{
			table.InteractionPerformed (entity, source);
		}

		//Stuff related to the view
		Transition transition = new Transition();
		IView parentView;
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
			if (Input.MousePressed)
				game.SetView (table);
			else if (Input.CloseKeyPressed)
				game.SetView (parentView);
		}

		public bool ShouldRenderScene ()
		{
			return true;
		}

		static Vector3 eyeOffset = new Vector3(0, 0, 10);
		public Vector3 GetEyePos()
		{
			return transition.GetEyePos (Pos+eyeOffset);
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
			
		}

		public void SetCoalStripTable(CoalStripTable table)
		{
			this.table = table;
		}

		public void SetParentView(IView parentView)
		{
			this.parentView = parentView;
		}
	}
}

