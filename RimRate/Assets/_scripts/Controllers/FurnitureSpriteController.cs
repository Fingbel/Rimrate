using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class FurnitureSpriteController : MonoBehaviour
{
    Dictionary<Furniture, GameObject> furnitureGameObjectMap;
    Dictionary<string, Sprite> furnitureSprites;

    World world
    {
        get { return WorldController.Instance.world; }
    }
    
    // Use this for initialization
    void Start()
    {
        LoadSprites();
        world.RegisterFurnitureCreated(OnFurnitureCreated);

        // Instantiate ours dictionary that tracks which GameObject is rendering which Tile data.
        furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();     
        
    }

    //fonction de chargement des ressources
    void LoadSprites()
    {
        furnitureSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Furnitures/");
        Debug.Log("DONE LOADED RESSOURCES : " + sprites.Length +" furnitures.");
        foreach (Sprite s in sprites)
        {
            furnitureSprites[s.name] = s;
        }
    }

    public void OnFurnitureCreated(Furniture furn)
    {
        //Debug.Log("OnFurnitureCreated");
        //create a visual GameObject linked to this data
        // This creates a new GameObject and adds it to our scene.
        GameObject furn_go = new GameObject();

        // Add our tile/GO pair to the dictionary.
        if (!furnitureGameObjectMap.ContainsKey(furn))
        {
            furnitureGameObjectMap.Add(furn, furn_go);
        }
        

        furn_go.name = furn.objectType +"_"+ furn.tile.X +"_" + furn.tile.Y;
        furn_go.transform.position = new Vector3(furn.tile.X, furn.tile.Y, 0);
        furn_go.transform.SetParent(this.transform, true);


        furn_go.AddComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furn);
        furn_go.GetComponent<SpriteRenderer>().sortingLayerName = "furniture";
        // Register our callback so that our GameObject gets updated whenever
        // the objects's type changes.
        furn.RegisterOnChangedCallback(OnFurnitureChanged);
    }

    void OnFurnitureChanged(Furniture furn)
    {     
        //Update des gfx des furnitures
        if (furnitureGameObjectMap.ContainsKey(furn) == false)
        {
            Debug.LogError("OnFurnitureChanged -- pas de key " + furn + "dans le dictionnary " + furnitureGameObjectMap);
            return;
        }
        GameObject furn_go = furnitureGameObjectMap[furn];
        furn_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furn);


    }

    public Sprite GetSpriteForFurniture(Furniture obj){
        if (obj.linksToNeighbour == false)
        {
            return furnitureSprites[obj.objectType];
        }
        string spriteName = obj.objectType + "_";

        int x = obj.tile.X;
        int y = obj.tile.Y;
        //CHECK FOR NEIGBOUR HERE CLOCKWISE 
        Tile t;
        t = world.GetTileAt(x, y + 1);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType)
        {
            spriteName += "N";
        }
        t = world.GetTileAt(x+1, y);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType)
        {
            spriteName += "E";
        }
        t = world.GetTileAt(x, y - 1);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType)
        {

            spriteName += "S";
        }
        t = world.GetTileAt(x-1, y);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType)
        {
            spriteName += "W";
        }

        if(furnitureSprites.ContainsKey(spriteName) == false) {
            Debug.LogError("No sprite with that name" + spriteName);
            return null;
        }
        return furnitureSprites[spriteName];
    }

    public Sprite GetSpriteForFurniture(string objectType)
    {
        if (furnitureSprites.ContainsKey(objectType))
        {
            return furnitureSprites[objectType];
        }

        if (furnitureSprites.ContainsKey(objectType+"_"))
        {
            return furnitureSprites[objectType+"_"];
        }

        Debug.LogError("No sprite with that name" + objectType);

        return null;
    }
}
