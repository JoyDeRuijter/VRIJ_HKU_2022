using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    #region Variables

    [Header("Character Material")]
    [SerializeField] Material material;

    [HideInInspector] public int xPos, yPos, zPos;
    [HideInInspector] public Vector3Int position;
    [HideInInspector] public bool isMoving;

    private MeshRenderer meshRenderer;

    #endregion

    private void Awake()
    {
        xPos = (int)transform.position.x;
        yPos = (int)(transform.position.y - 5.1f);
        zPos = (int)transform.position.z;
        position = new Vector3Int(xPos, yPos, zPos);

        //Set the material of the whole object to the material provided in the inspector
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = material;
    }

    private void Update()
    {
        xPos = (int)transform.position.x;
        yPos = (int)(transform.position.y - 5.1f);
        zPos = (int)transform.position.z;
        position = new Vector3Int(xPos, yPos, zPos);
    }

    private void FixedUpdate()
    {
        CheckForMovement();
    }

    private IEnumerator CheckForMovement()
    {
        Vector3 startPosition = transform.position;
        yield return new WaitForSeconds(0.3f);

        if (transform.position != startPosition)
            isMoving = true;
        else
            isMoving = false;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Cube>() != null)
            collision.gameObject.GetComponent<Cube>().SetTouchedMaterial();
        // Do the same for stairs later: collision.gameObject.transform.parent.GetComponent<Stairs>() != null
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<Cube>() != null)
            collision.gameObject.GetComponent<Cube>().SetNormalMaterial();
        // Do the same for stairs later: collision.gameObject.transform.parent.GetComponent<Stairs>() != null
    }
}
