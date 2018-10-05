using System.Collections.Generic;
using UnityEngine;

//SCRIPT POUR LES OBJETS FIXES (démontable)
public class InstalledObject
{

    Tile tile;
    string objectType;      //Identifiant utilisé pour render les graphiques
    float movementCost;     //objects speed multiplier (stackable) Si = 0 alors impassable comme un mur
    int width;              //les objets pourront prendre plusieurs tile
    int height;

    //TODO: implémenter les objets larges
    //TODO :implémenter la rotation d'objet

    protected InstalledObject()
    {

    }

    //Prototypage de l'object
    static public InstalledObject CreatePrototype(string objectType, float movementCost = 1f, int width = 1, int height = 1)
    {
        InstalledObject obj = new InstalledObject();
        obj.objectType = objectType;
        obj.movementCost = movementCost;
        obj.width = width;
        obj.height = height;

        return obj;
    }

    //Placement de l'object
    static public InstalledObject PlaceInstance(InstalledObject proto, Tile tile)
    {
        InstalledObject obj = new InstalledObject();
        obj.objectType = proto.objectType;
        obj.movementCost = proto.movementCost;
        obj.width = proto.width;
        obj.height = proto.height;
        obj.tile = tile;


        if (tile.PlaceObject(obj) == false)
        {
            //fail du placement
            return null;
        }

        return obj;
    }
}