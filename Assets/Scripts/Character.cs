using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
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
    [SerializeField] Animator anim;

    [HideInInspector] public int xPos, yPos, zPos;
    [HideInInspector] public Vector3Int position;
    [HideInInspector] public bool isMoving;
    [HideInInspector] public int currentPathID;
    [HideInInspector] public bool boundToPath;

    private List<Transform> nodes = new List<Transform>();
    private MeshRenderer meshRenderer;

    private bool isGrounded = true;
    private CapsuleCollider capsuleCollider;
    private Vector3 lostNodePotition;

    private Rigidbody rb;
    #endregion

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
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
        Debug.Log("boundToPath: " + boundToPath + ", isGrounded: " + isGrounded);
        UpdatePosition();

        if (Input.GetKeyDown(KeyCode.Space))
            FlipDirection();

        GroundCheck();
        if (transform.position.y < diesBelowYLevel)
            deathBehaviour();

        PlayAnimations();

        if (isGrounded)
        {
            Invoke("SwitchOffGravity", 0.2f);
        }
        else
        {
            CancelInvoke("SwitchOffGravity");
            rb.useGravity = true;
        }
    }

    private void FixedUpdate()
    {
        //StartCoroutine(CheckForMovement());
        if (isGrounded)
            Move();
    }

    private void PlayAnimations()
    {
        if (!isGrounded)
            anim.SetBool("IsFalling", true);

        if (isGrounded)
            anim.SetBool("IsFalling", false);
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
        isGrounded = Physics.SphereCast(transform.position + (transform.rotation * (Vector3.up * capsuleCollider.height / 2 + capsuleCollider.center)), transform.localScale.x / 8, -transform.up, out RaycastHit hit, capsuleCollider.height + 0.1f, LayerMask.GetMask("Terrain")); // Layermask 7 is terrain
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
        // If char is on node then...
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 0.05f)
        {
            // First, if we stand still then we don't do anything
            if (direction == Direction.stationary)
            {
                return;
            }
            // If there was no wall, and the current node is a last/first node, then...
            else
            {
                // If the character is on the last node and is moving forward
                if (currentNode == nodes.Count - 1 && direction == Direction.forward)
                {
                    // If the char is on the last node of a path, he should lose this path if he didn't turn around earlier
                    LosePathing();
                }

                // Or if the character is on the first node and is moving backwards
                else if (currentNode == 0 && direction == Direction.backward)
                {
                    // Again, lose the path
                    LosePathing();
                }
            }

            // If the char lost the path, then we don't have to increase/decrease to update the current since there is none
            if (!boundToPath) return;

            // And if the char is on the path, then this will decide, based of direction, where to go next
            if (direction == Direction.forward)
                currentNode++;
            else if (direction == Direction.backward)
                currentNode--;
        }
    }

    private void LosePathing()
    {
        lostNodePotition = GetNodePosition(currentNode);
        boundToPath = false;
        rb.useGravity = true;
    }

    private void SwitchOffGravity()
    {
        rb.useGravity = false;
    }

    private void SearchForPath()
    {
        // IF WE USE UBEAT TEMPO, THIS SEARCHING AND BINDING TO A PATH HAS TO COME IN PULSES INSTEAD OF EVERY UPDATE
        if (Vector3.Distance(lostNodePotition, transform.position) < 0.99f) return;

        RaycastHit hit;
        float minCastdistance = 1f;
        float castScale = transform.localScale.x / 2;
        Physics.SphereCast(transform.position + (transform.rotation * (Vector3.up * capsuleCollider.height / 2 + capsuleCollider.center)), castScale, -transform.up, out hit, minCastdistance, LayerMask.GetMask("Path"), QueryTriggerInteraction.UseGlobal);
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
            direction = Direction.stationary;
            Debug.Log("Found the path");

            if (rb.useGravity)
            {
                StartCoroutine(StartWalkingAgain(foundPath.preferedDirection, 5.5f));
            }
            else
            {
                StartWalkingAgain(foundPath.preferedDirection);
            }
        }
    }

    private void StartWalkingAgain(Direction newDirection)
    {
        direction = newDirection;
    }

    private IEnumerator StartWalkingAgain(Direction newDirection, float afterSeconds)
    {
        yield return new WaitForSeconds(afterSeconds);
        direction = newDirection;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("MoveableObject"))
        {
            // If a moving object hits the player then it should lose his pathing
            LosePathing();
        }
    }
}

public enum Direction { forward, stationary, backward }