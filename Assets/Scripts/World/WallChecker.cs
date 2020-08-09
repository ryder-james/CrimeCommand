using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrimeCommand.World {
    [RequireComponent(typeof(Collider))]
    public class WallChecker : MonoBehaviour {
		public bool InContact { get; private set; }
		public bool TouchPainting { get; private set; }
		public GameObject Painting { get; private set; }

		private Collider trigger;

		private void Start() {
			trigger = GetComponent<Collider>();
			trigger.isTrigger = true;
		}

		private void OnTriggerEnter(Collider other) {
			if (other.CompareTag("Wall")) {
				InContact = true;
			}
			if (other.CompareTag("Painting")) {
				TouchPainting = true;
				Painting = other.gameObject;
			}
		}

		private void OnTriggerExit(Collider other) {
			if (other.CompareTag("Wall")) {
				InContact = false;
			}
			if (other.CompareTag("Painting")) {
				TouchPainting = false;
				Painting = null;
			}
		}
	}
}
