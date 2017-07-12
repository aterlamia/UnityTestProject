using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileMap : MonoBehaviour
{

	public GameObject tilePrefab;
	public Sprite[] _sprites;
	public Sprite[][] _textures;

	private int wallTypes = 8;
	public string seed;
	public string materialSeed;
	public int columns;
	public int rows;

	[Range (0, 100)]
	public int randomFillPercent;

	[Range (0, 100)]
	public int randomFillPercentMaterials;

	[Range (0, 100)]
	public int smooth;


	private int[,] map;


	private Dictionary<Tile, GameObject> tileToGameObjectMap;
	private Dictionary<GameObject, Tile> gameObjectToTileMap;


	void setMaterials (int type)
	{
		
		string localSeed = materialSeed + "+Hallo_\t" + type;
		int[,] materialMap = new int[columns, rows];
		System.Random pseudoRandom = new System.Random (localSeed.GetHashCode ());
		for (int x = 0; x < columns; x++) {
			for (int y = 0; y < rows; y++) {
				int rand = pseudoRandom.Next (0, 100);
				if (rand < randomFillPercentMaterials) {
					materialMap [x, y] = 1;
				} else {
					materialMap [x, y] = 0;
				}
			}
		}


		for (int i = 0; i < smooth; i++) {
			materialMap = SmoothMap (materialMap);
		}

		for (int x = 0; x < columns; x++) {
			for (int y = 0; y < rows; y++) {
				int rand = pseudoRandom.Next (0, 100);
				if (materialMap [x, y] == 0 && map [x, y] > 0) {
					map [x, y] = type;
				}
			}
		}

	}

	int GetSurroundingWallCount (int[,] surMap, int gridX, int gridY)
	{
		int[] types = new int[wallTypes];
		for (int x = 0; x < wallTypes; x++) {
			types [x] = 0;
		}

		int wallCount = 0;
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++) {
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++) {
				if (neighbourX >= 0 && neighbourX < columns && neighbourY >= 0 && neighbourY < rows) {
					types [surMap [neighbourX, neighbourY]]++;
				}
			}
		}

		int lastEnough = 0;
		System.Random pseudoRandom = new System.Random (materialSeed.GetHashCode ());
		for (int x = 0; x < wallTypes; x++) {
			
			if (types [x] > 3) {
				int rand = pseudoRandom.Next (0, 100);

				lastEnough = x;
				if (rand < randomFillPercent) {
					return x;
				}
			}
		}
		return surMap [gridX, gridY];
	}

	int[,] SmoothMap (int[,] smoothMap)
	{
		for (int x = 0; x < columns; x++) {
			for (int y = 0; y < rows; y++) {
				int surroundingWalls = GetSurroundingWallCount (smoothMap, x, y);
				smoothMap [x, y] = surroundingWalls;
			}
		}

		return smoothMap;
	}

	void DrawMap ()
	{
		TileFactory tileFactory = new TileFactory ();


		for (int x = 0; x < columns; x++) {
			for (int y = 0; y < rows; y++) {
				Tile tile = tileFactory.createTile ((TileType)map [x, y], x, y);
				GameObject tileObj = Instantiate (tilePrefab, new Vector3 (x, y, 0), Quaternion.identity, this.transform) as GameObject;
				SpriteRenderer renderer = tileObj.GetComponent<SpriteRenderer> ();

				tileToGameObjectMap [tile] = tileObj;
				gameObjectToTileMap [tileObj] = tile;
				renderer.sprite = _sprites [map [x, y]];
			}
		}	
	}

	private void generatePerlinMap ()
	{
		tileToGameObjectMap = new Dictionary<Tile, GameObject> ();
		gameObjectToTileMap = new Dictionary<GameObject, Tile> ();

		float noiseResolution = 0.01f;
		float scaler = 0.15f;
		map = new int[columns, rows];
		System.Random pseudoRandom = new System.Random (seed.GetHashCode ());
		Vector2 noiseOffset = new Vector2 ((float)pseudoRandom.NextDouble (), (float)pseudoRandom.NextDouble ()); 
		for (int x = 0; x < columns; x++) {
			for (int y = 0; y < rows; y++) {
				float value = pseudoRandom.Next (10, 15) * Mathf.PerlinNoise ((x * scaler) + noiseOffset.x, (y * scaler) + noiseOffset.y);
				map [x, y] = Mathf.Abs (((int)value) % wallTypes);
			}
		}

		for (int i = 0; i < smooth; i++) {
			map = SmoothMap (map);
		}
	}

	private void generateCellMap ()
	{
		map = new int[columns, rows];
		System.Random pseudoRandom = new System.Random (seed.GetHashCode ());
		for (int x = 0; x < columns; x++) {
			for (int y = 0; y < rows; y++) {
				int rand = pseudoRandom.Next (0, 100);

				if (x == 0 || x == columns - 1 || y == 0 || y == rows - 1) {
					map [x, y] = 1;
				} else {
					if (rand < randomFillPercent) {
						map [x, y] = 1;
					} else {
						map [x, y] = 0;
					}

				}
			}
		}

		for (int i = 0; i < 1; i++) {
			map = SmoothMap (map);
		}

		setMaterials (2);
		setMaterials (3);
		setMaterials (4);
		setMaterials (5);
		setMaterials (6);
		setMaterials (7);
	}

	// Use this for initialization
	void Start ()
	{
		_textures = new Sprite[10][];
		_sprites = Resources.LoadAll <Sprite> ("tilesground");  
		_textures [0] = Resources.LoadAll <Sprite> ("ground");  
		// generateCellMap ();

		generatePerlinMap ();

		DrawMap ();
	}

	// Update is called once per frame
	void Update ()
	{

	}
}
