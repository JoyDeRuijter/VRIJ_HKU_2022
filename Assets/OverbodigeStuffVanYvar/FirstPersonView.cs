using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonView : MonoBehaviour
{
    Character character;

    private void Awake()
    {
        character = FindObjectOfType<Character>();
    }

    private void Update()
    {
        if (character != null)
        {
            transform.position = character.transform.position;
            transform.rotation = character.transform.rotation;
            transform.position += character.transform.up * 0.5f;
        }
        else
        {
            character = FindObjectOfType<Character>();
        }
    }
}
