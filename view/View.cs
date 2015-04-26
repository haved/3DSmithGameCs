using System;
using OpenTK;

namespace DSmithGameCs
{
	public interface View
	{
		void OnViewUsed(View prevView);

		bool ShouldUpdateScene();

		void UpdateView(Scene s);

		bool ShouldRenderScene ();

		Vector3 GetEyePos();

		Vector3 GetEyeTarget();

		Vector3 GetEyeUp();

		void RenderView(Scene s);
	}
}

