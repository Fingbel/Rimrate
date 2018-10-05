using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AutomaticVerticalSize : MonoBehaviour {

    public float childheight = 35f;

	// Use this for initialization
	void Start () {
        AdjustSize();
	}

    public void AdjustSize()
    {
        Vector2 size = this.GetComponent<RectTransform>().sizeDelta;
        size.y = this.transform.childCount*childheight ;
        this.GetComponent<RectTransform>().sizeDelta = size;
    }
}
