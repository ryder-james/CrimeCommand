using System.Collections;
using UnityEngine;

namespace CrimeCommand.UI {
	[RequireComponent(typeof(RectTransform))]
	public class SlideUI : MonoBehaviour {
		[SerializeField] private bool startOnScreen = false;
		[SerializeField] private float transitionDuration = 0.5f;
		[SerializeField] private Vector2 onScreen = Vector2.zero;
		[SerializeField] private Vector2 offScreen = Vector2.zero;

		private bool isOnScreen, isSliding = false;
		private RectTransform rect;

		private void Start() {
			isOnScreen = startOnScreen;
			rect = GetComponent<RectTransform>();

			if (isOnScreen) {
				onScreen = rect.localPosition;
			}
		}

		public void Toggle() {
			StartCoroutine(nameof(Slide), !isOnScreen);
		}

		private IEnumerator Slide(bool on) {
			while (isSliding) {
				yield return null;
			}

			isSliding = true;
			for (float t = 0; t < 1; t += Time.deltaTime / transitionDuration) {
				rect.localPosition = Vector2.Lerp(on ? offScreen : onScreen, on ? onScreen : offScreen, t);
				yield return null;
			}
			rect.localPosition = on ? onScreen : offScreen;
			isOnScreen = !isOnScreen;
			isSliding = false;
		}
	}
}