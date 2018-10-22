using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class BuildModeController : MonoBehaviour
{
    bool buildModeIsObjects = false;
    TileType buildModeTile = TileType.Floor;
    string buildModeObjectType;

    //DEBUG
    public void DoPathFindingTest()
    {
        WorldController.Instance.world.SetupPathFindingDebug();
    }

    public void SetMode_BuildFloor()
    {
        buildModeIsObjects = false;
        buildModeTile = TileType.Floor;
    }

    public void SetMode_BuildWater()
    {
        buildModeIsObjects = false;
        buildModeTile = TileType.Water;
    }


        public void SetMode_Bulldoze()
    {
        buildModeIsObjects = false;
        buildModeTile = TileType.Empty;
    }

    public void SetMode_BuildFurniture(string objectType)
    {
        // Wall is not a Tile!  Wall is an "Furniture" that exists on TOP of a tile.
        buildModeIsObjects = true;
        buildModeObjectType = objectType;
    }

    public void DoBuild(Tile t)
    {
        if (buildModeIsObjects == true)
            //on est en mode de construction d'objets
        {

            string furnitureType = buildModeObjectType;
            if (WorldController.Instance.world.IsFurniturePlacementValid(furnitureType, t) && t.pendingFurnitureJob == null)
            {
                Job j = new Job(t,furnitureType, (theJob) =>
                {
                    WorldController.Instance.world.PlaceFurniture(furnitureType, theJob.tile);
                    t.pendingFurnitureJob = null;
                }
                );
                t.pendingFurnitureJob = j;
                j.RegisterJobCancelCallback((theJob) => { theJob.tile.pendingFurnitureJob = null; });

                //ajout du job a la queue de jobs
                WorldController.Instance.world.jobQueue.Enqueue(j);
            }
        }
        else
        {
            // On est en mode de changement de tile
            t.Type = buildModeTile;
        }
    }


}
