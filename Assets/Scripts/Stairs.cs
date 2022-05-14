using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    #region Variables

    [Header("Stairs Materials")]
    [SerializeField] Material normalMaterial;
    [SerializeField] Material touchedMaterial;

    private MeshRenderer[] meshRenderers;

    #endregion

    private void Awake()
    {
        //Set the material of the whole object to the material provided in the inspector
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        ChangeMaterial(normalMaterial);
    }

    public void ChangeMaterial(Material _material)
    {
        foreach (MeshRenderer mr in meshRenderers)
            mr.material = _material;
    }
}
