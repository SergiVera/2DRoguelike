using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Check if GameManager has been instantiated and if not instantiate one from a Prefab
public class Loader : MonoBehaviour {

    public GameManager gameManager;

	// Use this for initialization
	void Awake () {
        if (GameManager.instance == null)
            Instantiate(gameManager);
	}
}
