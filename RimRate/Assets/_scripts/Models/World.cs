using System.Collections.Generic;
using UnityEngine;

public class World
{
    //définition du monde
    Tile[,] tiles;

    Dictionary<string, InstalledObject> installedObjectPrototypes;

    public int Width { get; protected set; }
    public int Height { get; protected set; }

    //Initialisation d'une nouvelle instance du monde
    public World(int width = 100, int height = 100)
    {
        Width = width;
        Height = height;

        tiles = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = new Tile(this, x, y);
            }
        }
        Debug.Log("Monde créer avec " + (width * height) + " tiles");
        CreateInstalledObjectPrototypes();
    }

    void CreateInstalledObjectPrototypes()
    {
        installedObjectPrototypes = new Dictionary<string, InstalledObject>();

        installedObjectPrototypes.Add("Wall", InstalledObject.CreatePrototype(
                                    "Wall",
                                    0,
                                    1,
                                    1
                                    )
        );
    }
    //FONCTION DE récupération des coordonnées de tile
    public Tile GetTileAt(int x, int y)
    {
        if (x > Width || x < 0 || y > Height || y < 0)
        {
            Debug.LogError("Tile (" + x + "," + y + ") hors limite.");
            return null;
        }
        return tiles[x,y];
    }

    public void PlaceInstalledObject(string objectType, Tile t)
    {
        Debug.Log("PlaceInstalledObject");
        if (installedObjectPrototypes.ContainsKey(objectType) == false)
        {
            Debug.LogError("Dictionary installedObjectPrototypes ne contient pas de proto pour la clé" + objectType);
            return;
        }
        InstalledObject.PlaceInstance(installedObjectPrototypes[objectType], t);
    }
}