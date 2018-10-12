using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

    float soundCooldown = 0f;

	// Use this for initialization
	void Start () {
        WorldController.Instance.world.RegisterFurnitureCreated(OnFurnitureCreated);
        WorldController.Instance.world.RegisterTileChanged(OnTileChanged);
    }

    // Update is called once per frame
    void Update () {
        soundCooldown -= Time.deltaTime;
	}

    void OnTileChanged(Tile tile_data)
    {
        //FIXME - HARDCODED 
        if (soundCooldown > 0)
            return;

            AudioClip ac = Resources.Load<AudioClip>("Sounds/clap");
            AudioSource.PlayClipAtPoint(ac, Camera.main.transform.position);
        soundCooldown = 0.1f;
    }

    public void OnFurnitureCreated(Furniture furn)
    {
        //FIXME - HARDCODED 
        if (soundCooldown > 0)
            return;

        AudioClip ac = Resources.Load<AudioClip>("Sounds/plonk");
        AudioSource.PlayClipAtPoint(ac, Camera.main.transform.position);
        soundCooldown = 0.1f;
    }
}
