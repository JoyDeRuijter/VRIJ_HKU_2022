using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    #region Variables

    [Header("Is This Cube Walkable?")]
    public bool isWalkable;

    [Space(10)]
    [Header("Cube Materials")]
    public Material normalMaterial;
    public Material touchedMaterial;

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
        ChangeMaterial(normalMaterial);  
    }

    public void ChangeMaterial(Material _material)
    {
        meshRenderer.material = _material;
    }

    #region Collision With Character

    private void OnCollisionEnter(Collision _collision)
    {
        if (_collision.gameObject.GetComponent<Character>() != null && isWalkable)
            ChangeMaterial(touchedMaterial);
    }

    private void OnCollisionExit(Collision _collision)
    {
        if (_collision.gameObject.GetComponent<Character>() != null && isWalkable)
            ChangeMaterial(normalMaterial);
    }

    #endregion
}
