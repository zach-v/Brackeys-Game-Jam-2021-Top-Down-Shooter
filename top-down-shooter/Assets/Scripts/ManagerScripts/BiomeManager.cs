using ProceduralNoiseProject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BiomeManager : MonoBehaviour
{
	public enum Biome
	{
		Void, Planes, Swamp, Forest, Hell
	}
	[Header("Voronoi Noise")]
	public float vFrequency = 0.02f;
	public float vAmplitude = 5f;
	public float vScale = 0.1f;
	public Vector2 vOffset;
	[Header("Perlin Noise")]
	public float pFrequency = 0.01f;
	public float pAmplitude = 1f;
	public float pScale = 0.1f;
	public Vector2 pOffset;
	[Header("Other variables")]
	public float biomeVariation = 1;
	[Header("Debuging variables")]
	[SerializeField] private bool drawDebug = true;
	[SerializeField] private bool quickRefresh = true;
	[SerializeField] private int gridDrawSize = 100;
	[SerializeField] private float sphereRadius = 0.25f;
	[ReadOnly] [SerializeField] private float maxNoiseValue = 0;
	[ReadOnly] [SerializeField] private float minNoiseValue = 0;
	// Procedural generators
	private VoronoiNoise voronoi;
	private PerlinNoise noise;
	void Awake()
	{
		UnityEngine.Random.InitState(GlobalVariables.seed);
		// regionCount = Enum.GetNames(typeof(Biome)).Length;
		voronoi = new VoronoiNoise(GlobalVariables.seed, vFrequency, vAmplitude);
		noise = new PerlinNoise(GlobalVariables.seed, pFrequency, pAmplitude);
	}

	public (float, Biome) GetBiomeAt(Vector3 position)
	{
		float sampleValue = voronoi.Sample2D((position.x + vOffset.x) * vScale, (position.z + vOffset.y) * vScale);
		// track the scale of the map
		if (maxNoiseValue < sampleValue)
			maxNoiseValue = sampleValue;
		if (minNoiseValue > sampleValue)
			minNoiseValue = sampleValue;
		// determine biome
		float biomeValue = noise.Sample2D((position.x + pOffset.x) * pScale, (position.z + pOffset.y) * pScale);
		float combined = biomeValue + sampleValue;
		if (combined > 1f)
			return (combined, Biome.Forest);
		if (combined > 0.75f)
			return (combined, Biome.Swamp);
		if (combined > 0.15f)
			return (combined, Biome.Planes);
		if (combined >= 0)
			return (combined, Biome.Hell);
		return (combined, Biome.Planes);
	}
	public void OnDrawGizmosSelected()
	{
		if (quickRefresh)
		{
			voronoi = new VoronoiNoise(GlobalVariables.seed, vFrequency, vAmplitude);
			maxNoiseValue = 0;
			minNoiseValue = 0;
			quickRefresh = false;
		}
		if (drawDebug)
		{
			try
			{
				for (int x = 0; x < gridDrawSize; x++)
				{
					for (int z = 0; z < gridDrawSize; z++)
					{
						float sampleValue = voronoi.Sample2D((x + vOffset.x) * vScale, (z + vOffset.y) * vScale);
						Gizmos.color = new Color(sampleValue, sampleValue, sampleValue);
						Gizmos.DrawSphere(new Vector3(x, transform.position.y, z), sphereRadius);
						if (maxNoiseValue < sampleValue)
							maxNoiseValue = sampleValue;
						if (minNoiseValue > sampleValue)
							minNoiseValue = sampleValue;
					}
				}
			}
			catch (Exception)
			{
				// do nothing
			}
		}
	}
}
