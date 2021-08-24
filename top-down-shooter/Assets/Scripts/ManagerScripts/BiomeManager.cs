using ProceduralNoiseProject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeManager : MonoBehaviour
{
	public float frequency = 1;
	public float amplitude = 1;

	private VoronoiNoise voronoi;
	void Start()
	{
		voronoi = new VoronoiNoise(GlobalVariables.seed, frequency, amplitude);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
