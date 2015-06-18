using System;
using OpenTK;

namespace DSmithGameCs
{
	public interface IView
	{
		void OnViewUsed(IView prevView);

		bool ShouldUpdateScene();

		void UpdateView(Scene s);

		bool ShouldRenderScene ();

		Vector3 GetEyePos();

		Vector3 GetEyeTarget();

		Vector3 GetEyeUp();

		void RenderView(Matrix4 VP, Scene s);
	}
}

