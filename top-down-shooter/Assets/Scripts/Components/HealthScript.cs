using System;
using System.Collections;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
	public float StartHealth = 100;
	public float RegenRate = 0;
	[ReadOnly] [SerializeField] private float currentHealth;
	public Action noHealthAction;
	public Action changeHealthAction;
	private void Start()
	{
		currentHealth = StartHealth;
		StartCoroutine(UpdateHealth());
	}
	public void ChangeHealth(float amount)
	{
		if (changeHealthAction != null)
		{
			changeHealthAction.Invoke();
		}
		currentHealth += amount;
	}
	private IEnumerator UpdateHealth()
	{
		for (; ; )
		{
			if (RegenRate != 0)
				currentHealth += RegenRate;
			if (currentHealth <= 0)
			{
				if (noHealthAction != null)
				{
					noHealthAction.Invoke();
				}
			}
			if (currentHealth >= StartHealth)
				currentHealth = StartHealth;
			yield return new WaitForSeconds(0.2f);
		}
	}
}
