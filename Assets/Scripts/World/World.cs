using UnityEngine;

namespace CrimeCommand.World {
	public class World : MonoBehaviour {
		private SyncedComponent[] components;

		private void Start() {
			components = FindObjectsOfType<SyncedComponent>();
		}

		public void Step() {
			foreach (SyncedComponent component in components) {
				component.Step();
			}
		}

		public void ToggleActiveTime() {
			foreach (SyncedComponent component in components) {
				component.RunActively = !component.RunActively;
			}
		}
	}
}
