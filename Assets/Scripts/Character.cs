using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class Character : MonoBehaviour
{
    #region Variables
    // Bool needed by effect tile for floating effect
    public static bool isFloating = false;

    [Space(10)]
    [Header("Character Movement & Pathfinding")]
    [SerializeField] private float movementSpeed = 1.2f;
    public Transform path;
    public int currentNode;
    public WalkDirection direction;
    private WalkDirection lastDirection;
    [SerializeField] bool toTheBeat = false;

    [Space(10)]
    [Header("Other...")]
    [SerializeField] Animator anim;

    [HideInInspector] public int currentPathID;
    [HideInInspector] public bool boundToPath;
    [HideInInspector] public NodePath nodePath;

    private List<Transform> nodes = new List<Transform>();

    private bool isGrounded = true;
    private CapsuleCollider capsuleCollider;
    private bool justFell = false;
    private bool routineIsRunning = false;
    [HideInInspector] public bool isMoving;
    private Vector3 lostPathNodePosition;
    private float minDistanceBetweenPoints = 0.05f;

    private float heightOfCurrentNodeRelativeToCharacter;

    private Rigidbody rb;
    #endregion

    #region Awake/Start/Update etc...
    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        InitializePath();
        currentPathID = nodePath.ID;

        lastDirection = WalkDirection.forward;

        if (toTheBeat) isMoving = false;
        else isMoving = true;

        //anim.SetBool("IsWalking", true);
    }

    private void Update()
    {
        // Is only for debugging, will be removed later
        if (Input.GetKeyDown(KeyCode.Space))
            FlipDirection();

        PlayAnimations();

        // Do a check to find the ground, if there is no ground then the player is falling
        GroundCheck();
        // If he lands then...
        if (!isGrounded)
            justFell = true;

        // then perform a function with a delay to play the landing animation
        if (justFell && isGrounded && !routineIsRunning)
        {
            routineIsRunning = true;
            StartCoroutine(StartWalkingAgain(WalkDirection.stationary, 5.5f));
        }

        // If we want to walk stairs (even if we are on walls on the side) then we want to turn off the gravity
        // Here we start of with a method to determine if the vector towards the next node is facing up or down
        // relative to the character
        Vector3 vector1 = nodePath.GetNodeFloorPointPosition(currentNode) - GetCharacterFeet();
        Vector3 vector2 = vector1 + transform.up;
        float distance1 = Vector3.Magnitude(vector1);
        float distance2 = Vector3.Magnitude(vector2);
        int posNegMultiplier;
        if (distance1 < distance2)
            posNegMultiplier = 1;
        else
            posNegMultiplier = -1;

        // Then we aply a calculation to determine the heigt of the next node (again relative to the character) and
        // multiply it by +1 or -1 based on the vector facing up or down
        heightOfCurrentNodeRelativeToCharacter = heightRelativeToTransformVector(GetCharacterFeet(), 
            nodePath.GetNodeFloorPointPosition(currentNode), transform.forward) * posNegMultiplier;
        if (heightOfCurrentNodeRelativeToCharacter > 0.05f && isGrounded && !justFell && boundToPath)
        {
            UseGravity(false);
        }
        else if (Mathf.Abs(heightOfCurrentNodeRelativeToCharacter) > 1.5f && boundToPath)
        {
            LosePathing();
        }
        else
        {
            UseGravity(true);
        }
    }

    private void FixedUpdate()
    {
        // If the player is on the ground (and the landing animation has finished), then we can move (again)
        if (isGrounded && !justFell)
            Move();
    }
    #endregion

    #region Player animations
    private void PlayAnimations()
    {
        if (!isGrounded)
            anim.SetBool("IsFalling", true);

        if (isGrounded)
            anim.SetBool("IsFalling", false);
    }
    #endregion

    #region Helper Functions
    private float heightRelativeToTransformVector(Vector3 relativeVector, Vector3 otherVector, Vector3 transformDirection)
    {
        Vector3 resultVector = otherVector - relativeVector;
        float angle = Vector3.Angle(transformDirection, resultVector);
        return Mathf.Sin(angle * Mathf.Deg2Rad) * Vector3.Magnitude(resultVector);
    }

    private Vector3 relativeToGravityHorizontalDirectionVector(Vector3 a, Vector3 b)
    {
        float angle = Vector3.Angle(-Physics.gravity, b - a);
        float distanceCloseToAngleVector = Mathf.Cos(angle * Mathf.Deg2Rad) * Vector3.Distance(a, b);
        Vector3 heightVector = Physics.gravity.normalized * distanceCloseToAngleVector;
        return (b - a) + heightVector;
    }
    
    private float CheckHorizontalDistanceToNode()
    {
        if (boundToPath)
        {
            return Vector3.Distance(Vector3.zero, relativeToGravityHorizontalDirectionVector(GetCharacterFeet(), nodePath.GetNodeFloorPointPosition(currentNode)));
        }
        else
        {
            return Vector3.Distance(Vector3.zero, relativeToGravityHorizontalDirectionVector(GetCharacterFeet(), lostPathNodePosition));
        }
    }

    public void UseGravity(bool condition)
    {
        rb.useGravity = condition;
    }

    public void deathBehaviour()
    {
        UserInterface.reloadScene();
    }

    private void GroundCheck()
    {
        if (Physics.SphereCast(GetCharacterTop(), (capsuleCollider.radius / 2) * 0.99f, -transform.up, out RaycastHit hit, capsuleCollider.height + 0.3f, LayerMask.GetMask("Terrain")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
            justFell = true;
        }
    }

    private Vector3 GetCharacterFeet()
    {
        return transform.position + (transform.rotation * (Vector3.down * capsuleCollider.height / 2 + capsuleCollider.center));
    }

    private Vector3 GetCharacterTop()
    {
        return transform.position + (transform.rotation * (Vector3.up * capsuleCollider.height / 2 + capsuleCollider.center));
    }

    private bool WallCheck(float castDistance)
    {
        if (Physics.Raycast(transform.position - (transform.up * capsuleCollider.height * 0.25f), transform.forward, castDistance))
        {
            return true;
        }
        return false;
    }
    #endregion

    #region Movement
    private void Move()
    {
        // When the character has a path to follow then we check for each upcomming node and move/rotate towards them
        if (boundToPath)
        {
            CheckWaypointDistance();
            if (isMoving)
            {
                MoveToNode();
                RotateTowardsNode();
            }
        }
        // When we don't have a path, we move in a straigt line from where we left, and look for a new path
        else
        {
            transform.rotation = Quaternion.LookRotation(transform.forward, -Physics.gravity);
            if (isGrounded)
            {
                SearchForPath();
                MoveFreely();
            }
        }
    }

    private IEnumerator StartWalkingAgain(WalkDirection newDirection, float afterSeconds)
    {
        yield return new WaitForSeconds(afterSeconds);
        justFell = false;
        direction = newDirection;
        routineIsRunning = false;
    }

    // Make the character move towards the current node position
    private void MoveToNode()
    {
        transform.position = Vector3.MoveTowards(transform.position, nodes[currentNode].position, Time.deltaTime * movementSpeed);
    }

    private void RotateTowardsNode()
    {
        if (nodes.Count < 2)
            return;

        Vector3 walkingDirectionVector;
        if (direction == WalkDirection.forward)
        {
            if (currentNode == 0)
            {
                //walkingDirectionVector = (nodePath.GetNodeFloorPointPosition(currentNode + 1) - nodePath.GetNodeFloorPointPosition(currentNode)).normalized;
                walkingDirectionVector = relativeToGravityHorizontalDirectionVector(nodes[currentNode].position, nodes[currentNode + 1].position);
            }
            else
            {
                //walkingDirectionVector = (nodePath.GetNodeFloorPointPosition(currentNode) - nodePath.GetNodeFloorPointPosition(currentNode - 1)).normalized;
                walkingDirectionVector = relativeToGravityHorizontalDirectionVector(nodes[currentNode - 1].position, nodes[currentNode].position);
            }
        }
        else
        {
            if (currentNode == nodes.Count - 1)
            {
                //walkingDirectionVector = (nodePath.GetNodeFloorPointPosition(currentNode - 1) - nodePath.GetNodeFloorPointPosition(currentNode)).normalized;
                walkingDirectionVector = relativeToGravityHorizontalDirectionVector(nodes[currentNode].position, nodes[currentNode - 1].position);
            }
            else
            {
                //walkingDirectionVector = (nodePath.GetNodeFloorPointPosition(currentNode) - nodePath.GetNodeFloorPointPosition(currentNode + 1)).normalized;
                walkingDirectionVector = relativeToGravityHorizontalDirectionVector(nodes[currentNode + 1].position, nodes[currentNode].position);
            }
        }

        Debug.DrawLine(transform.position, transform.position + walkingDirectionVector);

        Quaternion targetRotation = Quaternion.LookRotation(walkingDirectionVector, -Physics.gravity.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 50f);
    }

    private void MoveFreely()
    {
        transform.position += transform.forward * Time.deltaTime * movementSpeed;
    }

    // Make the character change directions and move to the nodes in opposite order
    public void FlipDirection()
    {
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
        if ((currentNode == nodes.Count - 1 && Vector3.Distance(transform.position, nodes[currentNode].position) >= minDistanceBetweenPoints) || currentNode != nodes.Count - 1)
        {
            direction = WalkDirection.forward;
            if (nodePath.isLoop && currentNode == nodes.Count - 1)
                currentNode = 0;
            else
                currentNode++;
        }
    }

    private void flipToBackward()
    {
        if ((currentNode == 0 && Vector3.Distance(transform.position, nodes[currentNode].position) >= minDistanceBetweenPoints) || currentNode != 0)
        {
            direction = WalkDirection.backward;
            if (nodePath.isLoop && currentNode == 0)
                currentNode = nodes.Count - 1;
            else
                currentNode--;
        }
    }

    #endregion

    #region Pathfinding
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

    // Check the distance between the character and the currentNode, and move to the next node if it's close enough
    private void CheckWaypointDistance()
    {
        // If char is on node then...
        if (CheckHorizontalDistanceToNode() < minDistanceBetweenPoints)
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
                    if (nodePath.isLoop)
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
                    if (nodePath.isLoop)
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
                currentNode++;
            }
            else if (direction == WalkDirection.backward)
            {
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
        lostPathNodePosition = nodePath.GetNodeFloorPointPosition(currentNode);
        boundToPath = false;
    }

    private void SearchForPath()
    {
        if (CheckHorizontalDistanceToNode() < 0.9f || GravityTowardsPoint.boundToPoint || !isGrounded)
            return;

        RaycastHit hit;
        float castScale = capsuleCollider.radius / 2 - 0.01f;
        Physics.SphereCast(GetCharacterTop(), castScale, -transform.up, out hit, capsuleCollider.height + 0.1f, LayerMask.GetMask("Path"), QueryTriggerInteraction.UseGlobal);
        if (hit.collider != null)
        {
            Debug.Log("Path found!");
            nodePath = hit.transform.GetComponentInParent<NodePath>();
            nodes = nodePath.nodes;
            currentPathID = nodePath.ID;
            string temp = hit.transform.name;
            temp = temp.Remove(0, 6);
            temp = temp.Remove(temp.Length - 1);
            currentNode = int.Parse(temp);
            boundToPath = true;
            direction = WalkDirection.stationary;

            Physics.gravity = nodePath.gravityDirection * GravityTowardsPoint.gravityStrenght;

            WalkDirection startingPathDirection;

            if (currentNode == nodes.Count - 1)
            {
                startingPathDirection = WalkDirection.backward;
            }
            else if (currentNode == 0)
            {
                startingPathDirection = WalkDirection.forward;
            }
            else if (nodePath.preferedDirection == WalkDirection.stationary)
            {
                startingPathDirection = lastDirection;
            }
            else
            {
                startingPathDirection = nodePath.preferedDirection;
            }
            if (justFell)
                StartCoroutine(StartWalkingAgain(startingPathDirection, 5.5f));
            else
                direction = startingPathDirection;
        }
    }
    #endregion

    public void MoveNext()
    {
        // If toTheBeat is enabled, then when this function is called, the char moves to the next node (if it's not lost of course)
        isMoving = true;
        direction = lastDirection;
    }

    private bool delayActive = false;
    private void OnCollisionStay(Collision collision)
    {
        MovableObject obj = collision.gameObject.GetComponentInParent<MovableObject>();
        if (obj != null && !delayActive)
        {
            delayActive = true;
            Invoke("waitTillNewCheck", 0.1f);
            if (WallCheck(capsuleCollider.radius + 0.1f))
                FlipDirection();
        }

        if (obj != null)
        {
            if (Mathf.Abs(heightOfCurrentNodeRelativeToCharacter) > 0.5f)
                LosePathing();
        }
    }

    private void waitTillNewCheck()
    {
        delayActive = false;
    }
}

public enum WalkDirection { forward, stationary, backward }