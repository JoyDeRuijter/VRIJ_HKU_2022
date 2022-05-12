using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    #region Variables

    [Header ("Door Material")]
    [SerializeField] Material material;

    private MeshRenderer[] meshRenderers;

    #endregion

    private void Awake()
    {
        //Set the material of the whole object to the material provided in the inspector
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer mr in meshRenderers)
            mr.material = material;
    }
}
