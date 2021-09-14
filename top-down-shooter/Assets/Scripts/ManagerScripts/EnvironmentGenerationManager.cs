using Assets.Scripts.Components;
using ProceduralNoiseProject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BiomeManager;

public class EnvironmentGenerationManager : MonoBehaviour
{
	[Header("Texture setting")]
	[SerializeField] private BiomeManager biomeManager;
	[SerializeField] private float resolutionScale = 3;
	[ReadOnly] [SerializeField] private Vector2Int newMappedTextureRes;
	[Header("Biome Gradients")]
	[SerializeField] private Gradient planesGradient;
	[SerializeField] private Gradient swampGradient;
	[SerializeField] private Gradient forestGradient;
	[SerializeField] private Gradient hellGradient;
	[Header("Noise Settings")]
	public float pFrequency = 0.5f;
	public float pAmplitude = 1f;
	public float pScale = 1f;
	public Vector2 pOffset;
	// Private variables
	private Renderer groundRenderer;
	private PerlinNoise pNoise;
	void Awake()
	{
		pNoise = new PerlinNoise(GlobalVariables.seed, pFrequency, pAmplitude);
	}
	void Start()
	{
		// Set texture scale
		newMappedTextureRes = new Vector2Int((int) (biomeManager.textureSize.x * resolutionScale), (int) (biomeManager.textureSize.y * resolutionScale));
		// Set renderer
		groundRenderer = biomeManager.groundPlane.GetComponent<Renderer>();
		StartCoroutine(RenderGroundTexture(groundRenderer));
	}
	public IEnumerator RenderGroundTexture(Renderer ground)
	{
		yield return new WaitForFixedUpdate();
		ground.material.mainTexture = GenerateTexture(biomeManager.groundPlane.transform);
		yield break;
	}
	private Texture2D GenerateTexture(Transform objectTransform)
	{
		// Create a new texture to the scale of the quad
		Texture2D texture = new Texture2D(newMappedTextureRes.x, newMappedTextureRes.y);
		// Loop through the texture size
		for (int x = 0; x < newMappedTextureRes.x; x++)
		{
			for (int y = 0; y < newMappedTextureRes.y; y++)
			{
				float newX = Extensions.Map(x, 0, newMappedTextureRes.x, 0, biomeManager.textureSize.x);
				float newY = Extensions.Map(y, 0, newMappedTextureRes.y, 0, biomeManager.textureSize.y);
				// Get color from the biome manager
				texture.SetPixel(x, y, DetermineColor(newX + (objectTransform.position.x - (objectTransform.localScale.x / 2)),
					newY + (objectTransform.position.z - (objectTransform.localScale.y / 2))));
			}
		}
		texture.Apply();
		return texture;
	}
	private Color DetermineColor(float x, float y)
	{
		// Get the noise sample and biome tuple
		(float returnedSample, Biome biome) = biomeManager.GetBiomeAt(new Vector3(x, 0, y));
		float perlinNoiseSample = pNoise.Sample2D(x, y);
		// Determine which color gradient to use
		switch (biome)
		{
			case Biome.Planes:
				return planesGradient.Evaluate(perlinNoiseSample.Map(-pAmplitude, pAmplitude, 0, 1));
			case Biome.Swamp:
				return swampGradient.Evaluate(perlinNoiseSample.Map(-pAmplitude, pAmplitude, 0, 1));
			case Biome.Forest:
				return forestGradient.Evaluate(perlinNoiseSample.Map(-pAmplitude, pAmplitude, 0, 1));
			case Biome.Hell:
				return hellGradient.Evaluate(perlinNoiseSample.Map(-pAmplitude, pAmplitude, 0, 1));
			case Biome.Void:
				return Color.black;
		}
		return Color.clear;
	}
	void Update()
	{

	}
}
