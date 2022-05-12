using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager instance;

    #endregion

    #region Variables

    public Dictionary<Vector3Int, Cube> walkableCubes = new Dictionary<Vector3Int, Cube>();
    private Character character;
    private Stairs stairs;

    #endregion

    private void Awake()
    {
        instance = this;
        character = FindObjectOfType<Character>();
        stairs = FindObjectOfType<Stairs>();
    }

    private void Start()
    {
        SaveWalkableCubes();
    }

    private void Update()
    {
        if (character.isMoving)
        { 
            if(walkableCubes.ContainsKey(character.position))
                walkableCubes[character.position].SetTouchedMaterial();
        }
    }

    // Find all the cubes that are walkable in the scene and store them in a list
    private void SaveWalkableCubes()
    {
        Cube[] _cubes = FindObjectsOfType<Cube>();

        for (int i = 0; i < _cubes.Length; i++)
        {
            _cubes[i].name = "Cube_" + i;
            Vector3Int _cubePosition = _cubes[i].position;
            if (_cubes[i].isWalkable && !walkableCubes.ContainsKey(_cubePosition))
            { 
                walkableCubes.Add(_cubePosition, _cubes[i]);
                //Debug.Log("Added " + _cubes[i].name + " with the position: " + _cubePosition);
            }
        }
    }
}
