using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : IEquatable<WeaponBase>, ItemInterface
{
	[Header("Weapon Base Variables")]
	public string Name;
	public LayerMask TargetLayer;
	public float UsageRate = 100; // ms
	public Vector3 PositionOffset;
	public Vector3 RotationOffset;
	[Range(0.01f,1.5f)]
	public float DropPercentage = 1;
	[ReadOnly] public bool RecentlyUsed = false;
	[ReadOnly] public float TimeSinceUsed = 0;

	public bool Equals(WeaponBase other)
	{
		return Name == other.Name;
	}

	public abstract void Method();
}
