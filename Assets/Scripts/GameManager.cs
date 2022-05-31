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
    public NodePath[] paths;
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

        SortPaths(unsortedPaths);
    }

    private void SortPaths(NodePath[] _path)
    {
        for (int i = 0; i < _path.Length; i++)
            paths[_path[i].ID] = _path[i];
    }

    private List<Transform> SortNodes(List<Transform> _nodes)
    {
        List<Transform> newNodes = new List<Transform>();
        for (int i = 0; i < newNodes.Count - 1; i++)
        {
            for (int j = 0; j < _nodes.Count - 1; j++)
            {
                if (_nodes[j].name == "Node_" + i)
                    newNodes[i] = _nodes[j];
            }
        }
        return newNodes;
    }

    public IEnumerator SwitchCharacterPath(int _newPathID, int _startNode, int _doorNode, Direction _startDirection, float _delay)
    {
        DestroyCharacter();
        yield return new WaitForSeconds(_delay);
        SpawnCharacter(_newPathID, _startNode, _startDirection);
    }

    public void RemoveNodesFromPath(int _pathID, int[] _nodeIDs)
    {
        for (int i = 0; i < paths[_pathID].nodes.Count - 1; i++)
        {
            for (int j = 0; j < _nodeIDs.Length; i++)
            { 
                if (i == _nodeIDs[j])
                    paths[_pathID].nodes.RemoveAt(i);
            }
        }
    }

    public void AddNodesToPath(int _pathID, int[] _nodeIDs) 
    {
        foreach (int _node in _nodeIDs)
            paths[_pathID].nodes.Add(paths[_pathID].transform.Find("Node_" + _node));

        paths[_pathID].nodes = SortNodes(paths[_pathID].nodes);
    }

    #endregion

    #region Spawning & Destroying

    public void SpawnCharacter(int _pathID, int _startNode, Direction _startDirection) 
    {
        if (characterGameObject == null)
        {
            Vector3 spawnPosition = paths[_pathID].gameObject.GetComponent<NodePath>().nodes[_startNode].position;

            characterGameObject = Instantiate(characterPrefab, spawnPosition, Quaternion.identity);
            character = characterGameObject.GetComponent<Character>();
            character.path = paths[_pathID].transform;
            character.currentNode = _startNode;
            character.direction = _startDirection;
            character.boundToPath = true;
        }

        CamFollowsPlayer(characterGameObject);
    }

    public void DropCharacter(Vector3 _dropPosition)
    {
        if (characterGameObject == null)
        { 
            characterGameObject = Instantiate(characterPrefab, _dropPosition, Quaternion.identity);
            character = characterGameObject.GetComponent<Character>();
            character.gameObject.GetComponent<Rigidbody>().useGravity = true;
            character.path = null;
            character.currentNode = 0;
            character.direction = Direction.stationary;
            character.boundToPath = false; 
        }

        CamFollowsPlayer(characterGameObject);
    }

    // If the player spawns in / drops in, the camera has to follow the player
    public void CamFollowsPlayer(GameObject player)
    {
        PlayerCamera.playerIsActive = true;
        PlayerCamera.characterTransform = player.transform;
    }

    // And if the player gets destroyed, this function makes sure the PlayerCamera script doesn't create any errors
    public void CamStopsFollowPlayer()
    {
        PlayerCamera.playerIsActive = false;
        PlayerCamera.characterTransform = null;
    }

    public void DestroyCharacter()
    {
        CamStopsFollowPlayer();
        Destroy(characterGameObject);
    }

    #endregion
}
