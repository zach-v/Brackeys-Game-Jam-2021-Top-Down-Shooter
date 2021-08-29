using Assets.Scripts.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BiomeManager;

public class PlayerManager : MonoBehaviour
{
	[Header("Manager Fields")]
	[SerializeField] private PlayerMovement movement;
	[SerializeField] private AudioManager audioManager;
	[SerializeField] private ItemManager weaponManager;
	[SerializeField] private BiomeManager biomeManager;
	[Header("Walking Sound Fields")]
	[SerializeField] private float TimeToStepSoundJog = 0.9f;
	[SerializeField] private float TimeToStepSoundRun = 0.5f;
	[Header("Item Fields")]
	[SerializeField] private Transform GunPoint;
	[SerializeField] private Transform GunPoint2;
	private Vector3 GunPointOriginalPosition;
	private Vector3 GunPoint2OriginalPosition;
	[ReadOnly] [SerializeField] private GameObject ItemInHand;
	[ReadOnly] [SerializeField] private Gun currentGun = null;
	[ReadOnly] [SerializeField] private List<Gun> gunInventory;
	[ReadOnly] [SerializeField] private Grenade currentGrenade;
	[ReadOnly] [SerializeField] private List<Grenade> grenadeInventory;
	[ReadOnly] [SerializeField] private List<Item> itemInventory;
	[Header("UI Interaction")]
	[SerializeField] private GameObject InventoryCanvas;
	[SerializeField] private FlexibleGridLayout GunTableGrid;
	[SerializeField] private FlexibleGridLayout GrenadeTableGrid;
	[SerializeField] private FlexibleGridLayout ItemsTableGrid;
	[Header("Other Readonlys")]
	[SerializeField] private Biome currentBiome = Biome.Planes;
	[ReadOnly] [SerializeField] private Biome previousBiome = Biome.Void;
	[ReadOnly] [SerializeField] private float currentTimeToSound = 0;
	[ReadOnly] [SerializeField] private int numberOfJogSounds = 0;
	[ReadOnly] [SerializeField] private int numberOfRunSounds = 0;
	[ReadOnly] [SerializeField] private bool continueToFire = true;

	public PlayerManager()
	{
		gunInventory = new List<Gun>();
		grenadeInventory = new List<Grenade>();
	}
	void Awake()
	{
		GunPointOriginalPosition = GunPoint.position;
		GunPoint2OriginalPosition = GunPoint2.position;
		// Set current gun to first one in list
		AddWeaponToInventory(weaponManager.GunList[0]);
		SetCurrentGun(0);
		// Populate random sound list
		foreach (Sound s in audioManager.itemSounds)
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
		StartCoroutine(CheckWeaponBulletList());
		StartCoroutine(CheckBiome());
	}
	#region Update Methods
	private void Update()
	{
		// If we are not in the inventory screen
		if (!InventoryCanvas.gameObject.activeSelf)
		{
			CheckShooting();
		}
		// Change weapon
		if (Input.GetButton("Fire3"))
		{
			InventoryCanvas.gameObject.SetActive(true);
			movement.AllowedToTurn = false;
			audioManager.ChangeFilterState(AudioManager.FilterState.LowPass);
		}
		if (Input.GetButtonUp("Fire3"))
		{
			InventoryCanvas.gameObject.SetActive(false);
			movement.AllowedToTurn = true;
			audioManager.ChangeFilterState(AudioManager.FilterState.Normal);
		}
		UpdateWeaponTimes();
	}
	private void UpdateWeaponTimes()
	{
		List<WeaponBase> allWeapons = new List<WeaponBase>(gunInventory);
		allWeapons.AddRange(grenadeInventory.ToArray());
		foreach (WeaponBase weapon in allWeapons)
		{
			if (weapon.RecentlyUsed)
			{
				weapon.TimeSinceUsed += Time.deltaTime * 1000;
				if (weapon.TimeSinceUsed >= weapon.UsageRate)
				{
					weapon.TimeSinceUsed = 0;
					weapon.RecentlyUsed = false;
				}
			}
		}
	}
	private void FixedUpdate()
	{
		if (movement.movement.magnitude >= 0.1)
			currentTimeToSound += Time.fixedDeltaTime;
	}
	private void LateUpdate()
	{
		if (currentTimeToSound >= TimeToStepSoundJog)
		{
			audioManager.Play("Dirt_Jogging-" + Mathf.RoundToInt(Random.Range(1, numberOfJogSounds)), Sound.SoundType.Walking);
			currentTimeToSound = 0;
		}
	}
	private void CheckShooting()
	{
		// if not recently used
		if (!currentGun.RecentlyUsed)
		{
			if (Input.GetButton("Fire1") && continueToFire)
			{
				// If the weapon is hitscan
				if (currentGun.HitScan)
				{
					// For all shots in a single fire operation
					for (int i = 0; i < currentGun.ShotCount; i++)
					{
						// Create some variation in spread
						Vector3 betterSpread = currentGun.Spread(GunPoint.forward, 10, currentGun.spread);
						// Make a new gameobject for the tracers
						GameObject newBullet = Instantiate(currentGun.TracerEffect, GunPoint.position, Quaternion.LookRotation(betterSpread));
						// Attach die after time to it and set the time
						killSelf killSelfComponent = newBullet.AddComponent(typeof(killSelf)) as killSelf;
						killSelfComponent.timeTillDeath = currentGun.TracerEffectTime;
						// Add it to the list of bullets
						currentGun.ActiveBullets.Add(newBullet);
						// Handle damage dealing
						Debug.DrawRay(GunPoint.position, betterSpread, Color.green);
						if (Physics.Raycast(GunPoint.position, betterSpread, out RaycastHit hit))
						{
							if (hit.collider.gameObject.layer.Equals(currentGun.TargetLayer.MaskToLayer()))
							{
								hit.collider.gameObject.GetComponent<HealthScript>().CurrentHealth -= currentGun.Damage;
							}
						}
					}
					// Play fire sound
					audioManager.Play(currentGun.FireSoundName);
				}
				else // Not hitscan
				{
					// TODO Create projectile system for shooting projectile based weapons
				}
				if (currentGun.SingleFire)
				{
					continueToFire = false;
				}
				currentGun.RecentlyUsed = true;
			}
			if (Input.GetButtonUp("Fire1"))
			{
				continueToFire = true;
			}
		}
	}
	#endregion
	private int GetIndexOfType<T>(T weapon) where T : WeaponBase
	{
		if (typeof(T).IsEquivalentTo(typeof(Gun)))
			return gunInventory.IndexOf(weapon as Gun);
		if (typeof(T).IsEquivalentTo(typeof(Grenade)))
			return grenadeInventory.IndexOf(weapon as Grenade);
		return -1;
	}
	private void SetCurrentGun(int index)
	{
		// Remove previous item
		Destroy(ItemInHand);
		// Add new one based on index
		currentGun = gunInventory[index];
		ItemInHand = Instantiate(currentGun.WeaponModel, GunPoint.position + currentGun.PositionOffset,
			Quaternion.Euler(GunPoint.rotation.eulerAngles + currentGun.RotationOffset), transform);

		if (currentGun.SingleHanded)
		{
			GunPoint2.position = new Vector3(GunPoint.position.x, GunPoint.position.y - 0.1f, GunPoint.position.z);
		}
		else
		{
			GunPoint2.position = GunPoint2OriginalPosition;
		}
	}
	public bool AddWeaponToInventory(WeaponBase weapon)
	{
		try
		{
			if (weapon.GetType().IsEquivalentTo(typeof(Gun)))
				gunInventory.Add(weapon as Gun);
			if (weapon.GetType().IsEquivalentTo(typeof(Grenade)))
				grenadeInventory.Add(weapon as Grenade);
		}
		catch (System.Exception)
		{
			return false;
		}
		return true;
	}
	#region Coroutines
	private IEnumerator CheckWeaponBulletList()
	{
		for (; ; )
		{
			currentGun.ActiveBullets.RemoveAll(bullet => bullet == null);
			yield return new WaitForSeconds(1);
		}
	}
	private IEnumerator CheckBiome()
	{
		for(; ; )
		{
			if (previousBiome != currentBiome)
			{
				switch (currentBiome)
				{
					case Biome.Planes:
						break;
					case Biome.Swamp:
						audioManager.Play("ambient-swamp");
						break;
					case Biome.Forest:
						audioManager.Play("ambient-forest");
						break;
					case Biome.Hell:
						audioManager.Play("ambient-hell");
						break;
				}
				previousBiome = currentBiome;
			}
			//currentBiome = biomeManager.GetBiomeAt(transform.position);
			yield return new WaitForSeconds(1);
		}
	}
	#endregion
}
