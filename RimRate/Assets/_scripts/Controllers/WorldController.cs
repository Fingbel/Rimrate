using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; protected set; }

    // The world and tile data
    public World world { get; protected set; }

    
    // Use this for initialization
    void OnEnable()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be two world controllers.");
        }
        Instance = this;

        // Create a world with Empty tiles
        world = new World();
        
    //center the camera
    Camera.main.transform.position = new Vector3(world.Width / 2, world.Width / 2, Camera.main.transform.position.z);
    }

    void Update()
    {
        //TODO : Gestion de l'écoulement du temps du jeu !
        world.Update(Time.deltaTime);
    }

    public Tile GetTileAtWorldCoord(Vector3 coord)
    {
        int x = Mathf.FloorToInt(coord.x);
        int y = Mathf.FloorToInt(coord.y);

        return world.GetTileAt(x, y);
    } 
}
