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
    public WalkDirection direction;
    [SerializeField] KeyCode directionChangeKey;
    private WalkDirection lastDirection;
    [SerializeField] float diesBelowYLevel;
    [SerializeField] bool toTheBeat;

    [Space(10)]
    [Header("Other...")]
    [SerializeField] Animator anim;

    [HideInInspector] public int xPos, yPos, zPos;
    [HideInInspector] public Vector3Int position;
    [HideInInspector] public int currentPathID;
    [HideInInspector] public bool boundToPath;

    private List<Transform> nodes = new List<Transform>();
    private MeshRenderer meshRenderer;

    private bool isGrounded = true;
    private CapsuleCollider capsuleCollider;
    private bool justFell = false;
    private bool routineIsRunning = false;
    [HideInInspector] public bool isMoving;
    private Vector3 lostPathNodePosition;
    private int lastNode;

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

        lastDirection = WalkDirection.forward;

        if (toTheBeat) isMoving = false;
        else isMoving = true;

        //anim.SetBool("IsWalking", true);
    }

    private void Update()
    {
        Debug.Log("boundToPath: " + boundToPath + ", PathID: " + currentPathID + ", isGrounded: " + isGrounded);
        UpdatePosition();

        if (Input.GetKeyDown(KeyCode.Space))
            FlipDirection();

        GroundCheck();
        if (transform.position.y < diesBelowYLevel)
            deathBehaviour();

        PlayAnimations();

        if (isGrounded)
        {
            if (GetVerticalDistance(nodes[currentNode].position, nodes[lastNode].position) > 0.2f && nodes[currentNode].position.y > nodes[lastNode].position.y)
            {
                rb.useGravity = false;
            }
            else
            {
                rb.useGravity = true;
            }
        }
        else
        {
            rb.useGravity = true;
            justFell = true;
        }

        if (GetVerticalDistance(transform.position, GetNodePosition(currentNode)) > 1.5f)
            LosePathing();

        if (justFell && isGrounded && !routineIsRunning)
        {
            routineIsRunning = true;
            StartCoroutine(StartWalkingAgain(WalkDirection.stationary, 5.5f));
        }
    }
    private IEnumerator StartWalkingAgain(WalkDirection newDirection, float afterSeconds)
    {
        yield return new WaitForSeconds(afterSeconds);
        justFell = false;
        direction = newDirection;
        routineIsRunning = false;
    }

    private void FixedUpdate()
    {
        if (isGrounded && !justFell)
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

    private int GetPathID()
    {
        return path.gameObject.GetComponent<NodePath>().ID;
    }

    public Vector3 GetNodePosition(int _node)
    {
        return nodes[_node].position;
    }

    public bool isPathLooped()
    {
        return path.gameObject.GetComponent<NodePath>().isLoop;
    }

    public void deathBehaviour()
    {
        UserInterface.reloadScene();
    }

    private float GetHorizontalDistance(Vector3 a, Vector3 b)
    {
        Vector3 newA = new Vector3(a.x, 0, a.z);
        Vector3 newB = new Vector3(b.x, 0, b.z);
        return Vector3.Distance(newA, newB);
    }

    private float GetVerticalDistance(Vector3 a, Vector3 b)
    {
        Vector3 newA = new Vector3(0, a.y, 0);
        Vector3 newB = new Vector3(0, b.y, 0);
        return Vector3.Distance(newA, newB);
    }
    #endregion

    #region Movement & Pathfinding

    // Initialize the first path the character has to follow
    private void InitializePath()
    {
        if (direction == WalkDirection.stationary)
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
            if (isMoving) MoveToNode();
        }
        else
        {
            if (isGrounded)
            {
                SearchForPath();
                MoveFreely();
            }
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
        if (Physics.SphereCast(transform.position + (transform.rotation * (Vector3.up * capsuleCollider.height / 2 + capsuleCollider.center)), transform.localScale.x / 8, -transform.up, out RaycastHit hit, capsuleCollider.height + 0.3f, LayerMask.GetMask("Terrain"))) // Layermask 7 is terrain
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
            justFell = true;
        }

    }

    // Make the character change directions and move to the nodes in opposite order
    public void FlipDirection()
    {
        Debug.Log("Me flippin");
        if (direction == WalkDirection.stationary)
        {
            if (lastDirection == WalkDirection.forward)
                flipToBackward();
            else
                flipToForward();
        }

        if (direction == WalkDirection.forward)
        {
            flipToBackward();
        }
        else if (direction == WalkDirection.backward)
        {
            flipToForward();
        }
    }

    private void flipToForward()
    {
        if ((currentNode == nodes.Count - 1 && GetHorizontalDistance(transform.position, nodes[currentNode].position) >= 0.05f) || currentNode != nodes.Count - 1)
        {
            lastNode = currentNode;
            direction = WalkDirection.forward;
            if (isPathLooped() && currentNode == nodes.Count - 1)
                currentNode = 0;
            else
                currentNode++;
        }
    }

    private void flipToBackward()
    {
        if ((currentNode == 0 && GetHorizontalDistance(transform.position, nodes[currentNode].position) >= 0.05f) || currentNode != 0)
        {
            lastNode = currentNode;
            direction = WalkDirection.backward;
            if (isPathLooped() && currentNode == 0)
                currentNode = nodes.Count - 1;
            else
                currentNode--;
        }
    }

    // Check the distance between the character and the currentNode, and move to the next node if it's close enough
    private void CheckWaypointDistance()
    {
        // If char is on node then...
        if (GetHorizontalDistance(transform.position, nodes[currentNode].position) < 0.05f)
        {
            // First, if we stand still then we don't do anything
            if (direction == WalkDirection.stationary && !toTheBeat)
            {
                return;
            }
            // If we don't stand still then...
            else
            {
                // If the character is on the last or first node then...
                if (currentNode == nodes.Count - 1 && direction == WalkDirection.forward)
                {
                    if (isPathLooped())
                    {
                        currentNode = 0;
                    }
                    else
                    {
                        LastNodeBehaviour();
                        return;
                    }
                }
                else if (currentNode == 0 && direction == WalkDirection.backward)
                {
                    if (isPathLooped())
                    {
                        currentNode = nodes.Count - 1;
                    }
                    else
                    {
                        LastNodeBehaviour();
                        return;
                    }
                }
            }

            // And if the char is on the path, then this will decide, based of direction, where to go next
            if (direction == WalkDirection.forward)
            {
                lastNode = currentNode;
                currentNode++;
            }
            else if (direction == WalkDirection.backward)
            {
                lastNode = currentNode;
                currentNode--;
            }

            if (toTheBeat)
            {
                isMoving = false;
                lastDirection = direction;
                direction = WalkDirection.stationary;
            }
        }
    }

    private void LastNodeBehaviour()
    {
        if (WallCheck(1.0f))
            FlipDirection();
        else
            LosePathing();
    }

    private void LosePathing()
    {
        lostPathNodePosition = nodes[currentNode].position;
        boundToPath = false;
    }

    private void SearchForPath()
    {
        // IF WE USE UBEAT TEMPO, THIS SEARCHING AND BINDING TO A PATH HAS TO COME IN PULSES INSTEAD OF EVERY UPDATE
        if (GetVerticalDistance(transform.position, lostPathNodePosition) < 1.0f)
            if (GetHorizontalDistance(transform.position, lostPathNodePosition) < 0.9f)
                return;

        RaycastHit hit;
        float castScale = transform.localScale.x / 2 - 0.01f;
        Physics.SphereCast(transform.position + (transform.rotation * (Vector3.up * capsuleCollider.height / 2 + capsuleCollider.center)), castScale, -transform.up, out hit, capsuleCollider.height + 0.01f, LayerMask.GetMask("Path"), QueryTriggerInteraction.UseGlobal);
        Debug.DrawLine(transform.position + (transform.rotation * (Vector3.up * capsuleCollider.height / 2 + capsuleCollider.center)), transform.position - transform.up * 100, Color.yellow);
        if (hit.collider != null)
        {
            NodePath foundPath = hit.transform.GetComponentInParent<NodePath>();
            nodes = foundPath.nodes;
            currentPathID = foundPath.ID;
            string temp = hit.transform.name;
            temp = temp.Remove(0, 6);
            temp = temp.Remove(temp.Length - 1);
            currentNode = int.Parse(temp);
            lastNode = currentNode;
            boundToPath = true;
            direction = WalkDirection.stationary;

            if (justFell)
            {
                StartCoroutine(StartWalkingAgain(foundPath.preferedDirection, 5.5f));
            }
            else
            {
                if (foundPath.preferedDirection == WalkDirection.stationary)
                {
                    StartWalkingAgain(lastDirection);
                }
                StartWalkingAgain(foundPath.preferedDirection);
            }
        }
    }

    private void StartWalkingAgain(WalkDirection newDirection)
    {
        direction = newDirection;
    }

    private bool WallCheck(float castDistance)
    {
        Debug.DrawRay(transform.position, transform.forward, Color.yellow);
        if (Physics.Raycast(transform.position, transform.forward, castDistance))
        {
            Debug.Log("Seems there is a wall here...");
            return true;
        }
        return false;
    }
    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("MoveableObject"))
        {
            if (collision.transform.position.y + collision.transform.localScale.y / 2 > transform.position.y)
            {
                if (WallCheck(1.0f))
                {
                    FlipDirection();
                }
            }
        }
    }

    public void MoveNext()
    {
        // If toTheBeat is enabled, then when this function is called, the char moves to the next node (if it's not lost of course)
        isMoving = true;
        direction = lastDirection;
    }
}

public enum WalkDirection { forward, stationary, backward }