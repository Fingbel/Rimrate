using System.Collections;
using UnityEngine;

public class World
{
    //définition du monde
    Tile[,] tiles;
    public int Width { get; protected set; }
    public int Height { get; protected set; }

    //Initialisation d'une nouvelle instance du monde
    public World(int width = 100, int height = 100)
    {
        Width = width;
        Height = height;

        tiles = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = new Tile(this, x, y);
            }
        }
        Debug.Log("Monde créer avec " + (width * height) + " tiles");
    }

    //FONCTION DE récupération des coordonnées de tile
    public Tile GetTileAt(int x, int y)
    {
        if (x > Width || x < 0 || y > Height || y < 0)
        {
            Debug.LogError("Tile (" + x + "," + y + ") hors limite.");
            return null;
        }
        return tiles[x,y];
    }
    
    //DEBUG FONCTION
    public void RandomizeTile()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
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