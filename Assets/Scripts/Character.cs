using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Character : MonoBehaviour
{
    #region Variables

    [Header("Character Material")]
    [SerializeField] Material material;

    [Header("Character Movement & Pathfinding")]
    [Space(10)]
    [SerializeField] private float movementSpeed = 1.2f;
    public Transform path;
   
    [HideInInspector] public int xPos, yPos, zPos;
    [HideInInspector] public Vector3Int position;
    [HideInInspector] public bool isMoving;

    private List<Transform> nodes = new List<Transform>();
    private MeshRenderer meshRenderer;
    private int currentNode = 0;

    private enum Direction { forward, stationary, backward}
    private Direction direction;

    #endregion

    private void Awake()
    {
        UpdatePosition();

        //Set the material of the whole object to the material provided in the inspector
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = material;
    }

    private void Start()
    {
        InitializePath();
        direction = Direction.forward;
    }

    private void Update()
    {
        UpdatePosition();
    }

    private void FixedUpdate()
    {
        CheckForMovement();
        Move();
        CheckWaypointDistance();
    }

    #region Helper Functions

    private void UpdatePosition()
    {
        xPos = (int)transform.position.x;
        yPos = (int)(transform.position.y - 5.1f);
        zPos = (int)transform.position.z;
        position = new Vector3Int(xPos, yPos, zPos);
    }

    private IEnumerator CheckForMovement()
    {
        Vector3 startPosition = transform.position;
        yield return new WaitForSeconds(0.3f);

        if (transform.position != startPosition)
            isMoving = true;
        else
            isMoving = false;

    }

    #endregion

    #region Collision With Cubes

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Cube>() != null && collision.gameObject.GetComponent<Cube>().isWalkable)
            collision.gameObject.GetComponent<Cube>().SetTouchedMaterial();
        // Do the same for stairs later: collision.gameObject.transform.parent.GetComponent<Stairs>() != null
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<Cube>() != null && collision.gameObject.GetComponent<Cube>().isWalkable)
            collision.gameObject.GetComponent<Cube>().SetNormalMaterial();
        // Do the same for stairs later: collision.gameObject.transform.parent.GetComponent<Stairs>() != null
    }

    #endregion

    #region Movement & Pathfinding

    // Initialize the first path the character has to follow
    private void InitializePath()
    {
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        
        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
                nodes.Add(pathTransforms[i]);
        }
    }

    // Make the character move towards the current node position
    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, nodes[currentNode].position, Time.deltaTime * movementSpeed);
    }

    // Make the character change directions and move to the nodes in opposite order
    private void FlipDirection()
    {
        if (direction == Direction.forward)
            direction = Direction.backward;
        else if (direction == Direction.backward)
            direction = Direction.forward;
    }

    // Check the distance between the character and the currentNode, and move to the next node if it's close enough
    private void CheckWaypointDistance()
    {
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 0.05f)
        {
            if (currentNode == nodes.Count - 1 && direction == Direction.forward) // If the character is on the last node and is moving forward
                // Later on add falling of the platform here when the character is on a ledge
                FlipDirection();
            else if (currentNode == 0 && direction == Direction.backward) // If the character is on the first node and is moving backwards
                // Later on add falling of the platform here when the character is on a ledge
                FlipDirection();

            if (direction == Direction.forward)
                currentNode++;
            else if (direction == Direction.backward)
                currentNode--;
        }
    }

    #endregion

}
