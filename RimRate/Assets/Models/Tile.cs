using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tile
{
    //déclaration des différents type de tile
    public enum TileType { Water, Boat, Grass, Stone };

    private TileType _type = TileType.Water;
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
   

    //définition d'une tile
    LooseObject looseObject;
    InstalledObject installedObject;
    World world;
    public int X { get; protected set; }
    public int Y { get; protected set; }

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
}
    
