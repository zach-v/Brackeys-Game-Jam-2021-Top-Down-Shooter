using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public CharacterController controller;
	[SerializeField] private AudioManager audioManager;
	public Camera cam;
	public float moveSpeed = 5f;
	public float gravity = 10;
	[Range(0, 1)] public float accelerationTime = 0.15f;
	public float degreeOffset = 90f;
	public bool AllowedToMove = true;
	public bool AllowedToTurn = true;
	[Header("Chunk Stuff")]
	public int chunkScale = 10;
	public int chunkHorizontalDistance = 10;
	public int chunkVerticalDistance = 10;
	public EnvironmentGenerationManager evgManager;
	[ReadOnly] [SerializeField] private (int cx, int cz) chunkLocation = (1, 1);
	[Header("Walking Sound Fields")]
	[SerializeField] private float TimeToStepSoundJog = 0.9f;
	[SerializeField] private float TimeToStepSoundRun = 0.5f;
	[ReadOnly] [SerializeField] private int numberOfJogSounds = 0;
	[ReadOnly] [SerializeField] private int numberOfRunSounds = 0;
	[ReadOnly] [SerializeField] private float currentTimeToSound = 0;
	[Header("Internal Variables")]
	private float currentSpeed = 0;
	[ReadOnly] public Vector2 movement;
	[ReadOnly] [SerializeField] private Vector3 mousePos;
	[ReadOnly] [SerializeField] private float angle = 0;
	[ReadOnly] [SerializeField] private string DisplaySpeed = "0.0";

	void Awake()
	{
		// Populate random sound list
		foreach (Sound s in audioManager.walkingSounds)
		{
			if (s.name.Contains("Dirt_Jogging-"))
			{
				numberOfJogSounds++;
			}
			if (s.name.Contains("Dirt_Running-"))
			{
				numberOfRunSounds++;
			}
		}
	}
	void Start()
	{
		// Generate initial chunks at the start
		checkNearbyChunks(chunkLocation);
	}
	void Update()
	{
		movement.x = Input.GetAxisRaw("Horizontal");
		movement.y = Input.GetAxisRaw("Vertical");

		mousePos = Input.mousePosition;
	}
	void FixedUpdate()
	{
		// Store current movement variables
		currentSpeed = controller.velocity.magnitude;
		DisplaySpeed = currentSpeed.ToString("0.0");

		if (AllowedToMove)
		{
			// Movement
			if (movement.magnitude >= 0.1f)
			{
				Vector3 direction = new Vector3(movement.x, 0, movement.y).normalized;
				controller.Move(direction * Mathf.Lerp(currentSpeed, moveSpeed, accelerationTime) * Time.fixedDeltaTime);
			}
			if (movement.magnitude >= 0.1)
				currentTimeToSound += Time.fixedDeltaTime;
		}
		if (AllowedToTurn)
		{
			// Rotation
			Ray rayFromMouse = cam.ScreenPointToRay(mousePos);
			if (Physics.Raycast(rayFromMouse, out RaycastHit hitInfo, 300f))
			{
				Vector3 lookDir = hitInfo.point - controller.transform.position;

				angle = Mathf.Atan2(lookDir.z, lookDir.x) * Mathf.Rad2Deg - degreeOffset;

				controller.transform.rotation = Quaternion.Euler(new Vector3(0f, -angle, 0f));
			}
		}
		if (currentChunk(transform.position) != chunkLocation)
		{
			chunkLocation = currentChunk(transform.position);
			checkNearbyChunks(chunkLocation);
		}
	}
	void LateUpdate()
	{
		if (currentTimeToSound >= TimeToStepSoundJog)
		{
			audioManager.Play("Dirt_Jogging-" + Random.Range(1, numberOfJogSounds + 1), Sound.SoundType.Walking);
			currentTimeToSound = 0;
		}
	}
	private (int, int) currentChunk(Vector3 position)
	{
		int x = (int)(position.x / chunkScale);
		int z = (int)(position.z / chunkScale);
		return (x, z);
	}
	private void checkNearbyChunks((int x, int z) location)
	{
		if (evgManager != null)
		{
			// Create an empty list to use as keys
			List<(int, int)> chunks = new List<(int, int)>();
			// Get half the render distance
			int cxHalf = (chunkHorizontalDistance / 2);
			int czHalf = (chunkVerticalDistance / 2);
			// Populate the list with our new render location
			for (int a = location.x - cxHalf; a <= location.x + cxHalf; a++)
			{
				for (int b = location.z - czHalf; b <= location.z + czHalf; b++)
				{
					chunks.Add((a, b));
				}
			}
			StartCoroutine(evgManager.UpdateChunkMap(chunks.ToArray(), chunkScale));
		}
	}
}
