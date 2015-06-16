using System;
using OpenTK;

namespace DSmithGameCs
{
	public class SlimCoalTableEntity : InteractiveEntity, IEntityEventListener, View
	{
		readonly Smith2DGame game;
		readonly Mesh table, coal;
		Vector4 coalColor = new Vector4(1,1,1,1);
		public SlimCoalTableEntity (Smith2DGame game, Mesh table, Mesh coal, float x, float y, float z, float xSize, float ySize) : base(null, x,y,z,xSize,ySize)
		{
			this.game = game;
			this.table = table;
			this.coal = coal;
			EventHandler = this;
			eyeTarget = new Vector3 (x, y, 0);
			eyePos = eyeTarget + new Vector3 (0, 0, 10);
		}

		public override void Render(Scene s, Matrix4 VP)
		{
			Matrix4 MVP = Modelspace * VP;
			BasicShader.GetInstance ().Bind ();
			BasicShader.GetInstance ().SetModelspaceMatrix (Modelspace);
			BasicShader.GetInstance ().SetMVP (MVP);
			table.Draw ();
			ColorShader.GetInstance ().Bind ();
			ColorShader.GetInstance ().SetMVP (MVP);
			coalColor.X = coalColor.Y = coalColor.Z = 1.3f + (float)Math.Sin(Time.CurrentTime()*2)/6;
			ColorShader.GetInstance ().SetColor (coalColor);
			coal.Draw ();
		}

		#region IEntityEventListener implementation

		public void InteractionPerformed (InteractiveEntity entity, object source)
		{
			game.SetView (this);
		}

		#endregion

		#region View implementation

		Transition transition = new Transition();
		public void OnViewUsed (View prevView)
		{
			transition.SetStart (prevView);
		}

		public bool ShouldUpdateScene ()
		{
			return false;
		}

		public void UpdateView (Scene s)
		{
			transition.UpdateTransition (Time.Delta () * 2f);
		}

		public bool ShouldRenderScene ()
		{
			return true;
		}

		readonly Vector3 eyeTarget;
		readonly Vector3 eyePos;
		static readonly Vector3 eyeUp =	Vector3.UnitY;
		public Vector3 GetEyePos ()
		{
			return transition.GetEyePos (eyePos);
		}

		public Vector3 GetEyeTarget ()
		{
			return transition.GetEyeTarget (eyeTarget);
		}

		public Vector3 GetEyeUp ()
		{
			return transition.GetEyeUp (eyeUp);
		}

		public void RenderView (Scene s)
		{
			
		}

		#endregion
	}
}

