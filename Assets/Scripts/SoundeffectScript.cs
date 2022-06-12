using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundeffectScript : MonoBehaviour
{
    public AudioSource src;
    public AudioClip sfx1;
    
   
    
    

    void Start()
    {
        // Vector3 pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
           if (Input.anyKey)
        {
            src.clip = sfx1;
            src.PlayOneShot(sfx1);
           }

        }





        // if (transform.hasChanged)
        //{

        //    src.clip = sfx1;
        //    src.Play();


        //   }




    }


       

                

      

