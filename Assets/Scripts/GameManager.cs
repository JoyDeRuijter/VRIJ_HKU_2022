using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager instance;

    #endregion

    #region Variables

    [Header("Character")]
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private int startPathID;
    [SerializeField] private int startNode;
    [SerializeField] private Direction startDirection;

    public Dictionary<Vector3Int, Cube> walkableCubes = new Dictionary<Vector3Int, Cube>();
    [HideInInspector] public NodePath[] paths;
    [HideInInspector] public Character character;

    private Stairs stairs;
    private GameObject characterGameObject;

    #endregion

    private void Awake()
    {
        instance = this;
        stairs = FindObjectOfType<Stairs>();
        InitializePaths();
        SpawnCharacter(startPathID, startNode, startDirection);
    }

    private void Start()
    {
        SaveWalkableCubes();
    }

    #region Cubes

    // Find all the cubes that are walkable in the scene and store them in a list
    private void SaveWalkableCubes()
    {
        Cube[] _cubes = FindObjectsOfType<Cube>();

        for (int i = 0; i < _cubes.Length; i++)
        {
            _cubes[i].name = "Cube_" + i;
            Vector3Int _cubePosition = _cubes[i].position;
            if (_cubes[i].isWalkable && !walkableCubes.ContainsKey(_cubePosition)) 
                walkableCubes.Add(_cubePosition, _cubes[i]);
        }
    }
    #endregion

    #region Paths & Path Switching

    private void InitializePaths()
    {
        NodePath[] unsortedPaths = FindObjectsOfType<NodePath>();
        paths = new NodePath[unsortedPaths.Length];

        for (int i = 0; i < unsortedPaths.Length; i++)
            paths[unsortedPaths[i].ID] = unsortedPaths[i];
    }

    public IEnumerator SwitchCharacterPath(int _newPathID, int _startNode, int _doorNode, Direction _startDirection, float _delay)
    {
        Vector3Int nodePosition = new Vector3Int((int)character.GetNodePosition(_doorNode).x, (int)(character.GetNodePosition(_doorNode).y - 5.1), (int)character.GetNodePosition(_doorNode).z);
        
        DestroyCharacter();
        //walkableCubes[nodePosition].ChangeMaterial(walkableCubes[nodePosition].normalMaterial);
        yield return new WaitForSeconds(_delay);

        SpawnCharacter(_newPathID, _startNode, _startDirection);
    }

    #endregion

    #region Spawning & Destroying

    public void SpawnCharacter(int _pathID, int _startNode, Direction _startDirection) 
    {
        Vector3 spawnPosition = paths[_pathID].gameObject.GetComponent<NodePath>().nodes[_startNode].position;

        characterGameObject = Instantiate(characterPrefab, spawnPosition, Quaternion.identity);
        character = characterGameObject.GetComponent<Character>();
        character.path = paths[_pathID].transform;
        character.currentNode = _startNode;
        character.direction = _startDirection;
    }

    public void DropCharacter(Vector3 _dropPosition)
    {
        characterGameObject = Instantiate(characterPrefab, _dropPosition, Quaternion.identity);
        character = characterGameObject.GetComponent<Character>();
        character.path = null;
        character.currentNode = 0;
        character.direction = Direction.stationary;
    }

    private void DestroyCharacter()
    {
        Destroy(characterGameObject);
    }

    #endregion
}
