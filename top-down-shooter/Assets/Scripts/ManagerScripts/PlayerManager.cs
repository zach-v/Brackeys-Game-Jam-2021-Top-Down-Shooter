using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BiomeManager;

public class PlayerManager : MonoBehaviour
{
	[Header("Manager Fields")]
	public PlayerMovement movement;
	public AudioManager audioManager;
	public ItemManager weaponManager;
	public BiomeManager biomeManager;
	[Header("Walking Sound Fields")]
	public float TimeToStepSoundJog = 0.9f;
	public float TimeToStepSoundRun = 0.5f;
	[Header("Item Fields")]
	public Transform GunPoint;
	[SerializeField] private Vector3 RotationOffset = new Vector3(-90, 90, 0);
	[ReadOnly] [SerializeField] private GameObject ItemInHand;
	[ReadOnly] [SerializeField] private Gun currentGun = null;
	[ReadOnly] [SerializeField] private List<Gun> currentGunInventory;
	[ReadOnly] [SerializeField] private Grenade currentGrenade;
	[ReadOnly] [SerializeField] private List<Grenade> currentGrenadeInventory;

	[Header("Other Readonlys")]
	[SerializeField] private Biome currentBiome = Biome.Planes;
	[ReadOnly] [SerializeField] private float currentTimeToSound = 0;
	[ReadOnly] [SerializeField] private int numberOfJogSounds = 0;
	[ReadOnly] [SerializeField] private int numberOfRunSounds = 0;

	public PlayerManager()
	{
		currentGunInventory = new List<Gun>();
		currentGrenadeInventory = new List<Grenade>();
	}
	void Awake()
	{
		// Set current gun to first one in list
		AddWeapon(weaponManager.GunList[0]);
		SetCurrentGun(0);
		// Populate random sound list
		foreach (Sound s in audioManager.sounds)
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
	private void SetCurrentGun(int index)
	{
		Destroy(ItemInHand);
		currentGun = currentGunInventory[index];
		ItemInHand = Instantiate(currentGun.WeaponModel, GunPoint.position, Quaternion.Euler(GunPoint.rotation.eulerAngles + RotationOffset), transform);
	}
	private int CurrentIndexOfType<T>() where T : WeaponBase
	{
		if (typeof(T).IsEquivalentTo(typeof(Gun)))
			return currentGunInventory.IndexOf(currentGun);
		if (typeof(T).IsEquivalentTo(typeof(Grenade)))
			return currentGrenadeInventory.IndexOf(currentGrenade);
		return -1;
	}
	public bool AddWeapon(WeaponBase weapon)
	{
		try
		{
			if (weapon.GetType().IsEquivalentTo(typeof(Gun)))
				currentGunInventory.Add(weapon as Gun);
			if (weapon.GetType().IsEquivalentTo(typeof(Grenade)))
				currentGrenadeInventory.Add(weapon as Grenade);
		}
		catch (System.Exception)
		{
			return false;
		}
		return true;
	}
	void FixedUpdate()
	{
		if (movement.movement.magnitude >= 0.1)
			currentTimeToSound += Time.fixedDeltaTime;
	}
	private void LateUpdate()
	{
		if (currentTimeToSound >= TimeToStepSoundJog)
		{
			audioManager.Play("Dirt_Jogging-" + Mathf.RoundToInt(Random.Range(1, numberOfJogSounds)));
			currentTimeToSound = 0;
		}
		currentBiome = biomeManager.GetBiomeAt(transform.position);
	}
}
