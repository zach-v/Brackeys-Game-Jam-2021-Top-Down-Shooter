using Assets.Scripts.Components;
using ProceduralNoiseProject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
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
	[Header("Worley Noise")]
	public float wFrequency = 0.5f;
	public float wAmplitude = 1f;
	public float wScale = 1f;
	public Vector2 wOffset;
	[Header("Chunk Stuff")]
	[SerializeField] private Text ChunkStatusText;
	[SerializeField] private GameObject chunkQuadPrefab;
	[ReadOnly] public Vector2Int textureSize;
	[SerializeField] private Shader groundShader;
	[SerializeField] private Vector3 quadRotationOffset;
	private Dictionary<(int, int), GameObject> chunkMap;
	// Private variables
	private PerlinNoise pNoise;
	private WorleyNoise wNoise;
	void Awake()
	{
		// Initialize the chunk map
		chunkMap = new Dictionary<(int, int), GameObject>();
		// Initialize the noise generation
		pNoise = new PerlinNoise(GlobalVariables.seed, pFrequency, pAmplitude);
		wNoise = new WorleyNoise(GlobalVariables.seed, wFrequency, wAmplitude);
		// Set texture sizing
		textureSize = new Vector2Int((int)chunkQuadPrefab.transform.localScale.x, (int)chunkQuadPrefab.transform.localScale.y);
	}
	void Start()
	{
		// Set texture scale
		newMappedTextureRes = new Vector2Int((int)(textureSize.x * resolutionScale), (int)(textureSize.y * resolutionScale));
	}
	public IEnumerator RenderGroundTexture(GameObject quad)
	{
		Material groundMat = new Material(Shader.Find("Universal Render Pipeline/Simple Lit"));
		groundMat.mainTexture = GenerateTexture(quad.transform);
		quad.GetComponent<Renderer>().material = groundMat;
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
				float newX = Extensions.Map(x, 0, newMappedTextureRes.x, 0, textureSize.x);
				float newY = Extensions.Map(y, 0, newMappedTextureRes.y, 0, textureSize.y);
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
		Biome biome = biomeManager.GetBiomeAt(new Vector3(x, 0, y));
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
	public IEnumerator UpdateChunkMap((int, int)[] chunks, int scale)
	{
		int j = 0;
		// Remove the keys not in the list to render
		(int, int)[] chunksToRemove = chunkMap.Keys.Except(chunks).ToArray();
		foreach ((int, int) key in chunksToRemove)
		{
			try
			{
				Destroy(chunkMap[key]);
				chunkMap.Remove(key);
			}
			catch (Exception e)
			{
				Debug.Log($"Error while removing chunk: {e.Message}\nStuff can still probably run...");
			}
			// Update debug information
			ChunkStatusText.text = $"Chunk Status: Removing\n{j} out of {chunksToRemove.Length}";
			yield return null;
			j++;
		}
		int i = 0;
		// Search the list of chunks
		foreach ((int x, int z) chunk in chunks)
		{
			// If it's not in the list of chunks
			if (!chunkMap.ContainsKey(chunk))
			{
				// Change from chunk space to world space
				float anchorOffset = scale * 2.0f;
				float xPos = chunk.x * anchorOffset;
				float zPos = chunk.z * anchorOffset;
				Vector3 chunkPosition = new Vector3(xPos + anchorOffset, 0, zPos + anchorOffset);
				Quaternion rotation = Quaternion.Euler(transform.rotation.eulerAngles + quadRotationOffset);
				// Create new chunk
				GameObject newChunk = Instantiate(chunkQuadPrefab, chunkPosition, rotation, transform);
				StartCoroutine(RenderGroundTexture(newChunk));
				chunkMap.Add(chunk, newChunk);
			}
			// Update debug information
			ChunkStatusText.text = $"Chunk Status: Rendering\n{i} out of {chunks.Length}";
			yield return null;
			i++;
		}
		ChunkStatusText.text = $"Chunk Status: Ready!";
		yield break;
	}
}
