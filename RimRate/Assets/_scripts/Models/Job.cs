using System;
using System.Collections.Generic;
using UnityEngine;

public class Job {
    public Tile tile { get; protected set; }
    float jobTime;

    //FIXME : hardcoded parameter 
    public string jobObjectType { get; protected set; }

    Action<Job> cbJobComplete;
    Action<Job> cbJobCancel;

    //Initialisation de la classe Job
    public Job (Tile tile,string jobObjectType, Action<Job> cbJobComplete, float jobTime = 1f)
    {
        this.tile = tile;
        this.jobObjectType = jobObjectType;
        this.cbJobComplete += cbJobComplete;
    }

    //Les fonctions d'enregistrement / désenregistrements
    public void RegisterJobCompleteCallback(Action<Job> cb)
    {
        cbJobComplete += cb;
    }

    public void RegisterJobCancelCallback(Action<Job> cb)
    {
        cbJobCancel += cb;
    }

    public void UnregisterJobCompleteCallback(Action<Job> cb)
    {
        cbJobComplete -= cb;
    }

    public void UnregisterJobCancelCallback(Action<Job> cb)
    {
        cbJobCancel -= cb;
    }

    //Fonction de complétion du travail
    public void DoWork(float workTime)
    {
        jobTime -= workTime;
        if(jobTime <= 0)
        {
            if(cbJobComplete != null)
            cbJobComplete(this);
        }
    }
    //Fonction d'annulation du travail
    public void CancelJob(float workTime)
    {
            if (cbJobCancel != null)
            cbJobCancel(this);
    }
}
