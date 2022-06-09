using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    #region Variables
    [Header("Camera information")]
    [Range(0f, 0.5f), SerializeField] float camSpeed;
    [SerializeField] GameObject camNodesObject;
    [SerializeField] float minCamHeight;
    [SerializeField] float maxCamHeight;
    [SerializeField] int startingNode;

    [Header("Camera looks at player or a centerObject")]
    [SerializeField] bool looksAtPlayer;
    [SerializeField] Transform centerObject;

    [Header("Camera options")]
    [SerializeField] bool isRotating;
    [SerializeField] bool nodeFollow;
    [SerializeField] float rotationOffset;
    [SerializeField] float rotationSpeed;
    [SerializeField] GameObject cameraCenterOfMass;
    [SerializeField] GameObject cameraNodePath;
    [SerializeField] GameObject camFollowObject;

    private float smallestDistance;
    private Transform closestNode;
    Vector3 zero = Vector3.zero;

    public static Transform characterTransform = null;
    public static bool playerIsActive = false;
    private float camPlayerHeightOffset = 0;
    #endregion

    private void Start()
    {
        //camNodesObject.transform.position = new Vector3(camNodesObject.transform.position.x, characterTransform.position.y, camNodesObject.transform.position.z);
        //transform.position = camNodesObject.GetComponent<CameraNodes>().nodes[startingNode].position;
    }

    private void Update()
    {
        if (!playerIsActive) return;

        cameraNodePath.transform.position = new Vector3(0, characterTransform.position.y, 0);

        // The camera transforms towards the closest node
        CameraNodes camNodes = camNodesObject.GetComponent<CameraNodes>();
        smallestDistance = Vector3.Distance(camNodes.nodes[0].position, characterTransform.position);
        closestNode = camNodes.nodes[0];

        foreach (Transform node in camNodes.nodes)
        {
            float newDistance = Vector3.Distance(node.position, characterTransform.position);
            if (newDistance < smallestDistance)
            {
                smallestDistance = newDistance;
                closestNode = node;
            }
        }

        // Rotate around object
        if (isRotating)
        {
            CamRotateToNode();
        }
        else
        {
            CamMoveToNode();
        }

        if (Physics.gravity.y < 0)
        {
            camPlayerHeightOffset = 2;
        }
        else if (Physics.gravity.y > 0)
        {
            camPlayerHeightOffset = -2;
        }
        else
        {
            camPlayerHeightOffset = 0;
        }

        if (looksAtPlayer)
            transform.LookAt(characterTransform.position);
        else
            transform.LookAt(cameraCenterOfMass.transform.position);
    }

    private void CamMoveToNode()
    {
        transform.position = Vector3.SmoothDamp(transform.position, closestNode.position, ref zero, camSpeed);
    }

    private void CamRotateToNode()
    {
        Quaternion targetRotation;
        float followScale;
        cameraCenterOfMass.transform.position = centerObject.transform.position;
        if (nodeFollow)
        {
            Vector3 horizontalVector = cameraCenterOfMass.transform.position + GetHorizontalVector(cameraCenterOfMass.transform.position, closestNode.position);
            targetRotation = Quaternion.LookRotation(horizontalVector);
            followScale = horizontalVector.magnitude;
        }
        else
        {
            Vector3 horizontalVector = cameraCenterOfMass.transform.position + GetHorizontalVector(cameraCenterOfMass.transform.position, characterTransform.position);
            targetRotation = Quaternion.LookRotation(horizontalVector);
            followScale = horizontalVector.magnitude;
        }

        if (!looksAtPlayer)
            cameraCenterOfMass.transform.rotation = Quaternion.Slerp(cameraCenterOfMass.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        camFollowObject.transform.position = cameraCenterOfMass.transform.forward.normalized * followScale +
            cameraCenterOfMass.transform.forward * rotationOffset +
            Vector3.up * characterTransform.position.y +
            Vector3.up * camPlayerHeightOffset;

        transform.position = Vector3.SmoothDamp(transform.position, camFollowObject.transform.position, ref zero, camSpeed);
    }

    private Vector3 GetHorizontalVector(Vector3 a, Vector3 b)
    {
        Vector3 resultvector = b - a;
        return new Vector3(resultvector.x, 0, resultvector.z);
    }
}
