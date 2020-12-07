/*
 * Loader.cs - loads game manager instance at start of game
 * 
 * Alek DeMaio, Doug McIntyre, Inaya Alkhatib, JD Davis, June Tejada
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;
    
    // Start is called before the first frame update
    void Awake() //instantiates GameManager instance if one does not already exist
    {
        if (GameManager.instance == null)
            Instantiate(gameManager);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
