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

    [Space(10)]
    [Header("MovableObject Materials")]
    [SerializeField] Material unActivatedMaterial;
    [SerializeField] Material activatedMaterial;

    private MeshRenderer[] meshRenderers;
    private bool isActivated;
    private bool isRotatedRight;
    private bool isRotatedLeft;
    private Vector3 basePosition;
    private Quaternion baseRotation;
    [HideInInspector] public int rotationState = 0; // -1 = moving, 0 = base, 1 = right, 2 = left
    [HideInInspector] public int moveState = 0; // -1 = moving, 0 = base, 1 = moved forward

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
            MoveObject(basePosition, 4f);
            ChangeMaterial(unActivatedMaterial);
        }
        else
            RotateObject(3f);

        isActivated = false;
    }

    private void MoveObject(Vector3 _targetPosition, float _moveTime)
    {
        moveState = -1;
        transform.DOMoveX(_targetPosition.x, _moveTime);
        StartCoroutine(InMovement(_targetPosition, _moveTime));
    }

    private IEnumerator InMovement(Vector3 _targetPosition, float _moveTime)
    { 
        yield return new WaitForSeconds(_moveTime - 1f);
        if (_targetPosition == basePosition) // If it moves back to the base position
        {
            if (Vector3.Distance(_targetPosition, basePosition) <= 1f)
                moveState = 0;
        }
        else if (_targetPosition == movedPosition) // If it moves out to the moved position
        {  
            if (Vector3.Distance(_targetPosition, movedPosition) <= 1f)
                moveState = 1;      
        }
    }

    private void RotateObject(float _moveTime)
    {
        if (!isRotatedLeft && !isRotatedRight)
        {
            transform.DORotateQuaternion(rightRotation, _moveTime);
            isRotatedRight = true;
            rotationState = 1;
        }
        else if (isRotatedLeft)
        {
            transform.DORotateQuaternion(baseRotation, _moveTime);
            isRotatedLeft = false;
            ChangeMaterial(unActivatedMaterial);
            rotationState = 0;
        }
        else if (isRotatedRight)
        {
            transform.DORotateQuaternion(leftRotation, _moveTime * 2);
            isRotatedRight = false;
            isRotatedLeft = true;
            rotationState = 2;
        }
    }
}
