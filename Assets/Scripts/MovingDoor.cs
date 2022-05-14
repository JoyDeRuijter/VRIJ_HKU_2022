using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingDoor : Door
{
    #region Variables

    [Space(10)]
    [Header("Moved Exit")]
    public int exit2PathID;
    public int exit2Node;
    public Direction exit2Direction;

    private MovableObject movableObject;

    #endregion

    public override void Awake()
    {
        base.Awake();
        movableObject = this.gameObject.GetComponentInParent<MovableObject>();
    }

    private void Update()
    {
        //if(movableObject.moveState != 0)
            //Debug.Log(movableObject.moveState);
    }

    public override IEnumerator Use(float _useDuration)
    {
        MovingDoor _exitDoor = exitDoor as MovingDoor;
        _exitDoor.isBlocked = true;
        ChangeMaterial(useMaterial);
        _exitDoor.ChangeMaterial(useMaterial);
        yield return new WaitForSeconds(0.3f);

        // WERKT NIET OMDAT DIT AL WORDT UITGEVOERD VOORDAT DE PREFAB UBERHAUPT WORDT VERWIJDERD
        // MOET ERGENS TIJDENS HET VERVOEREN KUNNEN CHECKEN
        if (movableObject.moveState == 0)
            StartCoroutine(gameManager.SwitchCharacterPath(exitPathID, exitNode, enterNode, exitDirection, _useDuration));
        else if (movableObject.moveState == 1)
            StartCoroutine(gameManager.SwitchCharacterPath(exit2PathID, exit2Node, enterNode, exit2Direction, _useDuration));
        //else if (moveState == -1)
        //    gameManager.DropCharacter(transform.position);

        yield return new WaitForSeconds(_useDuration + 1f);

        ChangeMaterial(normalMaterial);
        _exitDoor.ChangeMaterial(normalMaterial);
        yield return new WaitForSeconds(_useDuration + 0.5f);

        _exitDoor.isBlocked = false;
    }
}
