﻿using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; protected set; }

    // The only tile sprite we have right now, so this
    // it a pretty simple way to handle it.
    public Sprite floorSprite; //FIXME
    public Sprite waterSprite; //FIXME
    public Sprite emptySprite; //FIXME


    Dictionary<Tile, GameObject> tileGameObjectMap;
    Dictionary<Furniture, GameObject> furnitureGameObjectMap;

    Dictionary<string, Sprite> furnitureSprites;
    // The world and tile data
    public World World { get; protected set; }
  
    // Use this for initialization
    void Start()
    {
        furnitureSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Furnitures/");
        Debug.Log("LOADED RESSOURCES:");
        foreach(Sprite s in sprites)
        {
            //Debug.Log(s);
            furnitureSprites[s.name] = s;
        }

        if (Instance != null)
        {
            Debug.LogError("There should never be two world controllers.");
        }
        Instance = this;

        // Create a world with Empty tiles
        World = new World();

        World.RegisterFurnitureCreated(OnFurnitureCreated);

        // Instantiate ours dictionary that tracks which GameObject is rendering which Tile data.
        tileGameObjectMap = new Dictionary<Tile, GameObject>();
        furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();

        // Create a GameObject for each of our tiles, so they show visually. (and redunt reduntantly)
        for (int x = 0; x < World.Width; x++)
        {
            for (int y = 0; y < World.Height; y++)
            {
                // Get the tile data
                Tile tile_data = World.GetTileAt(x, y);

                // This creates a new GameObject and adds it to our scene.
                GameObject tile_go = new GameObject();

                // Add our tile/GO pair to the dictionary.
                tileGameObjectMap.Add(tile_data, tile_go);

                tile_go.name = "Tile_" + x + "_" + y;
                tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
                tile_go.transform.SetParent(this.transform, true);


                tile_go.AddComponent<SpriteRenderer>().sprite = emptySprite;
                

                // Register our callback so that our GameObject gets updated whenever
                // the tile's type changes.
                tile_data.RegisterTileTypeChangedCallback(OnTileTypeChanged);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // THIS IS AN EXAMPLE -- NOT CURRENTLY USED
    void DestroyAllTileGameObjects()
    {
        // This function might get called when we are changing floors/levels.
        // We need to destroy all visual **GameObjects** -- but not the actual tile data!

        while (tileGameObjectMap.Count > 0)
        {
            Tile tile_data = tileGameObjectMap.Keys.First();
            GameObject tile_go = tileGameObjectMap[tile_data];

            // Remove the pair from the map
            tileGameObjectMap.Remove(tile_data);

            // Unregister the callback!
            tile_data.UnregisterTileTypeChangedCallback(OnTileTypeChanged);

            // Destroy the visual GameObject
            Destroy(tile_go);
        }

        // Presumably, after this function gets called, we'd be calling another
        // function to build all the GameObjects for the tiles on the new floor/level
    }

    // This function should be called automatically whenever a tile's type gets changed.
    void OnTileTypeChanged(Tile tile_data)
    {

        if (tileGameObjectMap.ContainsKey(tile_data) == false)
        {
            Debug.LogError("tileGameObjectMap doesn't contain the tile_data -- did you forget to add the tile to the dictionary? Or maybe forget to unregister a callback?");
            return;
        }

        GameObject tile_go = tileGameObjectMap[tile_data];

        if (tile_go == null)
        {
            Debug.LogError("tileGameObjectMap's returned GameObject is null -- did you forget to add the tile to the dictionary? Or maybe forget to unregister a callback?");
            return;
        }

        if (tile_data.Type == TileType.Grass)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
        }
        else if (tile_data.Type == TileType.Water)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = waterSprite;
        }
        else if(tile_data.Type == TileType.Empty)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = emptySprite;
        }
        else
        {
            Debug.LogError("OnTileTypeChanged - Unrecognized tile type.");
        }


    }

    /// <summary>
    /// Gets the tile at the unity-space coordinates
    /// </summary>
    /// <returns>The tile at world coordinate.</returns>
    /// <param name="coord">Unity World-Space coordinates.</param>
    public Tile GetTileAtWorldCoord(Vector3 coord)
    {
        int x = Mathf.FloorToInt(coord.x);
        int y = Mathf.FloorToInt(coord.y);

        return World.GetTileAt(x, y);
    }

    public void OnFurnitureCreated(Furniture furn)
    {
        //Debug.Log("OnFurnitureCreated");
        //create a visual GameObject linked to this data
        // This creates a new GameObject and adds it to our scene.
        GameObject furn_go = new GameObject();

        // Add our tile/GO pair to the dictionary.
        furnitureGameObjectMap.Add(furn, furn_go);

        furn_go.name = furn.objectType +"_"+ furn.tile.X +"_" + furn.tile.Y;
        furn_go.transform.position = new Vector3(furn.tile.X, furn.tile.Y, 0);
        furn_go.transform.SetParent(this.transform, true);


        furn_go.AddComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furn);

        // Register our callback so that our GameObject gets updated whenever
        // the objects's type changes.
        furn.RegisterOnChangedCallback(OnFurnitureChanged);
    }

    void OnFurnitureChanged(Furniture furn)
    {
        
        //Update des gfx des furnitures
        if (furnitureGameObjectMap.ContainsKey(furn) == false)
        {
            Debug.LogError("OnFurnitureChanged -- pas de key " + furn + "dans le dictionnary " + furnitureGameObjectMap);
            return;
        }
        GameObject furn_go = furnitureGameObjectMap[furn];
        furn_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furn);


    }

    Sprite GetSpriteForFurniture(Furniture obj){
        if (obj.linksToNeighbour == false)
        {
            return furnitureSprites[obj.objectType];
        }
        string spriteName = obj.objectType + "_";

        int x = obj.tile.X;
        int y = obj.tile.Y;
        //CHECK FOR NEIGBOUR HERE CLOCKWISE //FIXME: RENAME SPRITE
        Tile t;
        t = World.GetTileAt(x, y + 1);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType)
        {
            spriteName += "N";
        }
        t = World.GetTileAt(x+1, y);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType)
        {
            spriteName += "E";
        }
        t = World.GetTileAt(x, y - 1);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType)
        {

            spriteName += "S";
        }
        t = World.GetTileAt(x-1, y);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType)
        {
            spriteName += "W";
        }

        if(furnitureSprites.ContainsKey(spriteName) == false) {
            Debug.LogError("No sprite with that name" + spriteName);
            return null;
        }
        return furnitureSprites[spriteName];
    }

   
}
