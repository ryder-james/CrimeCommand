using System.Collections;
using System.Linq;
using UnityEngine;

namespace CrimeCommand.World {
	[RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour {
		[SerializeField] private float speed = 3;
		[SerializeField] private float turnSpeed = 2;
		[SerializeField] private WallChecker wallChecker = null;
		[SerializeField] private World world = null;

		public bool IsViewingCam { get; set; } = false;
		public WallChecker WallChecker { get => wallChecker; set => wallChecker = value; }

		private Rigidbody rb;
		private Camera firstPersonCam;
		private SecurityCam hackedCam;
		private SecurityCam[] cameras;

		private bool isMoving = false;

		private void Start() {
			rb = GetComponent<Rigidbody>();
			firstPersonCam = Camera.main;
			cameras = GameObject.FindGameObjectsWithTag("Camera").Select(g => g.GetComponent<SecurityCam>()).ToArray();
		}

		public bool Hack(string camName) {
			world.Step();

			foreach (SecurityCam cam in cameras) {
				if (cam.gameObject.name.ToLower() == camName.ToLower()) {
					if (hackedCam != null) {
						hackedCam.IsHacked = false;
					}
					hackedCam = cam;
					hackedCam.IsHacked = true;
					return true;
				}
			}

			return false;
		}

		public bool ViewCam(bool viewing) {
			if (hackedCam != null) {
				IsViewingCam = viewing;

				if (viewing) {
					firstPersonCam.depth = 0;
					hackedCam.Controller = this;
					hackedCam.Cam.depth = 1;
				} else {
					hackedCam.Cam.depth = 0;
					hackedCam.Controller = null;
					firstPersonCam.depth = 1;
				}
				
				world.Step();

				return true;
			}

			world.Step();

			return false;
		}

		public void Move(Direction direction) {
			world.Step();

			Vector3 targetTile = transform.position + transform.forward;
			StartCoroutine(nameof(MoveTo), direction);
		}

		public void Turn(Direction direction) {
			world.Step();

			Vector3 degrees;

			switch (direction) {
			default:
			case Direction.Forward:
				degrees = Vector3.zero;
				break;
			case Direction.Back:
				degrees = 180 * Vector3.up;
				break;
			case Direction.Right:
				degrees = 90 * Vector3.up;
				break;
			case Direction.Left:
				degrees = -90 * Vector3.up;
				break;
			}

			StartCoroutine(nameof(Rotate), degrees);
		}

		private IEnumerator Rotate(Vector3 angle) {
			while (isMoving) {
				yield return null;
			}

			isMoving = true;

			Vector3 startPos = transform.position;
			rb.constraints ^= RigidbodyConstraints.FreezeRotationY;

			Quaternion start = transform.rotation;
			Quaternion end = Quaternion.Euler(transform.eulerAngles + angle);
			for (var t = 0f; t < 1; t += Time.deltaTime * turnSpeed) {
				transform.rotation = Quaternion.Slerp(start, end, t);
				yield return null;
			}

			transform.rotation = end;
			transform.position = startPos;

			rb.constraints |= RigidbodyConstraints.FreezeRotationY;
			isMoving = false;
		}

		private IEnumerator MoveTo(Direction direction) {
			while (isMoving) {
				yield return null;
			}

			if (direction == Direction.Forward && WallChecker.InContact) {
				isMoving = false;
				yield break;
			}

			isMoving = true;

			Vector3 end = transform.position;
			switch (direction) {
			default:
			case Direction.Forward:
				end += transform.forward;
				break;
			case Direction.Back:
				end -= transform.forward;
				break;
			case Direction.Right:
				end += transform.right;
				break;
			case Direction.Left:
				end -= transform.right;
				break;
			}

			Vector3 start = transform.position;
			for (float t = 0; t < 1; t += Time.deltaTime * speed) {
				transform.position = Vector3.Lerp(start, end, t);
				yield return null;
			}

			isMoving = false;
		}

	}
}
