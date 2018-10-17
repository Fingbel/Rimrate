using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteController : MonoBehaviour {

    Dictionary<Character, GameObject> characterGameObjectMap;
    Dictionary<string, Sprite> characterSprites;

    World world
    {
        get { return WorldController.Instance.world; }
    }
    // Use this for initialization
    void Start () {
        LoadSprites();

        // Instantiate ours dictionary that tracks which GameObject is rendering which Tile data.
        characterGameObjectMap = new Dictionary<Character, GameObject>();
        world.RegisterCharacterCreated(OnCharacterCreated);



        //DEBUG 
        world.CreateCharater(world.GetTileAt(world.Width / 2,world.Height / 2));
        //world.CreateCharater(world.GetTileAt(world.Width / 2+1, world.Height / 2));
        //world.CreateCharater(world.GetTileAt(world.Width / 2-1, world.Height / 2));



    }

    void LoadSprites()
    {
        characterSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Characters/");
        Debug.Log("DONE LOADED RESSOURCES : " + sprites.Length + " characters.");
        foreach (Sprite s in sprites)
        {
            characterSprites[s.name] = s;
        }
    }

    public void OnCharacterCreated(Character c)
    {

        GameObject char_go = new GameObject();

        // Add our char/GO pair to the dictionary.
        characterGameObjectMap.Add(c, char_go);

        char_go.name = "Character";
        char_go.transform.position = new Vector3(c.X, c.Y, 0);
        char_go.transform.SetParent(this.transform, true);


        char_go.AddComponent<SpriteRenderer>().sprite = characterSprites["chartest_0"];
        char_go.GetComponent<SpriteRenderer>().sortingLayerName = "character";
        // Register our callback so that our GameObject gets updated whenever
        // the objects's type changes.
        c.RegisterOnChangedCallback(OnCharacterChanged);
    }

     void OnCharacterChanged(Character c)
     {
         //Update des gfx des characters
         if (characterGameObjectMap.ContainsKey(c) == false)
         {
             Debug.LogError("OnCharacterChanged -- pas de key " + c + "dans le dictionnary " + characterGameObjectMap);
             return;
         }
         GameObject char_go = characterGameObjectMap[c];
        //char_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForCharacter(character);
        char_go.transform.position = new Vector3(c.X, c.Y, 0);


    }
     
}
