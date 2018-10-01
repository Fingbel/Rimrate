using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public Sprite waterSprite;

    World world;

    void Start()
    {
        //Hello World !
        world = new World();
        for (int x = 0; x < world.Width; x++)
        {
            for (int y = 0; y < world.Height; y++)
            {

                //Association du tile_data au bon tile
                Tile tile_data = world.GetTileAt(x, y);

                //Création du GameObject
                GameObject tile_go = new GameObject();

                //Initialisaiton du GameObject
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

    void Update()
    {

    }

    //execution du callback
    void OnTileTypeChanged(Tile tile_data, GameObject tile_go)
    {
        if (tile_data.Type == Tile.TileType.Water)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = waterSprite;
        }
        else if (tile_data.Type == Tile.TileType.Boat)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = null;
        }
        else
        {
            Debug.LogError("OnTileTypeChnaged - Unrecognized tile type.");
        }
    }
}
