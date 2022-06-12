using UnityEngine;

[SelectionBase]
public class PlayerCamera : MonoBehaviour
{
    #region Variables
    [Header("Camera information")]
    [Range(10f, 50f), SerializeField] float camSpeed;
    [SerializeField] GameObject camNodesObject;
    [SerializeField] float minCamHeight;
    [SerializeField] float maxCamHeight;
    [SerializeField] int startingNode;

    [Header("Camera looks at player or a centerObject")]
    [SerializeField] bool looksAtPlayer;
    [SerializeField] Transform centerObject;

    [Header("Camera options")]
    [SerializeField] bool isRotating;
    //[SerializeField] bool nodeFollow;
    [SerializeField] float rotationOffset;
    [SerializeField] float rotationSpeed;
    [SerializeField] GameObject cameraNodePath;
    [SerializeField] Transform COM;
    [SerializeField, Range(0f, 5f)] float camHeight;
    [SerializeField] float minDistanceToNode;

    private float smallestDistance;
    private Transform closestNode;
    Vector3 zero = Vector3.zero;

    public static Transform characterTransform = null;
    public static bool playerIsActive = false;
    private float camPlayerHeightOffset = 0;
    private Vector3 activePosition = Vector3.zero;
    #endregion

    private void Start()
    {
        COM.position = centerObject.position;
        transform.position = camNodesObject.GetComponent<CameraNodes>().nodes[startingNode].position;
    }

    private void Update()
    {
        if (playerIsActive)
        {
            activePosition = characterTransform.position;
        }

        // The camera transforms towards the closest node
        CameraNodes camNodes = camNodesObject.GetComponent<CameraNodes>();
        smallestDistance = Vector3.Distance(camNodes.nodes[0].position, activePosition);
        closestNode = camNodes.nodes[0];

        foreach (Transform node in camNodes.nodes)
        {
            float newDistance = Vector3.Distance(node.position, activePosition);
            if (newDistance < smallestDistance)
            {
                smallestDistance = newDistance;
                closestNode = node;
            }
        }

        // Rotate around object
        if (Vector3.Distance(activePosition, closestNode.position) < minDistanceToNode)
        {
            CamMoveToNode();
        }
        else
        {
            CamRotateToNode();
        }

        if (Physics.gravity.y < 0)
        {
            camPlayerHeightOffset = camHeight;
        }
        else if (Physics.gravity.y > 0)
        {
            camPlayerHeightOffset = -camHeight;
        }
        else
        {
            camPlayerHeightOffset = 0;
        }

        if (looksAtPlayer)
            transform.LookAt(activePosition);
        else
            transform.LookAt(centerObject.transform.position);
    }

    private void CamMoveToNode()
    {
        transform.position = Vector3.SmoothDamp(transform.position, closestNode.position, ref zero, Time.deltaTime * camSpeed);
    }

    private void CamRotateToNode()
    {
        Vector3 targetVector = GetHorizontalVector(centerObject.position, activePosition);
        COM.position = Vector3.SmoothDamp(COM.position, Vector3.up * activePosition.y + Vector3.up * camPlayerHeightOffset, ref zero, Time.deltaTime * camSpeed);
        COM.rotation = Quaternion.Slerp(COM.rotation, Quaternion.LookRotation(targetVector, Vector3.up), rotationSpeed * Time.deltaTime);
        float distanceToCharacter = Vector3.Distance(centerObject.position, activePosition);
        if (playerIsActive)
        {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.forward * rotationOffset + Vector3.forward * distanceToCharacter, ref zero, Time.deltaTime * camSpeed);
        }
        else
        {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.forward * (2 *rotationOffset) + Vector3.forward * distanceToCharacter, ref zero, Time.deltaTime * camSpeed);
        }
    }

    private Vector3 GetHorizontalVector(Vector3 a, Vector3 b)
    {
        Vector3 resultvector = b - a;
        return new Vector3(resultvector.x, 0, resultvector.z);
    }
}
