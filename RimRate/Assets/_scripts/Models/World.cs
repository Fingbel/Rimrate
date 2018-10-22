using UnityEngine;
using System.Collections.Generic;
using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;


public class World : IXmlSerializable
{


    // Un tableau a deux dimensions pour contenir nos tiles
    Tile[,] tiles;
    public List<Character> characters;
    public List<Furniture> furnitures;

    //Le graphe de pathfinding
    public Path_TileGraph tileGraph;

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
    public World(int width, int height)
    {
        SetupWorld(width, height);

        Character c = CreateCharater(GetTileAt(Width / 2, Height / 2));
    }

    void SetupWorld(int width, int height)
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
        furnitures = new List<Furniture>();
    }

    public void Update(float deltaTime)
    {
        foreach (Character c in characters)
        {
            c.Update(deltaTime);
        }

        foreach (Furniture f in furnitures)
        {
            f.Update(deltaTime);
        }
    }

    public Character CreateCharater(Tile t)
    {
        Character c = new Character(t);
        characters.Add(c);
        if (cbCharacterCreated != null)
            cbCharacterCreated(c);
        return c;

    }

    void CreateFurniturePrototypes()
    {
        furniturePrototypes = new Dictionary<string, Furniture>();

        furniturePrototypes.Add("wall",
            new Furniture(
                                "wall",
                                0,  // Impassable
                                1,  // Width
                                1,  // Height
                                true //link to neighbours
                            )
        );

        furniturePrototypes.Add("door",
            new Furniture(
                                "door",
                                0,  // Impassable
                                1,  // Width
                                1,  // Height
                                true //link to neighbours
                            )
        );
        furniturePrototypes["door"].furnParameters["openess"] = 0;
        furniturePrototypes["door"].updateActions += FurnitureActions.Door_UpdateAction;
    }

    /// Fonction pour récupérer la tile aux coordonnées globale x et y
    public Tile GetTileAt(int x, int y)
    {
        if (x >= Width || x < 0 || y >= Height || y < 0)
        {
            //Debug.LogError("Tile (" + x + "," + y + ") is out of range.");
            return null;
        }
        return tiles[x, y];
    }

    public Furniture PlaceFurniture(string objectType, Tile t)
    {
        if (furniturePrototypes.ContainsKey(objectType) == false)
        {
            Debug.LogError("No prototype");
            return null;
        }
        Furniture furn = Furniture.PlaceInstance(furniturePrototypes[objectType], t);

        if (furn == null)
        {
            return null;
        }

        furnitures.Add(furn);

        if (cbFurnitureCreated != null)
        {
            cbFurnitureCreated(furn);
            InvalidateTileGraph();

        }
        return furn;
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

    //Appeler quand n'importe quel tile change
    void OntileChanged(Tile t)
    {
        if (cbTileChanged == null)
            return;
        cbTileChanged(t);
        InvalidateTileGraph();
    }

    //appeler pour détruire le graphe de navigation (changement de tile/furniture/etc)
    public void InvalidateTileGraph()
    {
        tileGraph = null;
    }

    //fonctions de vérifications de la validité de l'emplacement
    public bool IsFurniturePlacementValid(string furnitureType, Tile t)
    {
        return furniturePrototypes[furnitureType].IsValidPosition(t);

    }

    public Furniture GetFurniturePrototype(string objectType)
    {
        if (furniturePrototypes.ContainsKey(objectType) == false)
        {
            Debug.LogError("No furniture with type : " + objectType);
        }

        return furniturePrototypes[objectType];
    }

    public void SetupPathFindingDebug()
    {
        int l = (Width / 2) - 5;
        int b = (Height / 2) - 5;
        for (int x = l - 5; x < l + 15; x++)
        {
            for (int y = b - 5; y < b + 15; y++)
            {
                tiles[x, y].Type = TileType.Floor;

                if (x == l || x == (l + 9) || y == b || y == (b + 9))
                {
                    if (x != (l + 9) && y != (b + 4))
                    {
                        PlaceFurniture("wall", tiles[x, y]);
                    }
                }
            }
        }
    }


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///
    ///                                 SAVING & LOADING
    /// 
    /// 
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public World()
    {


    }

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void WriteXml(XmlWriter writer)
    {

        writer.WriteAttributeString("Width", Width.ToString());
        writer.WriteAttributeString("Height", Width.ToString());

        writer.WriteStartElement("Tiles");
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                writer.WriteStartElement("Tile");
                tiles[x, y].WriteXml(writer);
                writer.WriteEndElement();
            }
        }
        writer.WriteEndElement();

        writer.WriteStartElement("Furnitures");
        foreach (Furniture furn in furnitures)
        {
            writer.WriteStartElement("Furniture");
            furn.WriteXml(writer);
            writer.WriteEndElement();
        }
        writer.WriteEndElement();

        writer.WriteStartElement("Characters");
        foreach (Character c in characters)
        {
            writer.WriteStartElement("Character");
            c.WriteXml(writer);
            writer.WriteEndElement();
        }
        writer.WriteEndElement();
    }

    

    public void ReadXml(XmlReader reader)
    {
        Debug.Log("ReadXml");

        Width = int.Parse(reader.GetAttribute("Width"));
        Height = int.Parse(reader.GetAttribute("Height"));

        SetupWorld(Width, Height);

        while (reader.Read())
        {
            switch (reader.Name)
            {
                case "Tiles":
                    ReadXml_Tiles(reader);
                    break;

                case "Furnitures":
                    ReadXml_Furnitures(reader);
                    break;

                case "Characters":
                    ReadXml_Characters(reader);
                    break;

            }
        }

        
    }

    void ReadXml_Tiles(XmlReader reader)
    {
        while (reader.Read())
        {
            if(reader.Name != "Tile")
            {
                return;
            }
            int x = int.Parse(reader.GetAttribute("X"));
            int y = int.Parse(reader.GetAttribute("Y"));
            tiles[x, y].ReadXml(reader);
        }
    }

    void ReadXml_Furnitures(XmlReader reader)
    {
        while (reader.Read())
        {
            if (reader.Name != "Furniture")
            {
                return;
            }
            int x = int.Parse(reader.GetAttribute("X"));
            int y = int.Parse(reader.GetAttribute("Y"));
            
            Furniture furn = PlaceFurniture(reader.GetAttribute("objectType"),tiles[x,y]);
            furn.ReadXml(reader);
        }
    }

    void ReadXml_Characters(XmlReader reader)
    {
        while (reader.Read())
        {
            if (reader.Name != "Character")
            {
                return;
            }
            int x = int.Parse(reader.GetAttribute("X"));
            int y = int.Parse(reader.GetAttribute("Y"));

            Character c = CreateCharater(tiles[x,y]);
            c.ReadXml(reader);
        }
    }
}
