using UnityEngine;
using System.Collections;

public class AnimationScript : MonoBehaviour {

	Animator animator;
	PlayerController playerController;

	void Start () {
		animator = GetComponent<Animator> ();
		playerController = GetComponent<PlayerController> ();
	}

	// Update is called once per frame
	void Update () {
		int stateInt = playerController.MoveStateToInt();

		animator.SetInteger ("MoveState", stateInt);
	}
}
