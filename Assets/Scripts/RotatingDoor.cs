using System.Collections;
using UnityEngine;

public class RotatingDoor : Door
{
    #region Variables

    [Space(10)]
    [Header("Rotated Exits")]
    public int exit2PathID;
    public int exit2Node;
    public Vector3 exit3Position;
    public Direction exit2Direction;

    private MovableObject movableObject;

    #endregion

    public override void Awake()
    {
        base.Awake();
        movableObject = this.gameObject.GetComponentInParent<MovableObject>();
        usageTime = 5f;
        exit3Position = transform.position;
    }

    public override IEnumerator Use(float _useDuration)
    {
        RotatingDoor _exitDoor = exitDoor as RotatingDoor;

        if (movableObject.rotationState == 0)
            yield break;

        _exitDoor.isBlocked = true;
        ChangeMaterial(useMaterial);
        _exitDoor.ChangeMaterial(useMaterial);
        gameManager.DestroyCharacter();
        yield return new WaitForSeconds(_useDuration);

        if (!movableObject.isRotation)
            yield return new WaitUntil(() => !movableObject.isMoving);
        else
            yield return new WaitUntil(() => !movableObject.isRotating);

        if (movableObject.rotationState == 0) // Base
            gameManager.DropCharacter(exit3Position);
        else if (movableObject.rotationState == 1) // Right
            gameManager.SpawnCharacter(exitPathID, exitNode, exitDirection);
        else if (movableObject.rotationState == 2) // Left
            gameManager.SpawnCharacter(exit2PathID, exit2Node, exit2Direction);


        yield return new WaitForSeconds(_useDuration / 4 * 3);

        ChangeMaterial(normalMaterial);
        _exitDoor.ChangeMaterial(normalMaterial);
        _exitDoor.isBlocked = false;
    }
}
