using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldController : MonoBehaviour
{
    //instance du monde
    public static WorldController Instance { get; protected set; }

    //Le monde et les données des tiles
    public World World { get; protected set; }

    //Déclaration des tiles :)
    public Sprite defaultSprite;
    public Sprite waterSprite;
    public Sprite grassSprite;
    public Sprite wallSprite;

    Dictionary<Tile, GameObject> tileGameObjectMap;

    void Start()
    {
        Instance = this;
        //Hello World !
        World = new World();

        //Hello dictionnaire des liens gameobject <=> tile_data
        tileGameObjectMap = new Dictionary<Tile, GameObject>();
        for (int x = 0; x < World.Width; x++)
        {
            for (int y = 0; y < World.Height; y++)
            {
                //Récupération du tile_data
                Tile tile_data = World.GetTileAt(x, y);

                //Création du GameObject
                GameObject tile_go = new GameObject();

                //liaison via le dicitonnaire
                tileGameObjectMap.Add(tile_data, tile_go);

                //Initialisaiton du GameObject et de ses paramètres
                tile_go.name = "Tile_" + x + "_" + y;
                tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y);
                tile_go.transform.SetParent(this.transform, true);

                //Initialisation du SpriteRenderer et de la mer
                SpriteRenderer tile_sr = tile_go.AddComponent<SpriteRenderer>();
                tile_sr.sprite = defaultSprite;

                //Initialisation du callback pour update le gameobject quand le tile_type change
                tile_data.RegisterTileTypeChangedCallback( OnTileTypeChanged);
            }
        }        
    }

    void DestroyAllGameObjects()
    {
        while (tileGameObjectMap.Count > 0)
        {
            Tile tile_data = tileGameObjectMap.Keys.First();
            GameObject tile_go = tileGameObjectMap[tile_data];
            tileGameObjectMap.Remove(tile_data);
            tile_data.UnRegisterTileTypeChangedCallback(OnTileTypeChanged);
            Destroy(tile_go);
        }
    }

    //Callback quand changement de tile.type
    void OnTileTypeChanged(Tile tile_data)
    {
        GameObject tile_go = tileGameObjectMap[tile_data];
        if (tileGameObjectMap.ContainsKey(tile_data) == false) {
            Debug.LogError("tileGameObjectMap vide de tilde_data");
            return;
        }        
        if (tile_go == null)
        {
            Debug.LogError("tileGameObjectMap vide de tile_go");
            return;
        }        
        if (tile_data.Type == TileType.Water)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = waterSprite;
        }
        else if (tile_data.Type == TileType.Grass)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = grassSprite;
        }        
        else
        {
            Debug.LogError("OnTileTypeChanged - Unrecognized tile type.");
        }
    }

    //récupération du tile en fonction des coordonéees 
    public Tile GetTileAtWorldCoord(Vector3 coord)
    {
        int x = Mathf.FloorToInt(coord.x);
        int y = Mathf.FloorToInt(coord.y);
        return World.GetTileAt(x, y);
    }
}
