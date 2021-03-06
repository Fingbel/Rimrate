﻿using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using System.IO;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; protected set; }

    // The world and tile data
    public World world { get; protected set; }

    static bool loadWorld = false;

    // Use this for initialization
    void OnEnable()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be two world controllers.");
        }
        Instance = this;

        if (loadWorld)
        {
            loadWorld = false;
            CreateWorldFromSaveFile();
        }
        else
        {
            CreateEmptyWorld();
        }
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

    public void NewWorld()
    {
        Debug.Log("New World Button Clicked");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SaveWorld()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(World));
        TextWriter writer = new StringWriter();

        serializer.Serialize(writer, world);
        writer.Close();

        Debug.Log(writer.ToString());

        PlayerPrefs.SetString("SaveGame00",writer.ToString());

    }

    public void LoadWorld()
    {
        //TODO Load
        loadWorld = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void CreateEmptyWorld()
    {
        Debug.Log("CreateEmptyWorld");
        // Hello World !
        world = new World(100,100);

        //Centrer la camera
        Camera.main.transform.position = new Vector3(world.Width / 2, world.Width / 2, Camera.main.transform.position.z);
    }

    void CreateWorldFromSaveFile()
    {
        Debug.Log("CreateWorldFromSaveFile");
        // Hello Again World ! (from save)
        XmlSerializer serializer = new XmlSerializer(typeof(World));
        TextReader reader = new StringReader(PlayerPrefs.GetString("SaveGame00"));

        world = (World)serializer.Deserialize(reader);
        reader.Close();

        //Centrer la camera
        Camera.main.transform.position = new Vector3(world.Width / 2, world.Width / 2, Camera.main.transform.position.z);
    }
}
