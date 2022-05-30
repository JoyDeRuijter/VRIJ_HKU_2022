using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    #region Variables

    public static bool isFloating = false;

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
    [SerializeField] float diesBelowYLevel;

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
    private bool isFalling;
    private bool isWalking = true;

    private Rigidbody rb;
    #endregion

    private void Awake()
    {
        UpdatePosition();

        //Set the material of the whole object to the material provided in the inspector
        //meshRenderer = GetComponentInChildren<MeshRenderer>();
        //meshRenderer.material = material;
        rb = GetComponent<Rigidbody>();
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
        if (transform.position.y < diesBelowYLevel)
            deathBehaviour();
    }

    private void FixedUpdate()
    {
        //StartCoroutine(CheckForMovement());
        Move();
        PlayAnimations();

    }

    private void PlayAnimations()
    {
        if (rb.velocity.y <= -0.1f)
        {
            isFalling = true;
            isWalking = false;
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsFalling", true);
        }
        if (isFalling && rb.velocity.y >= 0)
        { 
            isFalling = false;
            anim.SetBool("IsFalling", false);
        }
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

    public void deathBehaviour()
    {
        UserInterface.reloadScene();
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
        transform.position = Vector3.MoveTowards(transform.position, nodes[currentNode].position, Time.deltaTime * movementSpeed);
        transform.LookAt(nodes[currentNode].position);
        transform.rotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0);
    }

    private void MoveFreely()
    {
        transform.position += transform.forward * Time.deltaTime * movementSpeed;
    }

    private void GroundCheck()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        if (Physics.SphereCast(ray, transform.localScale.x / 2, 0.7f))
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
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 0.05f)
        {
            if (direction == Direction.stationary)
            {
                return;
            }
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
        // EITHER FIX THIS OR NEVER LET A PLAYER LAND ON A LAST/FIRST NODE
        if (WallCheck(0.6f))
            FlipDirection();
        else
        {
            boundToPath = false;
            rb.useGravity = true;
            Invoke("PathSearchingDelay", 1.5f);
        }
    }

    private void PathSearchingDelay()
    {
        canFindPath = true;
    }

    private void SwitchOffGravity()
    {
        rb.useGravity = false;
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
            string temp = hit.transform.name;
            temp = temp.Remove(0, 6);
            temp = temp.Remove(temp.Length - 1);
            currentNode = int.Parse(temp);
            boundToPath = true;
            canFindPath = false;
            direction = Direction.stationary;
            Invoke("SwitchOffGravity", 0.2f);
            isWalking = true;
            anim.SetBool("IsWalking", true);
            Invoke("StartWalkingAgain", 6f);
        }
    }

    private void StartWalkingAgain()
    {
        direction = Direction.forward;
    }

    private bool WallCheck(float castDistance)
    {
        if (Physics.Raycast(transform.position, transform.forward, castDistance))
        {
            return true;
        }
        return false;
    }

    #endregion

}

public enum Direction { forward, stationary, backward }