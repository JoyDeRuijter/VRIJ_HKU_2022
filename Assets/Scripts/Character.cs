using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Character : MonoBehaviour
{
    #region Variables

    [Header("Character Material")]
    [SerializeField] Material material;

    [Space(10)]
    [Header("Character Movement & Pathfinding")]
    [SerializeField] private float movementSpeed = 1.2f;
    public Transform path;
    public int currentNode;
    public Direction direction;
    [SerializeField] KeyCode directionChangeKey;
    private Direction lastDirection;

    [Space(10)]
    [Header("Other...")]
    [SerializeField] LayerMask onlyPathLayer;
    [SerializeField] Animator anim;
   
    [HideInInspector] public int xPos, yPos, zPos;
    [HideInInspector] public Vector3Int position;
    [HideInInspector] public bool isMoving;
    [HideInInspector] public int currentPathID;
    [HideInInspector] public bool boundToPath;

    private List<Transform> nodes = new List<Transform>();
    private MeshRenderer meshRenderer;

    private bool canFindPath = false;
    private bool isGrounded = true;
    public static bool isFloating = false;

    #endregion

    private void Awake()
    {
        UpdatePosition();

        //Set the material of the whole object to the material provided in the inspector
        //meshRenderer = GetComponentInChildren<MeshRenderer>();
        //meshRenderer.material = material;
    }

    private void Start()
    {
        InitializePath();
        currentPathID = GetPathID();

        lastDirection = Direction.forward;

        //anim.SetBool("IsWalking", true);
    }

    private void Update()
    {

        UpdatePosition();

        if (Input.GetKeyDown(KeyCode.Space))
            FlipDirection();

        GroundCheck();
    }

    private void FixedUpdate()
    {
        StartCoroutine(CheckForMovement());
        Move();
    }

    #region Helper Functions

    private void UpdatePosition()
    {
        xPos = (int)transform.position.x;
        yPos = (int)((transform.position.y) - 0.6f);
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

    private int GetPathID()
    {
        return path.gameObject.GetComponent<NodePath>().ID;
    }

    public Vector3 GetNodePosition(int _node)
    {
        return nodes[_node].position;
    }

    #endregion

    #region Movement & Pathfinding

    // Initialize the first path the character has to follow
    private void InitializePath()
    {
        if (direction == Direction.stationary)
            return;

        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        
        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
                nodes.Add(pathTransforms[i]);
        }
    }

    private void Move() 
    {
        if (boundToPath)
        {
            CheckWaypointDistance();
            MoveToNode();
        }
        else
        {
            SearchForPath();

            if (isGrounded)
                MoveFreely();
        }
    }

    // Make the character move towards the current node position
    private void MoveToNode()
    {
        if (direction != Direction.stationary)
        {
            transform.position = Vector3.MoveTowards(transform.position, nodes[currentNode].position, Time.deltaTime * movementSpeed);
            transform.LookAt(nodes[currentNode].position);
            transform.rotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0);
        }
    }

    private void MoveFreely()
    {
        transform.position += transform.forward * Time.deltaTime * movementSpeed;
    }

    private void GroundCheck()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if (Physics.SphereCast(ray, transform.localScale.x - 0.1f, out hit, 0.7f))
            isGrounded = true;
        else
        {
            if (!isFloating)
                isGrounded = false;
        }
    }

    // Make the character change directions and move to the nodes in opposite order
    private void FlipDirection()
    {
        if (direction == Direction.stationary)
        {
            if (lastDirection == Direction.forward)
                direction = Direction.backward;
            else
                direction = Direction.forward;
        }

        if (direction == Direction.forward)
        {
            direction = Direction.backward;
            lastDirection = Direction.forward;
        }
        else if (direction == Direction.backward)
        {
            direction = Direction.forward;
            lastDirection = Direction.backward;
        }
    }

    // Check the distance between the character and the currentNode, and move to the next node if it's close enough
    private void CheckWaypointDistance()
    {
        if (direction == Direction.stationary)
            return;

        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 0.05f)
        {
            if (currentNode == nodes.Count - 1 && direction == Direction.forward) // If the character is on the last node and is moving forward
            {
                LastNodeBehaviour();
            }
            else if (currentNode == 0 && direction == Direction.backward) // If the character is on the first node and is moving backwards
            {
                LastNodeBehaviour();
            }

            if (!boundToPath) return;

            if (direction == Direction.forward)
                currentNode++;
            else if (direction == Direction.backward)
                currentNode--;
        }
    }

    private void LastNodeBehaviour()
    {
        // CAUSES PLAYER TO STOP WALKING AFTER ONE NODE IF THEY LANDED ON A LAST/FIRST NODE
        // EITHER FIX THIS OR NEVER LET A PLAYER LAND ON A LAST/FIST NODE
        if (WallCheck(0.6f))
            FlipDirection();
        else
        {
            boundToPath = false;
            Invoke("PathSearchingDelay", 1f);
        }
    }

    private void PathSearchingDelay()
    {
        canFindPath = true;
    }

    private void SearchForPath()
    {
        // IF WE USE UBEAT TEMPO, THIS SEARCHING AND BINDING TO A PATH HAS TO COME IN PULSES INSTEAD OF EVERY UPDATE
        if (!canFindPath) return;

        RaycastHit hit;
        float minCastdistance = 1f;
        float castScale = 0.5f;
        Physics.SphereCast(transform.position, castScale, -transform.up, out hit, minCastdistance, onlyPathLayer, QueryTriggerInteraction.UseGlobal);
        if (hit.collider != null)
        {
            NodePath foundPath = hit.transform.GetComponentInParent<NodePath>();
            nodes = foundPath.nodes;
            currentPathID = foundPath.ID;
            string[] nodeNameArray = hit.transform.name.Split('_');
            currentNode = int.Parse(nodeNameArray[nodeNameArray.Length - 1]);
            transform.position = GetNodePosition(currentNode);
            boundToPath = true;
            canFindPath = false;
            direction = Direction.stationary;
            Debug.Log("I seem to have found my way again!");
        }
    }

    private bool WallCheck(float castDistance)
    {
        if (Physics.Raycast(transform.position, transform.forward, castDistance))
        {
            Debug.Log("Hmm... seems there is a wall here");
            return true;
        }
        Debug.Log("Oh Lord... I'm lost...");
        return false;
    }

    #endregion

}

    public enum Direction { forward, stationary, backward }