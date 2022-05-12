using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovableObject : MonoBehaviour
{
    #region Variables

    [Header("Interact Key")]
    [SerializeField] KeyCode key;

    [Header("Kind Of Manipulation")]
    [Space(10)]
    [SerializeField] private bool isRotation;
    [Space(10)]
    [SerializeField] private Quaternion leftRotation;
    [SerializeField] private Quaternion rightRotation;
    [Space(10)]
    [SerializeField] private Vector3 movedPosition;

    [Header("MovableObject Materials")]
    [Space(10)]
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
        foreach (MeshRenderer mr in meshRenderers)
            mr.material = unActivatedMaterial;

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

    private void ActivateObject()
    {
        foreach (MeshRenderer mr in meshRenderers)
            mr.material = activatedMaterial;

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
            foreach (MeshRenderer mr in meshRenderers)
                mr.material = unActivatedMaterial;        
        }

        if (!isRotation)
            MoveObject(basePosition, 6f);
        else
            RotateObject(3f);

        isActivated = false;
    }

    private void MoveObject(Vector3 targetPosition, float moveTime)
    {
        transform.DOMoveX(targetPosition.x, moveTime);
    }

    private void RotateObject(float moveTime)
    {
        if (!isRotatedLeft && !isRotatedRight)
        {
            transform.DORotateQuaternion(rightRotation, moveTime);
            isRotatedRight = true;
        }
        else if (isRotatedLeft)
        {
            transform.DORotateQuaternion(baseRotation, moveTime);
            isRotatedLeft = false;
            foreach (MeshRenderer mr in meshRenderers)
                mr.material = unActivatedMaterial;
        }
        else if (isRotatedRight)
        {
            transform.DORotateQuaternion(leftRotation, moveTime*2);
            isRotatedRight = false;
            isRotatedLeft = true;
        }
    }
}
