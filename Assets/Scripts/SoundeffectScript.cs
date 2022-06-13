using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundeffectScript : MonoBehaviour
{
    public AudioSource src;
    public AudioClip sfx1;
    private GameManager gameManager;
   
    
    

    void Start()
    {
        gameManager = GameManager.instance;
        // Vector3 pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.playStoneSound)
        {
            src.clip = sfx1;
            src.PlayOneShot(sfx1);
            gameManager.playStoneSound = false;
        }
    }
 }


       

                

      

