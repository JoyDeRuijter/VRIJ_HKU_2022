using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    #region Variables

    [Header("Cube Material")]
    [SerializeField] Material material;

    private MeshRenderer meshRenderer;

    #endregion

    private void Awake()
    {
        //Set the material of the whole object to the material provided in the inspector
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = material;
    }
}
