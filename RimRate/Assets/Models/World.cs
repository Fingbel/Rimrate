using System.Collections;
using UnityEngine;

public class World
{
    //définition du monde
    Tile[,] tiles;
    int width;
    int height;

    //création des tiles en fonction de la taille du monde
    public World(int width = 100, int height = 100)
    {
        this.width = width;
        this.height = height;

        tiles = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = new Tile(this, x, y);
            }
        }
    }

    //FONCTION DE récupération des coordonnées de tile
    public Tile GetTileAt(int x, int y)
    {
        if (x > width || x < 0)
        {
            Debug.LogError("Tile (" + x + "," + y + ") hors limite.");
            return null;
        }
        return tiles[x, y];
    }

    //Getter et Setter
    public int Width
    {
        get
        {
            return width;
        }
    }
    public int Height
    {
        get
        {
            return height;
        }
    }
    
    //DEBUG FONCTION
    public void RandomizeTile()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (Random.Range(0, 2) == 0)
                {
                    tiles[x, y].Type = Tile.TileType.Water;
                }
                else
                {
                    tiles[x, y].Type = Tile.TileType.Boat;
                }
            }
        }        
    }
}