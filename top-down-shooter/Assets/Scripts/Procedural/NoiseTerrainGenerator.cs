using UnityEngine;

public class NoiseTerrainGenerator : MonoBehaviour {

	public int depth = 20;

	public int width = 256;
	public int height = 256;

	public float scale = 20f;

	public bool randomGeneration = false;

	public Vector2 offset; // default to like 100 I guess

	public bool debug = false;

	private void Start() {
		if (randomGeneration) {
			offset.x = Random.Range(0f, 9999f);
			offset.y = Random.Range(0f, 9999f);
		}
		generate();
	}

	private void Update() {
		if (debug)
			generate();
	}

	public void generate() {
		Terrain terrain = GetComponent<Terrain>();
		terrain.terrainData = GenerateTerrain(terrain.terrainData);
	}

	private TerrainData GenerateTerrain(TerrainData terrainData) {
		terrainData.heightmapResolution = width + 1;

		terrainData.size = new Vector3(width, depth, height);

		terrainData.SetHeights(0, 0, GenerateHeights());
		return terrainData;
	}

	private float[,] GenerateHeights() {
		float[,] heights = new float[width, height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < width; y++) {
				heights[x, y] = CalculateHeight(x, y);
			}
		}
		return heights;
	}

	private float CalculateHeight (int x, int y) {
		float xCord = (float) x / width * scale + offset.x;
		float yCord = (float) y / height * scale + offset.y;

		return Mathf.PerlinNoise(xCord, yCord);
	}
}
