using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseMap : MonoBehaviour
{
    Dictionary<int, GameObject> tileset;
    Dictionary<int, GameObject> tile_groups;
    public GameObject grass_prefab;
    public GameObject dirt_prefab;
    public GameObject water_prefab;

    int map_width = 150;
    int map_height = 150;

    List<List<int>> noise_grid = new List<List<int>>();
    List<List<GameObject>> tile_grid = new List<List<GameObject>>();

    // Magnificantion changes the frequency of all terrain recommend 4 - 20
    float magnification = 14.0f;

    //offset changes the starting point of the map
    //These are fixed value offsets
    //int x_offset = 0; // <- +>
    //int y_offset = 0; // v- +^

    //random offset can add more randomness to the generated map
    //These are random value of offsets
    int randomOffsetX;
    int rabdomOffsetY;


    void Start()
    {
        System.Random xrandom = new System.Random(2000);
        randomOffsetX = xrandom.Next(0, 500);
        rabdomOffsetY = xrandom.Next(0, 500);
        Debug.Log(randomOffsetX);
        Debug.Log(rabdomOffsetY);
        CreateTileset();
        CreateTileGroup();
        GenerateMap();
    }

    //This is where you change the code if you want to change the frequency of certain terrain
    //also remembers to change scale_perlin == values in the if loop in the GetIdUsingPerlin method
    void CreateTileset()
    {
        tileset = new Dictionary<int, GameObject>();
        tileset.Add(0, grass_prefab);
        tileset.Add(1, grass_prefab);
        tileset.Add(2, dirt_prefab);
        tileset.Add(3, dirt_prefab);
        tileset.Add(4, dirt_prefab);
        tileset.Add(5, water_prefab);
    }

    void CreateTileGroup()
    {
        tile_groups = new Dictionary<int, GameObject>();
        foreach (KeyValuePair<int, GameObject> prefab_pair in tileset)
        {
            GameObject tile_group = new GameObject(prefab_pair.Value.name);
            tile_group.transform.parent = gameObject.transform;
            tile_group.transform.localPosition = new Vector3(0, 0, 0);
            tile_groups.Add(prefab_pair.Key, tile_group);
        }
    }

    void GenerateMap()
    {
        for (int x = 0; x < map_width; x++)
        {
            noise_grid.Add(new List<int>());
            tile_grid.Add(new List<GameObject>());

            for (int y = 0; y < map_height; y++)
            {
                int tile_id = GetIdUsingPerlin(x, y);
                noise_grid[x].Add(tile_id);
                CreateTile(tile_id, x, y);
            }
        }
    }

    int GetIdUsingPerlin(int x, int y)
    {
        float raw_perlin = Mathf.PerlinNoise(
            ((x + randomOffsetX) / magnification),
            ((y + rabdomOffsetY) / magnification)
            );

        float clamp_perlin = Mathf.Clamp(raw_perlin, 0.0f, 1.0f);
        float scale_perlin = clamp_perlin * tileset.Count;

        if (scale_perlin == 6)
        {
            scale_perlin = 5;
        }

        return Mathf.FloorToInt(scale_perlin);
    }

    void CreateTile(int tile_id, int x, int y)
    {
        GameObject tile_prefab = tileset[tile_id];
        GameObject tile_group = tile_groups[tile_id];
        GameObject tile = Instantiate(tile_prefab, tile_group.transform);

        tile.name = string.Format("tile_x{0}_y{1}", x, y);
        tile.transform.localPosition = new Vector3(x, y, 0);

        tile_grid[x].Add(tile);
    }

    GameObject GetTile(int x, int y)
    {
        return tile_grid[x][y];
    }
}
