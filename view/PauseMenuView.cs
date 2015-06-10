﻿using OpenTK;
using System.Drawing;

namespace DSmithGameCs
{
	public class PauseMenuView : MenuView
	{
		static readonly string[] options = {"ui.button.resume","ui.button.savegame", "ui.button.loadgame", "ui.button.exittomainmenu"};

		public static PauseMenuView Instance;

		readonly Smith2DGame game;

		PauseMenuView(Smith2DGame game) : base(options)
		{
			this.game = game;
		}

		#region implemented abstract members of MenuView
		public override Vector3 GetEyePos ()
		{
			return PrevView.GetEyePos ();
		}
		public override Vector3 GetEyeTarget ()
		{
			return PrevView.GetEyeTarget ();
		}
		public override Vector3 GetEyeUp ()
		{
			return PrevView.GetEyeUp ();
		}

		public override void OnButtonPressed (int button)
		{
			if (button == 0)
				game.SetView (PrevView);
		}

		#endregion

		public static void MakeInstance(Smith2DGame game)
		{
			Instance = new PauseMenuView (game);
		}
	}
}

