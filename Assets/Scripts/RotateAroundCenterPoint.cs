using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class RotateAroundCenterPoint : MonoBehaviour
{
    [SerializeField] GameObject centerObject;
    [SerializeField] float rotationSpeed;

    void Update()
    {
        centerObject.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
