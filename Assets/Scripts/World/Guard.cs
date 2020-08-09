using CrimeCommand.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : SyncedComponent {

	[SerializeField] private float speed = 3;
	[SerializeField] private float turnSpeed = 2;
	[SerializeField] private WallChecker wallChecker = null;
	[SerializeField] private List<Direction> route = new List<Direction>();

	private Rigidbody rb;
	private int currentRouteStep = 0;
	private bool isMoving = false;
	private int moveCount = 0;

	protected override void Start() {
		base.Start();

		rb = GetComponent<Rigidbody>();

	}

	public override void Step() {
		if(route.Count > 0) {
			if (route[currentRouteStep] == Direction.Forward) {
				StartCoroutine(nameof(Move));
			} else {
				Turn(route[currentRouteStep]);
			}
			currentRouteStep++;
			currentRouteStep %= route.Count;
		}
	}

	public void Turn(Direction direction) {
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
		while (isMoving || moveCount > 0) {
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

	private IEnumerator Move() {
		moveCount++;
		while (isMoving) {
			yield return null;
		}

		if (wallChecker.InContact) {
			isMoving = false;
			yield break;
		}

		isMoving = true;
		moveCount--;


		Vector3 end = transform.position;

		end += transform.forward;

		Vector3 start = transform.position;
		for (float t = 0; t < 1; t += Time.deltaTime * speed) {
			transform.position = Vector3.Lerp(start, end, t);
			yield return null;
		}

		isMoving = false;
	}

}
