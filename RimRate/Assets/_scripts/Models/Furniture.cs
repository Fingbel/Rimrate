using UnityEngine;
using System.Collections;
using System;

// Furniture are things like walls, doors, and furniture (e.g. a sofa)

public class Furniture
{

    public Tile tile { get; protected set; }
    public string objectType { get; protected set; }
    float movementCost;
    int width;
    int height;
    public bool linksToNeighbour { get; protected set; }
    Action<Furniture> cbOnChanged;
    Func<Tile, bool> funcPositionValidation;

    // TODO: Implement larger objects
    // TODO: Implement object rotation

    protected Furniture()
    {

    }

    static public Furniture CreatePrototype(string objectType, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbour = false)
    {
        Furniture obj = new Furniture();

        obj.objectType = objectType;
        obj.movementCost = movementCost;
        obj.width = width;
        obj.height = height;
        obj.linksToNeighbour = linksToNeighbour;
        obj.funcPositionValidation = obj.__IsValidPosition;
        return obj;
    }

    static public Furniture PlaceInstance(Furniture proto, Tile tile)
    {
        if (proto.funcPositionValidation(tile) == false)
        {
            Debug.LogError("PlaceInstance -- position validation fonction return FALSE");
            return null;
        }

        Furniture obj = new Furniture();

        obj.objectType = proto.objectType;
        obj.movementCost = proto.movementCost;
        obj.width = proto.width;
        obj.height = proto.height;
        obj.linksToNeighbour = proto.linksToNeighbour;
        obj.tile = tile;

        // FIXME: Nous ne gerons que les furniture de taille 1x1
        if (tile.PlaceFurniture(obj) == false)
        {
            return null;
        }
        if (obj.linksToNeighbour) //cette furniture est lié a ses voisins
        {            
            Tile t;
            int x = tile.X;
            int y = tile.Y;
            t = tile.world.GetTileAt(x, y + 1);
            if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType)
            {
                t.furniture.cbOnChanged(t.furniture);
            }
            t = tile.world.GetTileAt(x + 1, y);
            if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType)
            {
                t.furniture.cbOnChanged(t.furniture);
            }
            t = tile.world.GetTileAt(x, y - 1);
            if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType)
            {

                t.furniture.cbOnChanged(t.furniture);
            }
            t = tile.world.GetTileAt(x - 1, y);
            if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType)
            {
                t.furniture.cbOnChanged(t.furniture);
            }

        }
        return obj;
    }

    //Les fonctions d'enregistrement / désenregistrements

    public void RegisterOnChangedCallback(Action<Furniture> callbackFunc)
    {
        cbOnChanged += callbackFunc;
    }

    public void UnegisterOnChangedCallback(Action<Furniture> callbackFunc)
    {
        cbOnChanged -= callbackFunc;
    }
   

    //FIXME : NE DEVRAIT PAS ETRE PUBLIC
    public bool __IsValidPosition(Tile t)
    {
        if (t.Type != TileType.Floor)
        {
            return false;
        }
        if (t.furniture != null)
        {
            return false;
        }
        return true;
    }

    //fonction de vérifications de la validité de l'emplacement
    public bool IsValidPosition(Tile t)
    {
        return funcPositionValidation(t);
    }

    //fonctions de vérifications de la validité de l'emplacement pour les portes
    public bool IsValidPosition_Door(Tile t)
    {
        //assurons nous de la validité de base
        if (__IsValidPosition(t) == false)
        {
            return false;
        }

         //TODO : s'assurer qu'il y a un mur en N/S ou en E/W
        return true;
    }
}
