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
    [SerializeField] private WalkDirection startDirection;

    public Dictionary<Vector3Int, Cube> walkableCubes = new Dictionary<Vector3Int, Cube>();
    public NodePath[] paths;
    [HideInInspector] public Character character;

    private Stairs stairs;
    private GameObject characterGameObject;
    private MovableObject[] movableObjects;

    private bool gaveInput;
    [HideInInspector] public bool playStoneSound;
    #endregion

    private void Awake()
    {
        instance = this;
        stairs = FindObjectOfType<Stairs>();
        InitializePaths();
        SpawnCharacter(startPathID, startNode, startDirection);
        InitializeMovableObjects();
    }

    private void InitializeMovableObjects()
    {
        movableObjects = FindObjectsOfType<MovableObject>();
    }


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

    public IEnumerator SwitchCharacterPath(int _newPathID, int _startNode, int _doorNode, WalkDirection _startDirection, float _delay)
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

    public void SpawnCharacter(int _pathID, int _startNode, WalkDirection _startDirection)
    {
        if (characterGameObject == null)
        {
            NodePath spawnPath = paths[_pathID].gameObject.GetComponent<NodePath>();
            Vector3 spawnPosition = spawnPath.nodes[_startNode].position;

            characterGameObject = Instantiate(characterPrefab, spawnPosition, Quaternion.identity);
            character = characterGameObject.GetComponent<Character>();
            character.path = paths[_pathID].transform;
            character.currentNode = _startNode;
            character.direction = _startDirection;
            character.boundToPath = true;
            character.nodePath = spawnPath;

            Physics.gravity = paths[_pathID].gravityDirection * GravityTowardsPoint.gravityStrenght;
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
            character.direction = WalkDirection.stationary;
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

    #region InputManagement

    public void ReceiveInput(int _inputIndex)
    {
        switch (_inputIndex)
        {
            case 0:

                // No input
                break;

            case 1: // fluit pijp 1
                Debug.Log("First pipe was blown");
                // Switch character direction
                character.FlipDirection();
                break;

            case 2: // fluit pijp 2
                Debug.Log("Secoond pipe was blown");
                // Move movable objects

                break;

            case 3: // fluit pijp 3
                Debug.Log("Third pipe was blown");
                playStoneSound = true;
                foreach (MovableObject movableObject in movableObjects)
                {
                    if (movableObject.isRotation)
                        break;

                    if (!movableObject.isActivated)
                        movableObject.ActivateObject();
                    else
                        movableObject.DeactivateObject();
                }
                break;

            case 4: // fluit pijp 4
                Debug.Log("Fourth pipe was blown");

                // Another functionality can be added here...

                break;
        }
    }
    #endregion
}
