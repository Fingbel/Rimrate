using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path_TileGraph{

    //cette classe construit le graphe de pathfinding
    //chaque tile est une node
    //chaque voisins atteignable est lié via une connection edge

   public Dictionary<Tile,Path_Node<Tile>> nodes;

    public Path_TileGraph(World world)
    {
        //Debug.Log("Path_TileGraph");

        nodes = new Dictionary<Tile, Path_Node<Tile>>();
        //loop a travers toutes les tiles
        for (int x = 0; x < world.Width; x++){
            for (int y = 0; y < world.Height; y++){
                Tile t = world.GetTileAt(x, y);
                //if(t.movementCost > 0) //si la tile retourn 0, la tile ne peux etre traversé                
                    Path_Node<Tile> n = new Path_Node<Tile>();
                    n.data = t;
                    nodes.Add(t, n);
              
            }
        }
        //Debug.Log("Path_TileGraph: Created " + nodes.Count +" nodes");
        //pour chaque tile crée une node
        //tri des nodes avec des tiles vide ou impassable (mur)
        //reloop chaque node et créer les connexions pour les voisins
        int edgeCount = 0;
        foreach(Tile t in nodes.Keys)
        {
            Path_Node<Tile> n = nodes[t];

            List<Path_Edge<Tile>> edges = new List<Path_Edge<Tile>>();

            //récupération de la liste des voisins, et si le voisin est traversable, création d'un edge
            Tile[] neighbours = t.GetNeighbours(true,false);
            for (int i = 0; i < neighbours.Length; i++)
            {
                if(neighbours[i] != null && neighbours[i].movementCost > 0)
                {
                    if (isClippingCorner(t, neighbours[i]))
                    {
                        continue; //skip sans edge
                    }

                    Path_Edge<Tile> e = new Path_Edge<Tile>();
                    e.cost = neighbours[i].movementCost;
                    e.node = nodes[neighbours[i]];


                    edges.Add(e);
                    edgeCount++;
                }
            }
            n.edges = edges.ToArray();
        }
       // Debug.Log("Path_TileGraph: Created " + edgeCount + " edges");

    }

    bool isClippingCorner(Tile curr, Tile neigh)
    {
        int dX = (curr.X - neigh.X);
        int dY = (curr.Y - neigh.Y);

        if (Mathf.Abs(dX) + Mathf.Abs(dY) ==2) //check de la diagonale
        {          
            if(curr.world.GetTileAt(curr.X - dX,curr.Y).movementCost == 0)
            {
                return true;
            }
            if (curr.world.GetTileAt(curr.X, curr.Y - dY).movementCost == 0)
            {
                return true;
            }
        }
        return false;
    }

}
