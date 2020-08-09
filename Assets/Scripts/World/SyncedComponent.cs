using UnityEngine;


namespace CrimeCommand.World {
	public abstract class SyncedComponent : MonoBehaviour {
		[SerializeField] private float stepsPerSecond = 1;

		public bool RunActively { get; set; } = false;

		private float fixedStepTimer, stepTimer;

		protected virtual void Start() {
			fixedStepTimer = 1 / stepsPerSecond;
			stepTimer = 0;
		}

		public abstract void Step();

		protected virtual void Update() {
			if (!RunActively) {
				return;
			}

			stepTimer += Time.deltaTime;
			if (stepTimer >= fixedStepTimer) {
				Step();
				stepTimer -= fixedStepTimer;
			}
		}
	}
}
