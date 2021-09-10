using ProceduralNoiseProject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BiomeManager;

public class EnvironmentGenerationManager : MonoBehaviour
{
	public BiomeManager biomeManager;
	[Header("Texture Generation")]
	public GameObject groundPlane;
	public float simplexFrequency = 0.5f;
	public float simplexAmplitude = 1.2f;
	[ReadOnly] [SerializeField] private Vector2Int textureSize;
	[Header("Biome Gradients")]
	public Gradient planesGradient;
	public Gradient swampGradient;
	public Gradient forestGradient;
	public Gradient hellGradient;
	// Private variables
	private Renderer groundRenderer;
	private SimplexNoise simplexNoise;
	void Start()
	{
		// Set texture sizing
		textureSize = new Vector2Int((int)groundPlane.transform.localScale.x, (int)groundPlane.transform.localScale.z);
		groundRenderer = groundPlane.GetComponent<Renderer>();
		// Init noise generation
		simplexNoise = new SimplexNoise(GlobalVariables.seed, simplexFrequency, simplexAmplitude);
		groundRenderer.material.mainTexture = GenerateTexture();
	}
	private Texture2D GenerateTexture()
	{
		Texture2D texture = new Texture2D(textureSize.x, textureSize.y);

		for (int x = 0; x < textureSize.x; x++)
		{
			for (int y = 0; y < textureSize.y; y++)
			{
				texture.SetPixel(x, y, DetermineColor(x, y));
			}
		}
		texture.Apply();
		return texture;
	}
	private Color DetermineColor(int x, int y)
	{
		float sampleValue = simplexNoise.Sample2D(x, y);
		(float voroniSampleValue, Biome biome) = biomeManager.GetBiomeAt(new Vector3(x, 0, y));
		// Determine which color gradient to use
		switch (biome)
		{
			case Biome.Planes:
				return planesGradient.Evaluate(sampleValue);
			case Biome.Swamp:
				return swampGradient.Evaluate(sampleValue);
			case Biome.Forest:
				return forestGradient.Evaluate(sampleValue);
			case Biome.Hell:
				return hellGradient.Evaluate(sampleValue);
			case Biome.Void:
				return Color.black;
		}
		return Color.clear;
	}
	void Update()
	{
		
	}
}
