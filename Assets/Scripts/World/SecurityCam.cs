using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;

namespace CrimeCommand.World {
	public class SecurityCam : SyncedComponent {
		[SerializeField] private GameObject player = null;
		[SerializeField] private Color defaultColor = new Color(1, 59f / 255, 59f / 255, 32f / 255);
		[SerializeField] private Color hackedColor = new Color(12f / 255, 20f / 255, 1, 32f / 255);
		[SerializeField] private Light spotlight = null;
		[SerializeField] private Vector2 angleExtents = new Vector2(-45, 45);

		public bool IsHacked { 
			get => isHacked; 
			set { 
				isHacked = value;
				if (isHacked) {
					spotlight.color = hackedColor;
				} else {
					spotlight.color = defaultColor;
				}
			} 
		}

		public Player Controller { get; set; } = null;
		public Camera Cam { get; private set; }

		private Plane[] planes;
		private Collider playerCollider;

		private float currentRotation = 0;
		private bool isRotating = false;
		private bool isRotatingClockwise = true;
		private bool isHacked = false;

		protected override void Start() {
			base.Start();
			Cam = GetComponentInChildren<Camera>();
			playerCollider = player.GetComponent<Collider>();
		}

		protected override void Update() {
			if (Controller == null) {
				base.Update();

				if (!IsHacked && GeometryUtility.TestPlanesAABB(planes, playerCollider.bounds)) {
					if (Physics.Raycast(transform.position, (player.transform.position - transform.position).normalized, out RaycastHit hit)) {
						if (hit.collider.CompareTag("Player")) {
							Debug.Log("Player is visible to camera");
						}
					}
				}
			} else {
				if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
					isRotatingClockwise = false;
					StartCoroutine(nameof(Rotate));
				}
				if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
					isRotatingClockwise = true;
					StartCoroutine(nameof(Rotate));
				}
			}
		}

		public override void Step() {
			if (Controller == null) {
				StartCoroutine(nameof(Rotate));
			}
		}

		private IEnumerator Rotate() {
			while (isRotating) {
				yield return null;
			}

			isRotating = true;

			float change = (isRotatingClockwise ? 15 : -15);
			currentRotation += change;
			Vector3 degrees = Vector3.up * change;

			Quaternion start = transform.rotation;
			Quaternion end = Quaternion.Euler(transform.eulerAngles + degrees);
			for (var t = 0f; t < 1; t += Time.deltaTime) {
				transform.rotation = Quaternion.Slerp(start, end, t);
				yield return null;
			}

			transform.rotation = end;
			if (currentRotation <= angleExtents.x || currentRotation >= angleExtents.y) {
				isRotatingClockwise = !isRotatingClockwise;
			}

			planes = GeometryUtility.CalculateFrustumPlanes(Cam);

			isRotating = false;
		}
	}
}
