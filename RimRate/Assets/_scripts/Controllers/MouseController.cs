using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    public GameObject CursorPrefab;

    TileType buildModeTile = TileType.Water;

    //Settings
    public float minZoomSettings = 3f;
    public float maxZoomSettings = 50f;

    // The world-position of the mouse last frame.
    Vector3 lastFramePosition;
    Vector3 currFramePosition;

    // The world-position start of our left-mouse drag operation
    Vector3 dragStartPosition;
    List<GameObject> dragPreviewGameObjects;

    // Use this for initialization
    void Start()
    {
        dragPreviewGameObjects = new List<GameObject>();
    }



    // Update is called once per frame
    void Update()
    {
        currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currFramePosition.z = 0;

        //UpdateCursor();
        UpdateDragging();
        UpdateCameraMovement();

        // Update de la derniere position de la souris
        lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastFramePosition.z = 0;
    }
    
    void UpdateCursor()
    {
        /*
        // Update du curseur
        Tile tileUnderMouse = WorldController.Instance.GetTileAtWorldCoord(currFramePosition);
        if (tileUnderMouse != null)
        {
            CursorPrefab.SetActive(true);
            Vector3 cursorPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y, 0);
            CursorPrefab.transform.position = cursorPosition;
        }
        else
        {
            // Souris hors limite, cacher le curseur
            CursorPrefab.SetActive(false);
        }
        */
    }

    void UpdateDragging()
    {
        //Pour ne pas cliquer a travers l'UI

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        // Début du drag
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPosition = currFramePosition;
        }

        int start_x = Mathf.FloorToInt(dragStartPosition.x);
        int end_x = Mathf.FloorToInt(currFramePosition.x);
        int start_y = Mathf.FloorToInt(dragStartPosition.y);
        int end_y = Mathf.FloorToInt(currFramePosition.y);

        //On vérifie si jamais c'est un drag dans le "mauvais" sens et on inverse
        if (end_x < start_x)
        {
            int tmp = end_x;
            end_x = start_x;
            start_x = tmp;
        }
        if (end_y < start_y)
        {
            int tmp = end_y;
            end_y = start_y;
            start_y = tmp;
        }

        //Drag Preview Pooling
        while(dragPreviewGameObjects.Count > 0)
        {
            GameObject go = dragPreviewGameObjects[0];
            dragPreviewGameObjects.RemoveAt(0);
            SimplePool.Despawn (go);
        }

        //Pendant le drag
        if (Input.GetMouseButton(0))
        {
            for (int x = start_x; x <= end_x; x++)
            {
                for (int y = start_y; y <= end_y; y++)
                {
                    Tile t = WorldController.Instance.World.GetTileAt(x, y);
                    if (t != null)
                    {
                        GameObject go = (GameObject)SimplePool.Spawn(CursorPrefab, new Vector3(x, y, 0), Quaternion.identity);
                        go.transform.parent = this.transform;
                        dragPreviewGameObjects.Add(go);
                    }
                }                
            }
        }
        // Fin du drag
        if (Input.GetMouseButtonUp(0))
        {
            //On change tout les tiles en bouclant
            for (int x = start_x; x <= end_x; x++)
            {
                for (int y = start_y; y <= end_y; y++)
                {
                    Tile t = WorldController.Instance.World.GetTileAt(x, y);
                    if (t != null)
                    {
                        t.Type = buildModeTile;
                    }
                }
            }
        }
    }

    void UpdateCameraMovement()
    {
        // Gestion du déplacement de la caméra
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {   

            Vector3 diff = lastFramePosition - currFramePosition;
            Camera.main.transform.Translate(diff);

        }

        //gestion du zoom
        Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoomSettings, maxZoomSettings);
    }


    //BUILDING MODE
    public void SetMode_BuildGrass()
    {
        buildModeTile = TileType.Grass;
    }

    public void SetMode_Bulldoze()
    {
        buildModeTile = TileType.Water;
    }

}
