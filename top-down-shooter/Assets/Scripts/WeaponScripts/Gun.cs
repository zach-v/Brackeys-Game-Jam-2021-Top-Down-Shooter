using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gun : WeaponBase, IEquatable<Gun>
{
	public enum SpecialMethod
	{
		None, SillySound
	}
	[Header("Fire Parameters")]
	public float Damage = 1;
	public int ShotCount = 1;
	public float spread = 0.01f; // In percentage
	public bool SingleFire = false;
	public bool HitScan = true;
	public string FireSoundName = "pistol-1";
	[Header("Game Objects")]
	public GameObject WeaponModel;
	public Projectile projectile;
	public GameObject FireEffect;
	public GameObject TracerEffect;
	public float TracerEffectTime = 3;
	public GameObject Impact;
	[ReadOnly] public List<GameObject> ActiveBullets;
	[Header("Other")]
	public SpecialMethod method = SpecialMethod.None;
	public Gun()
	{
		ActiveBullets = new List<GameObject>();
	}
	public void Method(SpecialMethod method)
	{
		switch (method)
		{
			default:
				break;
		}
	}
	public bool Equals(Gun other)
	{
		return Name == other.Name;
	}
}
