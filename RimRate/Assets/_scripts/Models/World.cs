﻿using UnityEngine;
using System.Collections.Generic;
using System;

public class World
{

    // A two-dimensional array to hold our tile data.
    Tile[,] tiles;

    Dictionary<string, Furniture> furniturePrototypes;

    // The tile width of the world.
    public int Width { get; protected set; }

    // The tile height of the world
    public int Height { get; protected set; }

    Action<Furniture> cbFurnitureCreated;
    Action<Tile> cbTileChanged;

    //FIXME : Most likely will be replaced with a dedicated class for managing job queueS
    public Queue<Job> jobQueue;

    /// <summary>
    /// Initializes a new instance of the <see cref="World"/> class.
    /// </summary>
    /// <param name="width">Width in tiles.</param>
    /// <param name="height">Height in tiles.</param>
    public World(int width = 100, int height = 100) //FIXME : Hardcorded
    {
        jobQueue = new Queue<Job>();
        Width = width;
        Height = height;

        tiles = new Tile[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                tiles[x, y] = new Tile(this, x, y);
                tiles[x, y].RegisterTileTypeChangedCallback(OntileChanged);
            }
        }

        Debug.Log("world created with " + (Width * Height) + " tiles.");

        CreateFurniturePrototypes();
    }

    //FIXME : Hardcoded
    void CreateFurniturePrototypes()
    {
        furniturePrototypes = new Dictionary<string, Furniture>();

        furniturePrototypes.Add("wall",
            Furniture.CreatePrototype(
                                "wall",
                                0,  // Impassable
                                1,  // Width
                                1,  // Height
                                true //link to neighbours
                            )
        );
    }

    /// <summary>
    /// Gets the tile data at x and y.
    /// </summary>
    /// <returns>The <see cref="Tile"/>.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    public Tile GetTileAt(int x, int y)
    {
        if (x > Width || x < 0 || y > Height || y < 0)
        {
            Debug.LogError("Tile (" + x + "," + y + ") is out of range.");
            return null;
        }
        return tiles[x, y];
    }

    public void PlaceFurniture(string objectType, Tile t)
    {
        if(furniturePrototypes.ContainsKey(objectType) == false)
        {
            Debug.LogError("No prototype");
            return;
        }
        Furniture obj = Furniture.PlaceInstance(furniturePrototypes[objectType], t);

        if (obj == null)
        {
            //Failed to place an object -- most likely there was already something there
            return;
        }
        if(cbFurnitureCreated != null)
        {
            cbFurnitureCreated(obj);
        }
    }

    public void RegisterFurnitureCreated(Action<Furniture> callbackfunc)
    {
        cbFurnitureCreated += callbackfunc;
    }
    public void UnregisterFurnitureCreated(Action<Furniture> callbackfunc)
    {
        cbFurnitureCreated -= callbackfunc;
    }

    public void RegisterTileChanged(Action<Tile> callbackfunc)
    {
        cbTileChanged += callbackfunc;
    }
    public void UnregisterTileChanged(Action<Tile> callbackfunc)
    {
        cbTileChanged -= callbackfunc;
    }

    void OntileChanged (Tile t)
    {
        if (cbTileChanged == null)
            return;
        cbTileChanged(t);
    }

    public bool IsFurniturePlacementValid (string furnitureType, Tile t)
    {
        return furniturePrototypes[furnitureType].IsValidPosition(t);
        
    }
}
