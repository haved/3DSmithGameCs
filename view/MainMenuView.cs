using System;
using OpenTK;

namespace DSmithGameCs
{
	public class MainMenuView : MenuView
	{
		static readonly string[] options = {"ui.button.newgame","ui.button.loadgame","ui.button.exit"};
		
		readonly Smith2DGame game;

		public MainMenuView (Smith2DGame game) : base(options)
		{
			this.game = game;
		}

		#region implemented abstract members of MenuView

		static readonly Vector3 eyePos = new Vector3(1, -3, 6);
		static readonly Vector3 eyeTarget = new Vector3(1, 7, 0);
		static readonly Vector3 eyeUp = Vector3.UnitY;
		public override Vector3 GetEyePos ()
		{
			return eyePos;
		}

		public override Vector3 GetEyeTarget ()
		{
			return eyeTarget;
		}

		public override Vector3 GetEyeUp ()
		{
			return eyeUp;
		}

		public override void OnButtonPressed (int button)
		{
			switch(button){
			case 0:
				game.NewGame ();
				return;
			case 1:
				game.LoadGame ();
				return;
			case 2:
				game.Exit ();
				return;
			}
		}

		#endregion
	}
}

