using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	public Transform player;
	public float smoothing = 0.1f;
	public Vector3 offset;

	public void FixedUpdate()
	{
		Vector3 targetPosition = player.position + offset;
		// smooth to target position
		Vector3 smoothedTargetPosition = Vector3.Lerp(transform.position, targetPosition, smoothing);
		transform.position = smoothedTargetPosition;
	}
}
