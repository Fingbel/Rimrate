using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Character {

    public float X
    {
        get
        {
            return Mathf.Lerp(currTile.X, destTile.X, movementPercentage);
        }
    }

    public float Y
    {
        get
        {
            return Mathf.Lerp(currTile.Y, destTile.Y, movementPercentage);
        }
    }


    public Tile currTile { get; protected set; }
    Tile destTile;
    float movementPercentage;
    float speed = 2f;
    Job myJob;

    Action<Character> cbCharacterChanged;

    public Character(Tile tile)
    {
        currTile = destTile = tile;
    }

    public void Update(float deltaTime)
    {
        if(myJob == null)
        {
            //Cherche du travail
            myJob = currTile.world.jobQueue.Dequeue();
            

            if(myJob != null)
            {
                Debug.Log(myJob);
                //A trouvé du travail
                destTile = myJob.tile;
                myJob.RegisterJobCancelCallback(OnJobEnded);
                myJob.RegisterJobCompleteCallback(OnJobEnded);
            }
        }


        //On est arrivé ?
        if (currTile == destTile)
        {
            if(myJob != null)
            {
                myJob.DoWork(deltaTime);
            }
        }
           

        //Quel est la distance total de A a B
        float distToTravel = Mathf.Sqrt(Mathf.Pow(currTile.X - destTile.X, 2) + Mathf.Pow(currTile.Y - destTile.Y, 2));

        //De combien on a avancé ?
        float distThisFrame = speed * deltaTime;

        //en pourcentage du total ?
        float percThisFrame = distThisFrame / distToTravel;

        //On ajoute ca au total de voyage effectué en pourcentage
        movementPercentage += percThisFrame;
        if(movementPercentage >= 1)
        {
            currTile = destTile;
            movementPercentage = 0;
        }

        if(cbCharacterChanged != null)
        {
            cbCharacterChanged(this);
        }
    }

    public void SetDestination(Tile tile)
    { 
        if(currTile.IsNeighbour(tile, true) == false)
        {
            Debug.Log("Les deux tiles ne sont pas adjacentes");
        }
        destTile = tile;
    }

    public void RegisterOnChangedCallback(Action<Character> cb)
    {
        cbCharacterChanged += cb;
    }

    public void UnregisterOnChangedCallback(Action<Character> cb)
    {
        cbCharacterChanged -= cb;
    }

    //job fini ou cancel
    void OnJobEnded(Job j)
    {
        if (j != myJob)
        {
            Debug.LogError("Attention character ne travaillant pas sur son job");
            return;
        }
        myJob = null;
    }
}
