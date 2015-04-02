using System;
using OpenTK;

namespace DSmithGameCs
{
	public class DialogController
	{
		Dialog currentDialog;
		Matrix4 projectionMatrix;

		public DialogController ()
		{
			projectionMatrix = Matrix4.CreateOrthographicOffCenter (0, 800, 0, 450, 0.0f, 1);
		}

		public bool Update()
		{
			if (currentDialog != null) {
				currentDialog.Update ();
				return true;
			}
			return false;
		}

		public void Render()
		{
			if (currentDialog != null) {
				GUIShader.GetInstance ().Bind ();
				GUIShader.GetInstance ().SetMVP (projectionMatrix);
				currentDialog.Render ();
				BasicShader.GetInstance ().Bind ();
			}
		}

		public void OpenDialog(Dialog dialog)
		{
			currentDialog = dialog;
			dialog.OnOpen ();
		}

		public void CloseDialog()
		{
			if (currentDialog != null) {
				currentDialog.OnClose ();
				currentDialog = null;
			}
		}
	}
}

