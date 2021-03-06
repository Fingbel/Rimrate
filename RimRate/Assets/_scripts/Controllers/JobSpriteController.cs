﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobSpriteController : MonoBehaviour {

    //Controller responsable de l'affichage des GFX des jobs
    FurnitureSpriteController fsc;
    Dictionary<Job, GameObject> jobGameObjectMap;


    // Use this for initialization
    void Start () {
        fsc = GameObject.FindObjectOfType<FurnitureSpriteController>();
        jobGameObjectMap = new Dictionary<Job, GameObject>();
        WorldController.Instance.world.jobQueue.RegisterJobCreationCallBack(OnJobCreated);
	}


    //Fonction appelé a chaque création de job
    void OnJobCreated(Job job)
    {
        if (jobGameObjectMap.ContainsKey(job))
        {
            Debug.LogError("Job already exist");
            return;
        }

        GameObject job_go = new GameObject();
        // Add our tile/GO pair to the dictionary.
        jobGameObjectMap.Add(job, job_go);

        job_go.name = "JOB_" + job.jobObjectType + "_" + job.tile.X + "_" + job.tile.Y;
        job_go.transform.position = new Vector3(job.tile.X, job.tile.Y, 0);
        job_go.transform.SetParent(this.transform, true);

        SpriteRenderer sr = job_go.AddComponent<SpriteRenderer>();      
        sr.sprite = fsc.GetSpriteForFurniture(job.jobObjectType);
        sr.sortingLayerName = "job";
        sr.color = new Color(1f, 1f, 1f, 0.5f);

        //FIXME : Hardcoded stuff
        if (job.jobObjectType == "door")
        {
            Tile northTile = job.tile.world.GetTileAt(job.tile.X, job.tile.Y + 1);
            Tile southTile = job.tile.world.GetTileAt(job.tile.X, job.tile.Y - 1);
            if (northTile != null && southTile != null && northTile.furniture != null && southTile.furniture != null && northTile.furniture.objectType == "wall" && southTile.furniture.objectType == "wall")
            {
                job_go.transform.rotation = Quaternion.Euler(0, 0, 90);
                job_go.transform.Translate(1f, 0, 0, Space.World);
            }
        }

        //enregistrement des callbacks
        job.RegisterJobCompleteCallback(OnJobEnded);
        job.RegisterJobCancelCallback(OnJobEnded);
    }

    //Fonction appelé a chaque fin de job
    void OnJobEnded(Job job)
    {
        GameObject job_go = jobGameObjectMap[job];
        job.UnregisterJobCancelCallback(OnJobEnded);
        job.UnregisterJobCompleteCallback(OnJobEnded);

        Destroy(job_go);
    }
}
