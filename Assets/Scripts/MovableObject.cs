using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    #region Variables

    [Header("MovableObject Materials")]
    [SerializeField] Material unActivatedMaterial;
    [SerializeField] Material activatedMaterial;

    private MeshRenderer[] meshRenderers;
    private bool isActivated;

    #endregion

    private void Awake()
    {
        //Set the material of the whole object to the material provided in the inspector
        meshRenderers = this.transform.Find("Structure").GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in meshRenderers)
            mr.material = unActivatedMaterial;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && !isActivated)
            ActivateObject();
        else if (Input.GetKeyDown(KeyCode.W) && isActivated)
            DeactivateObject();
    }

    private void ActivateObject()
    {
        foreach (MeshRenderer mr in meshRenderers)
            mr.material = activatedMaterial;

        // Actually make the object move out

        isActivated = true;
    }

    private void DeactivateObject()
    {
        foreach (MeshRenderer mr in meshRenderers)
            mr.material = unActivatedMaterial;

        // Actually make the object move back to origin position

        isActivated = false;
    }
}
