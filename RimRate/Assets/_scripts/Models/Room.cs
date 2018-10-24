using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room {

    List<Tile> tiles;

    public Room()
    {
        tiles = new List<Tile>();
    }

    public void AssignTile(Tile t)
    {
        if (tiles.Contains(t))
            return;
        t.room = this;
        tiles.Add(t);
    }

    public void UnAssignAllTiles()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].room = tiles[i].world.GetOutsideRoom(); ; //Assign to outside
        }
        tiles = new List<Tile>();

    }

    public static void DoRoomFloodFill(Furniture sourceFurniture)
    { World world = sourceFurniture.tile.world;

        if (sourceFurniture.tile.room != world.GetOutsideRoom())
            world.DeleteRoom(sourceFurniture.tile.room);

    }
	
}
