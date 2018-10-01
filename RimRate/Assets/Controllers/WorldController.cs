using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; protected set; }

    //Le monde et les données des tiles
    public World World { get; protected set; }

    //Déclaration des tiles :)
    public Sprite waterSprite;
    public Sprite grassSprite;

    void Start()
    {
        Instance = this;
        //Hello World !
        World = new World();
        for (int x = 0; x < World.Width; x++)
        {
            for (int y = 0; y < World.Height; y++)
            {
                //Récupération du tile_data
                Tile tile_data = World.GetTileAt(x, y);

                //Création du GameObject
                GameObject tile_go = new GameObject();

                //Initialisaiton du GameObject et de ses paramètres
                tile_go.name = "Tile_" + x + "_" + y;
                tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y);
                tile_go.transform.SetParent(this.transform, true);

                //Initialisation du SpriteRenderer et de la mer
                SpriteRenderer tile_sr = tile_go.AddComponent<SpriteRenderer>();
                tile_sr.sprite = waterSprite;

                //Initialisation du callback + lambda
                tile_data.RegisterTileTypeChangedCallback( (tile) => { OnTileTypeChanged(tile, tile_go); } );
            }
        }
        
    }
    //execution du callback
    void OnTileTypeChanged(Tile tile_data, GameObject tile_go)
    {
        if (tile_data.Type == Tile.TileType.Water)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = waterSprite;
        }
        else if (tile_data.Type == Tile.TileType.Grass)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = grassSprite;
        }
        else
        {
            Debug.LogError("OnTileTypeChanged - Unrecognized tile type.");
        }
    }
}
