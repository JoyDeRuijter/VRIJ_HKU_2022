using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    #region Variables

    [Header("Camera information")]
    [Range(0f, 0.5f), SerializeField] float camSpeed;
    [SerializeField] GameObject camNodesObject;
    [Range(1f, 10f), SerializeField] float minCamOffset;
    [SerializeField] float minCamHeight;
    [SerializeField] float maxCamHeight;
    [SerializeField] int startingNode;

    [Header("Camera looks at player or a centerObject")]
    [SerializeField] bool looksAtPlayer;
    [SerializeField] Transform centerObject;

    public static Transform characterTransform = null;
    public static bool playerIsActive = false;

    private float smallestDistance;
    private Transform closestNode;
    Vector3 zero = Vector3.zero;

    #endregion

    private void Start()
    {
        camNodesObject.transform.position = new Vector3(camNodesObject.transform.position.x, characterTransform.position.y, camNodesObject.transform.position.z);
        transform.position = camNodesObject.GetComponent<CameraNodes>().nodes[startingNode].position;
    }

    private void Update()
    {
        if (!playerIsActive) return;

        float camPlayerOffset;
        if (Physics.gravity.y < 0)
        {
            camPlayerOffset = 2;
        }
        else if (Physics.gravity.y > 0)
        {
            camPlayerOffset = -2;
        }
        else
        {
            camPlayerOffset = 0;
        }

        if (camNodesObject.transform.position.y > minCamHeight && camNodesObject.transform.position.y < maxCamHeight)
            camNodesObject.transform.position = new Vector3(camNodesObject.transform.position.x, characterTransform.position.y + camPlayerOffset, camNodesObject.transform.position.z);

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

        transform.position = Vector3.SmoothDamp(transform.position, closestNode.position, ref zero, camSpeed);
        if (looksAtPlayer) transform.LookAt(characterTransform.position);
        else
        {
            centerObject.position = new Vector3(centerObject.position.x, characterTransform.position.y, centerObject.position.z);
            transform.LookAt(centerObject);
        }
    }
}
