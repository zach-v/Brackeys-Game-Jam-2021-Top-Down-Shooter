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
		AddItemToInventory(weaponManager.GunList[0]);
		SetCurrentGun(0);
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
	private void CheckShooting()
	{
		// if not recently used
		if (!currentGun.RecentlyUsed)
		{
			if (Input.GetButton("Fire1") && continueToFire)
			{
				// Muzzle position
				Vector3 muzzlePosition = ItemInHand.transform.position + currentGun.muzzle.transform.position;

				// For all shots in a single fire operation
				for (int i = 0; i < currentGun.ShotCount; i++)
				{
					// Create some variation in spread
					Vector3 betterSpread = currentGun.Spread(GunPoint.forward, currentGun.Range, currentGun.spread);
					// Make a new gameobject for the tracers
					GameObject fireEffect = Instantiate(currentGun.FireEffect, muzzlePosition,
								Quaternion.LookRotation(betterSpread), transform);
					// Attach die after time to it and set the time
					killSelf killSelfComponentFireEffect = fireEffect.AddComponent(typeof(killSelf)) as killSelf;
					killSelfComponentFireEffect.timeTillDeath = currentGun.FireEffectTime;
					// Add it to the list of effects
					currentGun.ActiveEffects.Add(fireEffect);
					// If the weapon is hitscan
					if (currentGun.HitScan)
					{
						// Handle damage dealing
						Debug.DrawRay(muzzlePosition, betterSpread, Color.green);
						if (Physics.Raycast(muzzlePosition, betterSpread, out RaycastHit hit))
						{
							if (hit.collider.gameObject.layer.Equals(currentGun.TargetLayer.MaskToLayer()))
							{
								hit.collider.gameObject.GetComponent<HealthScript>().CurrentHealth -= currentGun.Damage;
							}
							// Create the impact effect
							GameObject impactEffect = Instantiate(currentGun.ImpactEffect,
								hit.point, Quaternion.LookRotation(betterSpread));
							// Attach die after time to it and set the time
							killSelf killSelfComponentHitEffect = impactEffect.AddComponent(typeof(killSelf)) as killSelf;
							killSelfComponentHitEffect.timeTillDeath = currentGun.ImpactEffectTime;
							// Add it to the list of effects
							currentGun.ActiveEffects.Add(impactEffect);
						}
					}
					else // Not hitscan
					{
						// TODO Create projectile system for shooting projectile based weapons
					}
				}
				// Play fire sound
				audioManager.Play(currentGun.FireSoundName, Sound.SoundType.Item, 0.075f);
				// If single fire, toggle continue fire
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
	private int GetIndexOfType<T>(T item) where T : ItemInterface
	{
		if (typeof(T).IsEquivalentTo(typeof(Gun)))
			return gunInventory.IndexOf(item as Gun);
		if (typeof(T).IsEquivalentTo(typeof(Grenade)))
			return grenadeInventory.IndexOf(item as Grenade);
		if (typeof(T).IsEquivalentTo(typeof(Item)))
			return itemInventory.IndexOf(item as Item);
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
	public bool AddItemToInventory<T>(T item) where T : ItemInterface
	{
		try
		{
			if (item.GetType().IsEquivalentTo(typeof(Gun)))
				gunInventory.Add(item as Gun);
			if (item.GetType().IsEquivalentTo(typeof(Grenade)))
				grenadeInventory.Add(item as Grenade);
			if (item.GetType().IsEquivalentTo(typeof(Item)))
				itemInventory.Add(item as Item);
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
			currentGun.ActiveEffects.RemoveAll(bullet => bullet == null);
			yield return new WaitForSeconds(1);
		}
	}
	private IEnumerator CheckBiome()
	{
		for (; ; )
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
