using UnityEngine;
using System.Collections.Generic;
using System;

public class World
{

    // A two-dimensional array to hold our tile data.
    Tile[,] tiles;

    Dictionary<string, InstalledObject> installedObjectPrototypes;

    // The tile width of the world.
    public int Width { get; protected set; }

    // The tile height of the world
    public int Height { get; protected set; }

    Action<InstalledObject> cbInstalledObjectCreated;

    /// <summary>
    /// Initializes a new instance of the <see cref="World"/> class.
    /// </summary>
    /// <param name="width">Width in tiles.</param>
    /// <param name="height">Height in tiles.</param>
    public World(int width = 100, int height = 100)
    {
        Width = width;
        Height = height;

        tiles = new Tile[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                tiles[x, y] = new Tile(this, x, y);
            }
        }

        Debug.Log("World created with " + (Width * Height) + " tiles.");

        CreateInstalledObjectPrototypes();
    }

    void CreateInstalledObjectPrototypes()
    {
        installedObjectPrototypes = new Dictionary<string, InstalledObject>();

        installedObjectPrototypes.Add("Wall",
            InstalledObject.CreatePrototype(
                                "Wall",
                                0,  // Impassable
                                1,  // Width
                                1  // Height
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

    public void PlaceInstalledObject(string objectType, Tile t)
    {
        if(installedObjectPrototypes.ContainsKey(objectType) == false)
        {
            Debug.LogError("No prototype");
            return;
        }
        InstalledObject obj = InstalledObject.PlaceInstance(installedObjectPrototypes[objectType], t);

        if (obj == null)
        {
            //Failed to place an object -- most likely there was already something there
            return;
        }
        if(cbInstalledObjectCreated != null)
        {
            cbInstalledObjectCreated(obj);
        }
    }

    public void RegisterInstalledObjectCreated(Action<InstalledObject> callbackfunc)
    {
        cbInstalledObjectCreated += callbackfunc;
    }
    public void UnregisterInstalledObjectCreated(Action<InstalledObject> callbackfunc)
    {
        cbInstalledObjectCreated -= callbackfunc;
    }
}
