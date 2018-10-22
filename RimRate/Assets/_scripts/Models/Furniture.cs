using UnityEngine;
using System.Collections.Generic;
using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

// Furniture are things like walls, doors, and furniture (e.g. a sofa)

public class Furniture : IXmlSerializable
{
    public Dictionary<string, object> furnParameters;
    public Action<Furniture, float> updateActions;

    public void Update(float deltaTime)
    {
        if(updateActions != null)
        {
            updateActions(this,deltaTime);
        }
    }

    public Tile tile { get; protected set; }
    public string objectType { get; protected set; }
    public float movementCost { get; protected set; }
    int width;
    int height;
    public bool linksToNeighbour { get; protected set; }
    Action<Furniture> cbOnChanged;
    Func<Tile, bool> funcPositionValidation;

    // TODO: Implement larger objects
    // TODO: Implement object rotation


    //empty constructor for serialization
    public Furniture()
    {
        furnParameters = new Dictionary<string, object>();

    }

    //Copy constructor
    protected Furniture(Furniture other)
    {
        this.objectType = other.objectType;
        this.movementCost = other.movementCost;
        this.width = other.width;
        this.height = other.height;
        this.linksToNeighbour = other.linksToNeighbour;
        this.furnParameters = new Dictionary<string, object>(other.furnParameters);
        if(updateActions != null)
        {
            this.updateActions = (Action<Furniture, float>)other.updateActions.Clone();
        }
        
    }

    virtual public Furniture Clone()
    {
        return new Furniture(this);
    }

    //Prototype Constructor -- 
    public Furniture (string objectType, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbour = false)
    {      
        this.objectType = objectType;
        this.movementCost = movementCost;
        this.width = width;
        this.height = height;
        this.linksToNeighbour = linksToNeighbour;
        this.funcPositionValidation = this.__IsValidPosition;
        furnParameters = new Dictionary<string, object>();
    }

    static public Furniture PlaceInstance(Furniture proto, Tile tile)
    {
        if (proto.funcPositionValidation(tile) == false)
        {
            Debug.LogError("PlaceInstance -- position validation fonction return FALSE");
            return null;
        }

        Furniture obj = proto.Clone();
        obj.tile = tile;

        // FIXME: Nous ne gerons que les furniture de taille 1x1
        if (tile.PlaceFurniture(obj) == false)
        {
            return null;
        }

        //responsable de l'update des voisins
        if (obj.linksToNeighbour) //cette furniture est lié a ses voisins
        {            
            Tile t;
            int x = tile.X;
            int y = tile.Y;
            t = tile.world.GetTileAt(x, y + 1);
            if (t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.objectType == obj.objectType)
            {
                t.furniture.cbOnChanged(t.furniture);
            }
            t = tile.world.GetTileAt(x + 1, y);
            if (t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.objectType == obj.objectType)
            {
                t.furniture.cbOnChanged(t.furniture);
            }
            t = tile.world.GetTileAt(x, y - 1);
            if (t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.objectType == obj.objectType)
            {

                t.furniture.cbOnChanged(t.furniture);
            }
            t = tile.world.GetTileAt(x - 1, y);
            if (t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.objectType == obj.objectType)
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
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///
    ///                                 SAVING & LOADING
    /// 
    /// 
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public XmlSchema GetSchema()
    {
        return null;
    }
    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("X", tile.X.ToString());
        writer.WriteAttributeString("Y", tile.Y.ToString());

        writer.WriteAttributeString("objectType", objectType);
        writer.WriteAttributeString("movementCost", movementCost.ToString());

    }
    public void ReadXml(XmlReader reader)
    {
        movementCost = int.Parse(reader.GetAttribute("movementCost"));
    }

}
