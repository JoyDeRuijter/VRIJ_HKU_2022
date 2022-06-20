using UnityEngine;

[SelectionBase]
public class PlayerCamera : MonoBehaviour
{
    #region Variables
    [Header("Camera options")]
    [Range(0f, 1f), SerializeField] float camSpeed;
    [SerializeField] GameObject camNodesObject;
    [SerializeField] int currentFollowNode = 0;
    public static int currentAutoNode = 0;

    public Transform character = null;
    public bool playerIsActive = false;

    Vector3 zero = Vector3.zero;
    Vector3 lastPlayerPos;

    CameraNodes camNodes;
    private GameManager gameManager;
    private static int lastNode;
    #endregion

    private void Start()
    {
        camNodes = camNodesObject.GetComponent<CameraNodes>();
        gameManager = GameManager.instance;
    }

    private void Update()
    {
        if (gameManager.controlCam)
        {
            transform.position = Vector3.SmoothDamp(transform.position, camNodes.nodes[currentFollowNode].position, ref zero, camSpeed);
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, camNodes.nodes[currentAutoNode].position, ref zero, camSpeed);
            currentFollowNode = currentAutoNode;
        }

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
        if (currentFollowNode < camNodes.nodes.Count - 1)
        {
            currentFollowNode++;
        }
    }

    public void MoveToPreviousNode()
    {
        if (currentFollowNode > 0)
        {
            currentFollowNode--;
        }
    }

    public static void SetAutoCamNode(int current, int last)
    {
        lastNode = last;
        currentAutoNode = current;
    }

    public static void BackToLastNode()
    {
        currentAutoNode = lastNode;
    }
}
