using UnityEngine;

[SelectionBase]
public class PlayerCamera : MonoBehaviour
{
    #region Variables
    [Header("Camera options")]
    [Range(0f, 1f), SerializeField] float camSpeed;
    [SerializeField] GameObject camNodesObject;
    [SerializeField] int currentNode = 0;

    public Transform character = null;
    public bool playerIsActive = false;

    Vector3 zero = Vector3.zero;
    Vector3 lastPlayerPos;

    CameraNodes camNodes;
    #endregion

    private void Start()
    {
        camNodes = camNodesObject.GetComponent<CameraNodes>();
    }

    private void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, camNodes.nodes[currentNode].position, ref zero, camSpeed);

        if (playerIsActive)
        {
            transform.LookAt(character.transform);
            lastPlayerPos = character.position;
        }
        else
        {
            transform.LookAt(lastPlayerPos);
        }
    }

    public void MoveToNextNode()
    {
        if (currentNode < camNodes.nodes.Count - 1)
        {
            currentNode++;
        }
    }

    public void MoveToPreviousNode()
    {
        if (currentNode > 0)
        {
            currentNode--;
        }
    }
}
