using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

    public GameObject cursor;
    Vector3 lastFramePosition;

	void Update () {
        //Update de la position de la souris
        Vector3 currentFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentFramePosition.z = 0;

        //Update de la position du curseur
        Tile tileUnderMouse = GetTileAtWorldCoord(currentFramePosition);
        if (tileUnderMouse != null)
        {   Vector3 cursorPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y, 0);
            cursor.transform.position = cursorPosition;
        }
        else
        {
            cursor.SetActive(false);
        }

        //Gérer le clic gauche
        if (Input.GetMouseButtonDown(0))
        {
            if (tileUnderMouse != null)
            {
                if(tileUnderMouse.Type == Tile.TileType.Water)
                {
                    tileUnderMouse.Type = Tile.TileType.Grass;
                }
                else
                {
                    tileUnderMouse.Type = Tile.TileType.Water;
                }
            }
        }

        //Permet le mouvement de la caméra avec la souris
        if (Input.GetMouseButton(2) || Input.GetMouseButton(1)){

            Vector3 diff = lastFramePosition - currentFramePosition;
            Camera.main.transform.Translate(diff);
        }

        //Update de la derniere position de la souris
        lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastFramePosition.z = 0;

        Debug.Log("tileundermouse" + tileUnderMouse.Type);
    }

    Tile GetTileAtWorldCoord(Vector3 coord)
    {
        int x = Mathf.FloorToInt(coord.x);
        int y = Mathf.FloorToInt(coord.y);

        return WorldController.Instance.World.GetTileAt(x, y);
    }
}
