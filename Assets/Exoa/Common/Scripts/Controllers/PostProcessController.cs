using DG.Tweening;
using Exoa.Events;
using UnityEngine;
//using UnityEngine.Rendering.PostProcessing;

namespace Exoa.Designer
{
	public class PostProcessController : MonoBehaviour
	{
		//private PostProcessLayer layer;
		public Light directionalLight;
		public float defaultShadowStrength = 0.22f;

		void OnDestroy()
		{
			CameraEvents.OnBeforeSwitchPerspective -= OnBeforeSwitchPerspective;
		}
		void Awake()
		{
			//layer = GetComponent<PostProcessLayer>();
			CameraEvents.OnBeforeSwitchPerspective += OnBeforeSwitchPerspective;
		}

		private void OnBeforeSwitchPerspective(bool orthoMode)
		{
			DOTween.To(() => directionalLight.shadowStrength, x => directionalLight.shadowStrength = x, orthoMode ? 0 : defaultShadowStrength, 1);
		}
	}
}
