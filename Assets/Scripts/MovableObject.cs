using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovableObject : MonoBehaviour
{
    #region Variables

    [Header("Interact Key")]
    [SerializeField] KeyCode key;

    [Space(10)]
    [Header("Kind Of Manipulation")]
    [SerializeField] private bool isRotation;
    
    [Space(10)]
    [Header("If Rotation")]
    [SerializeField] private Quaternion leftRotation;
    [SerializeField] private Quaternion rightRotation;
    [SerializeField] private int numberOfRotationStates;

    [Space(10)]
    [Header("If Movement")]
    [SerializeField] private Vector3 movedPosition;
    [SerializeField] private int numberOfMoveStates;
    [SerializeField] private int affectedPathID;
    [SerializeField] private int[] affectedNodeIDs; // Nodes that are blocked from the path when the object is moved out

    [Space(10)]
    [Header("MovableObject Materials")]
    [SerializeField] Material unActivatedMaterial;
    [SerializeField] Material activatedMaterial;

    private MeshRenderer[] meshRenderers;
    private bool isActivated;
    [HideInInspector] public bool isMoving;
    private Vector3 basePosition;
    private Quaternion baseRotation;
    [HideInInspector] public int rotationState = 0; // 0 = base, 1 = right, 2 = left
    [HideInInspector] public int moveState = 0; // 0 = base, 1 = moved forward

    private GameManager gameManager;

    #endregion

    private void Awake()
    {
        //Set the material of the whole object to the material provided in the inspector
        meshRenderers = this.transform.Find("Structure").GetComponentsInChildren<MeshRenderer>();
        ChangeMaterial(unActivatedMaterial);

        if(isRotation)
            baseRotation = gameObject.transform.rotation;
        else
            basePosition = gameObject.transform.position;
    }

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(key) && !isActivated)
            ActivateObject();
        else if (Input.GetKeyDown(key) && isActivated)
            DeactivateObject();
    }

    public void ChangeMaterial(Material _material)
    {
        foreach (MeshRenderer mr in meshRenderers)
            mr.material = _material;
    }

    private void ActivateObject()
    {
        ChangeMaterial(activatedMaterial);

        if (!isRotation)
        {
            moveState = 1;
            MoveObject(movedPosition, 4f);
        }
        else
            RotateObject(2f);

        isActivated = true;
    }

    private void DeactivateObject()
    {
        if (!isRotation)
        {
            moveState = 0;
            MoveObject(basePosition, 4f);
            ChangeMaterial(unActivatedMaterial);
        }
        else
            RotateObject(3f);

        isActivated = false;
    }

    private void MoveObject(Vector3 _targetPosition, float _moveTime)
    {
        StartCoroutine(WhileMoving(_moveTime));
        transform.DOMoveX(_targetPosition.x, _moveTime);
    }

    private IEnumerator WhileMoving(float _moveTime)
    {
        isMoving = true;
        //if(moveState == 1 && affectedNodeIDs.Length != 0)
            //BlockEffectedNodes();

        yield return new WaitForSeconds(_moveTime);

        isMoving = false;
        //if(moveState == 0 && affectedNodeIDs.Length != 0)
           // UnblockEffectedNodes();
    }

    // THESE FREEZE THE WHOLE GAME, FIND OUT WHY
    private void BlockEffectedNodes()
    {
        gameManager.RemoveNodesFromPath(affectedPathID, affectedNodeIDs);
    }

    private void UnblockEffectedNodes()
    { 
        gameManager.AddNodesToPath(affectedPathID, affectedNodeIDs);
    }

    private void RotateObject(float _moveTime)
    {
        if (rotationState == 0)
        {
            transform.DORotateQuaternion(rightRotation, _moveTime);
            rotationState = 1;
        }
        else if (rotationState == 1)
        {
            transform.DORotateQuaternion(leftRotation, _moveTime * 2);
            rotationState = 2;
        }
        else if (rotationState == 2)
        {
            transform.DORotateQuaternion(baseRotation, _moveTime);
            ChangeMaterial(unActivatedMaterial);
            rotationState = 0;
        }
    }
}
