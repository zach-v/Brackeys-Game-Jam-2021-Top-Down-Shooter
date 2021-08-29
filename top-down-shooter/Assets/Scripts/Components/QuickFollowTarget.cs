using UnityEngine;

public class QuickFollowTarget : MonoBehaviour {

	[SerializeField]
	private Transform target;
	[SerializeField]
	private Vector3 offset = new Vector3(0, 2, 0);

	void Update () {
		if (target != null)
			transform.position = target.position + offset;
	}
}
