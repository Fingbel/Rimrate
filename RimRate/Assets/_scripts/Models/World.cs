using UnityEngine;
using System.Collections.Generic;
using System;

public class World
{

    // Un tableau a deux dimensions pour contenir nos tiles
    Tile[,] tiles;
    List<Character> characters;

    Dictionary<string, Furniture> furniturePrototypes;

    //La largeur en tile
    public int Width { get; protected set; }

    //La hauteur en tile
    public int Height { get; protected set; }

    Action<Furniture> cbFurnitureCreated;
    Action<Tile> cbTileChanged;
    Action<Character> cbCharacterCreated;


    //FIXME : Most likely will be replaced with a dedicated class for managing job queueS
    public JobQueue jobQueue;

    /// Initialisation de la classe World
    public World(int width = 100, int height = 100) 
    {
        jobQueue = new JobQueue();
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
        characters = new List<Character>();
        
    }

    public void Update(float deltaTime)
    {
        foreach(Character c in characters)
        {
            c.Update(deltaTime);
        }
    }

    public Character CreateCharater(Tile t)
    {
        Character c = new Character(t);
        characters.Add(c);
        if(cbCharacterCreated != null)
            cbCharacterCreated(c);
        return c;

    }

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

    /// Fonction pour récupérer la tile aux coordonnées globale x et y
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
            return;
        }
        if(cbFurnitureCreated != null)
        {
            cbFurnitureCreated(obj);
        }
    }

    //Les fonctions d'enregistrement / désenregistrements

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

    public void RegisterCharacterCreated(Action<Character> callbackfunc)
    {
        cbCharacterCreated += callbackfunc;
    }
    public void UnregisterCharacterCreated(Action<Character> callbackfunc)
    {
        cbCharacterCreated -= callbackfunc;
    }
    

    void OntileChanged (Tile t)
    {
        if (cbTileChanged == null)
            return;
        cbTileChanged(t);
    }

    //fonctions de vérifications de la validité de l'emplacement
    public bool IsFurniturePlacementValid (string furnitureType, Tile t)
    {
        return furniturePrototypes[furnitureType].IsValidPosition(t);
        
    }

    public Furniture GetFurniturePrototype(string objectType)
    {
        if(furniturePrototypes.ContainsKey(objectType)== false)
        {
            Debug.LogError("No furniture with type : " + objectType);
        }

        return furniturePrototypes[objectType];
    }
}
