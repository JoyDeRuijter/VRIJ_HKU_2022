using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    #region Variables

    [Header("Stairs Material")]
    [SerializeField] Material material;

    private MeshRenderer[] meshRenderers;

    #endregion

    private void Awake()
    {
        //Set the material of the whole object to the material provided in the inspector
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in meshRenderers)
            mr.material = material;
    }
}
