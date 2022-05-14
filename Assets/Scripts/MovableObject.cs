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
    [SerializeField] private Quaternion leftRotation;
    [SerializeField] private Quaternion rightRotation;
    [Space(10)]
    [SerializeField] private Vector3 movedPosition;

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
            MoveObject(movedPosition, 4f);
        else
            RotateObject(2f);

        isActivated = true;
    }

    private void DeactivateObject()
    {
        if (!isRotation)
        { 
            MoveObject(basePosition, 6f);
            ChangeMaterial(unActivatedMaterial);
        }
        else
            RotateObject(3f);

        isActivated = false;
    }

    private void MoveObject(Vector3 _targetPosition, float _moveTime)
    {
        transform.DOMoveX(_targetPosition.x, _moveTime);
    }

    private void RotateObject(float _moveTime)
    {
        if (!isRotatedLeft && !isRotatedRight)
        {
            transform.DORotateQuaternion(rightRotation, _moveTime);
            isRotatedRight = true;
        }
        else if (isRotatedLeft)
        {
            transform.DORotateQuaternion(baseRotation, _moveTime);
            isRotatedLeft = false;
            ChangeMaterial(unActivatedMaterial);
        }
        else if (isRotatedRight)
        {
            transform.DORotateQuaternion(leftRotation, _moveTime * 2);
            isRotatedRight = false;
            isRotatedLeft = true;
        }
    }
}
