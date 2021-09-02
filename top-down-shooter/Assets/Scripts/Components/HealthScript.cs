using System;
using System.Collections;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
	public float StartHealth = 100;
	public float RegenRate = 0;
	[ReadOnly] public float CurrentHealth;
	public Action NoHealthAction;
	private void Start()
	{
		CurrentHealth = StartHealth;
		StartCoroutine(UpdateHealth());
	}
	private IEnumerator UpdateHealth()
	{
		for (; ; )
		{
			if (RegenRate != 0)
				CurrentHealth += RegenRate;
			if (CurrentHealth <= 0)
			{
				if (NoHealthAction != null)
				{
					NoHealthAction.Invoke();
				}
			}
			if (CurrentHealth >= StartHealth)
				CurrentHealth = StartHealth;
			yield return new WaitForSeconds(0.2f);
		}
	}
}
