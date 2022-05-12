using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    #region Variables

    [Header("Is This Cube Walkable?")]
    public bool isWalkable;

    [Header("Cube Materials")]
    [Space(10)]
    [SerializeField] Material normalMaterial;
    [SerializeField] Material touchedMaterial;

    [HideInInspector] public int xPos, yPos, zPos;
    [HideInInspector] public Vector3Int position;

    private MeshRenderer meshRenderer;

    #endregion

    private void Awake()
    {
        xPos = (int)transform.localPosition.x;
        yPos = (int)transform.localPosition.y;
        zPos = (int)transform.localPosition.z;
        position = new Vector3Int(xPos, yPos, zPos);

        //Set the material of the whole object to the material provided in the inspector
        meshRenderer = GetComponent<MeshRenderer>();
        SetNormalMaterial();
        
    }

    public void SetNormalMaterial()
    {
        meshRenderer.material = normalMaterial;
    }

    public void SetTouchedMaterial()
    { 
        meshRenderer.material = touchedMaterial;
    }
}
