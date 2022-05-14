using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    #region Variables

    [Header("Door Materials")]
    [SerializeField] Material normalMaterial;
    [SerializeField] Material useMaterial;

    [Space(10)]
    [Header("Door Paths & Nodes")]
    public int enterPathID;
    public int exitPathID;
    public int enterNode;
    public int exitNode;
    public Direction exitDirection;
    [SerializeField] private Door exitDoor;

    private MeshRenderer[] meshRenderers;
    private GameManager gameManager;
    private Character character;
    private bool isBlocked;
    private bool characterIsAtDoor;

    #endregion

    private void Awake()
    {
        //Set the material of the whole object to the material provided in the inspector
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        ChangeMaterial(normalMaterial);
    }

    private void Start()
    {
        gameManager = GameManager.instance;
        character = gameManager.character;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Character>() != null && !isBlocked)
            StartCoroutine(Use(3f));
    }

    public void ChangeMaterial(Material _material)
    {
        foreach (MeshRenderer mr in meshRenderers)
            mr.material = _material;
    }

    private IEnumerator Use(float _useDuration)
    {
        exitDoor.isBlocked = true;
        ChangeMaterial(useMaterial);
        exitDoor.ChangeMaterial(useMaterial);
        yield return new WaitForSeconds(0.3f);

        StartCoroutine(gameManager.SwitchCharacterPath(exitPathID, exitNode, enterNode, exitDirection, _useDuration));
        yield return new WaitForSeconds(_useDuration + 1f);

        ChangeMaterial(normalMaterial);
        exitDoor.ChangeMaterial(normalMaterial);
        yield return new WaitForSeconds(_useDuration + 0.5f);

        exitDoor.isBlocked = false;
    }
}
