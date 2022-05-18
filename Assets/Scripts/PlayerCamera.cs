using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    #region Variables

    [Header("Camera information")]
    [Range(0f, 0.5f), SerializeField] float camSpeed;
    [SerializeField] GameObject camNodesObject;

    [Header("Camera looks at player or centerObject")]
    [SerializeField] bool looksAtPlayer;
    [SerializeField] Transform centerObject;

    public static Transform characterTransform = null;
    public static bool playerIsActive = false;

    private float smallestDistance;
    private Transform closestNode;
    Vector3 zero = Vector3.zero;

    #endregion

    private void Update()
    {
        if (!playerIsActive) return;

        camNodesObject.transform.position = new Vector3(camNodesObject.transform.position.x, characterTransform.position.y + 2, camNodesObject.transform.position.z);

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
