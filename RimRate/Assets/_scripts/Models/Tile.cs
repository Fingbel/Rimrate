using UnityEngine;
using System.Collections;
using System;

//TileType est le Type de la case 
public enum TileType { Empty, Floor, Water };

public class Tile
{
    private TileType _type = TileType.Empty;

    //Appel du callback quand on change le tile.TileType et getter classique
    public TileType Type
    {
        get { return _type; }
        set
        {
            TileType oldType = _type;
            _type = value;
            
            if (cbTileChanged != null && oldType != _type)
                cbTileChanged(this);
        }
    }

    //Initialisation des variables
    public Inventory inventory{ get; protected set; }
    public Furniture furniture { get; protected set; }
    public Job pendingFurnitureJob;
    public World world { get; protected set; }
    public int X { get; protected set; }
    public int Y { get; protected set; }

    // L'action qui est appelé par le callback quand le tileType a changé
    Action<Tile> cbTileChanged;

    //initialisation d'une nouvelle instance de tile
    public Tile(World world, int x, int y)
    {
        this.world = world;
        this.X = x;
        this.Y = y;
    }

    /// Enregistre la fonction a appeléquand le tileType a changé.

    public void RegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileChanged += callback;
    }


    /// Désenregistrement du callback
    public void UnregisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileChanged -= callback;
    }

    public bool PlaceFurniture(Furniture objInstance)
    {
        if (objInstance == null)
        {
            // On enleve ce qu'il y avait avant
            furniture = null;
            return true;
        }

        // objInstance n'est pas null
        if (furniture != null)
        {
            Debug.LogError("Trying to assign an furniture to a tile that already has one!");
            return false;
        }

        // Nous avons tout testé, plaçons l'instance de la furniture

        furniture = objInstance;
        return true;
    }

}
