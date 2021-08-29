using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter : MonoBehaviour {

	[SerializeField]
	private AudioManager audioManager;
	[SerializeField]
	private Transform targetToFollow;
	[SerializeField]
	private Vector3 offset = new Vector3(0, 2, 0);

	void Start () {
		if (audioManager == null)
			audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

		if (targetToFollow == null)
			targetToFollow = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
	}
}
