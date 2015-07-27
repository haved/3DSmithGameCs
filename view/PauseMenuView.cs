using OpenTK;

namespace DSmithGameCs
{
	public class PauseMenuView : MenuView
	{
		static readonly string[] options = {"ui.button.resume","ui.button.savegame", "ui.button.loadgame", "ui.button.exittomainmenu", "ui.button.exit"};

		readonly Smith2DGame game;

		public PauseMenuView(Smith2DGame game) : base(options)
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
			switch (button) {
			case 0:
				game.SetView (PrevView);
				return;
			case 1:
				game.GameStats.SaveGame ();
				game.SetView (PrevView);
				return;
			case 2:
				game.LoadGame ();
				return;
			case 3:
				game.ShowMainMenu ();
				return;
			case 4:
				game.Exit ();
				return;
			}
		}

		#endregion
	}
}

