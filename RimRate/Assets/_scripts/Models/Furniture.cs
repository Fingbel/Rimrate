using UnityEngine;
using System.Collections.Generic;
using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

// Furniture are things like walls, doors, and furniture (e.g. a sofa)

public class Furniture : IXmlSerializable
{
    public Dictionary<string, float> furnParameters;
    public Action<Furniture, float> updateActions;

    public Func<Furniture,Enterability> isEnterable;

    public void Update(float deltaTime)
    {
        if (this.updateActions != null)
        {
            updateActions(this, deltaTime);
        }
    }

    public Tile tile { get; protected set; }
    public string objectType { get; protected set; }
    public float movementCost { get; protected set; }
    public bool roomEnclosure { get; protected set; }
    int width;
    int height;
    public bool linksToNeighbour { get; protected set; }
    public Action<Furniture> cbOnChanged;
    Func<Tile, bool> funcPositionValidation;

    // TODO: Implement larger objects
    // TODO: Implement object rotation

   

    //empty constructor for serialization
    public Furniture()
    {
        furnParameters = new Dictionary<string, float>();

    }

    //Copy Constructor
    protected Furniture(Furniture other)
    {
        this.objectType = other.objectType;
        this.movementCost = other.movementCost;
        this.roomEnclosure = other.roomEnclosure;
        this.width = other.width;
        this.height = other.height;
        this.linksToNeighbour = other.linksToNeighbour;
        this.furnParameters = new Dictionary<string, float>(other.furnParameters);
        if(other.updateActions != null)
        {
            this.updateActions = (Action<Furniture, float>)other.updateActions.Clone();
        }
        this.isEnterable = other.isEnterable;
    }

    //Virtual Clone Constructor
    virtual public Furniture Clone()
    {
        return new Furniture(this);
    }

    //Prototype Constructor -- 
    public Furniture (string objectType, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbour = false,bool roomEnclosure = false)
    {      
        this.objectType = objectType;
        this.movementCost = movementCost;
        this.roomEnclosure = roomEnclosure;
        this.width = width;
        this.height = height;
        this.linksToNeighbour = linksToNeighbour;
        this.funcPositionValidation = this.__IsValidPosition;
        furnParameters = new Dictionary<string, float>();
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
        //writer.WriteAttributeString("movementCost", movementCost.ToString());

        foreach(string k in furnParameters.Keys)
        {
            writer.WriteElementString("name",k);
            writer.WriteStartElement("value",furnParameters[k].ToString());
            writer.WriteEndElement();

        }
    }
    public void ReadXml(XmlReader reader)
    {
        //movementCost = int.Parse(reader.GetAttribute("movementCost"));

        if (reader.ReadToDescendant("Param"))
        {
            do
            {
                string k = reader.GetAttribute("name");
                float v = float.Parse(reader.GetAttribute("Value"));
                furnParameters[k] = v;
            } while (reader.ReadToNextSibling("Param"));
        }
    }

}
