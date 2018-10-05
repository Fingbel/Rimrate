using System;
using UnityEngine;

public enum TileType { Water, Grass};

public class Tile
{
    //déclaration des différents type de tile
   

    private TileType _type = TileType.Grass;


    //getter et setter avec callback
    public TileType Type
    {
        get { return _type; }
        set
        {
            TileType oldType = _type;
            _type = value;
            //Callback avec vérification du changement de type de tile 

            if (cbTileTypeChanged != null && oldType != _type)
                cbTileTypeChanged(this);
        }
    }    
    public int X { get; protected set; }
    public int Y { get; protected set; }

    //définition d'une tile
    LooseObject looseObject;
    InstalledObject InstalledObject;
    World world;
    int layer = 1;


    //L'action appelé a chaque changement de type de tile
    Action<Tile> cbTileTypeChanged;

    //création de la tile
    public Tile(World world, int x, int y)
    {
        this.world = world;
        this.X = x;
        this.Y = y;
    }

    //ACTIONS
    public void RegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged += callback;
    }
    public void UnRegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged -= callback;
    }

    public bool PlaceObject(InstalledObject objInstance)
    {
        if(objInstance == null)
        {
            InstalledObject = null;
            return true;
        }
        
        if(objInstance != null)
        {
            Debug.LogError("Il y a deja un InstalledObject sur ce tile");
            return false;
        }

        InstalledObject = objInstance;
        return true;
    }
}
    
