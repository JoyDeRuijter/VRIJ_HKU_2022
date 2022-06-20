using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundeffectScript : MonoBehaviour
{
    public AudioSource src;
    public AudioClip[] clips;
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
            int r = Random.Range(0, clips.Length - 1);
            src.PlayOneShot(clips[r]);
            gameManager.playStoneSound = false;
        }
    }
 }


       

                

      

