using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase
{
	[Header("Weapon Base Variables")]
	public string Name;
	public LayerMask TargetLayer;
	public float UsageRate = 100; // ms
	[ReadOnly] public bool RecentlyUsed = false;
	[ReadOnly] public float TimeSinceUsed = 0;
}
