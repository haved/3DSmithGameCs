using System;

namespace DSmithGameCs
{
	public class AnvilDialog : Dialog
	{
		static readonly Mesh dialog = new Mesh("../../res/dialog.ply");

		public AnvilDialog (Smith2DGame g)
		{
			
		}

		public override void Render()
		{
			dialog.Draw ();
		}
	}
}

