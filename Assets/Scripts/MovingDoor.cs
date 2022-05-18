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
        usageTime = 5f;
    }

    public override IEnumerator Use(float _useDuration)
    {
        MovingDoor _exitDoor = exitDoor as MovingDoor;
        _exitDoor.isBlocked = true;
        ChangeMaterial(useMaterial);
        _exitDoor.ChangeMaterial(useMaterial);
        gameManager.DestroyCharacter();
        yield return new WaitForSeconds(_useDuration);

        yield return new WaitUntil(() => !movableObject.isMoving);

        if (movableObject.moveState == 0)
            gameManager.SpawnCharacter(exitPathID, exitNode, exitDirection);
        else if (movableObject.moveState == 1)
            gameManager.SpawnCharacter(exit2PathID, exit2Node, exit2Direction);

        yield return new WaitForSeconds(_useDuration / 4 * 3);

        ChangeMaterial(normalMaterial);
        _exitDoor.ChangeMaterial(normalMaterial);
        _exitDoor.isBlocked = false;
    }
}
