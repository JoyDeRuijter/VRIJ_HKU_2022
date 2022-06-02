using System.Collections;
using UnityEngine;

public class RotatingDoor : Door
{
    #region Variables

    [Space(10)]
    [Header("Rotated Exits")]
    public int exit2PathID;
    public int exit2Node;
    public Door exit2Door;
    public Vector3 exit3Position;
    public WalkDirection exit2Direction;

    private MovableObject movableObject;
    private RotatingDoor currentExitDoor;
    private Door currentExitDoor2;


    #endregion

    public override void Awake()
    {
        base.Awake();
        movableObject = gameObject.GetComponentInParent<MovableObject>();
        exit3Position = transform.position;
    }

    public override IEnumerator Use(float _useDuration)
    {
        if (movableObject.rotationState == 0)
        {
            currentExitDoor = exitDoor as RotatingDoor;
            currentExitDoor.isBlocked = true;
        }
        else if (movableObject.rotationState == 1)
        { 
            currentExitDoor2 = exit2Door;
        }

        if (movableObject.rotationState == 2)
            yield break;

        //ChangeMaterial(useMaterial);
        //_exitDoor.ChangeMaterial(useMaterial);
        gameManager.DestroyCharacter();
        yield return new WaitForSeconds(_useDuration);

        if (!movableObject.isRotation)
            yield return new WaitUntil(() => !movableObject.isMoving);
        else
            yield return new WaitUntil(() => !movableObject.isRotating);
        Debug.Log(movableObject.isRotating);
        if (movableObject.rotationState == 2) // Above an abyss
            gameManager.DropCharacter(exit3Position);
        else if (movableObject.rotationState == 1) // Left
            gameManager.SpawnCharacter(exitPathID, exitNode, exitDirection);
        else if (movableObject.rotationState == 0) // Base
            gameManager.SpawnCharacter(exit2PathID, exit2Node, exit2Direction);


        yield return new WaitForSeconds(_useDuration / 4 * 3);
        //ChangeMaterial(normalMaterial);
        //_exitDoor.ChangeMaterial(normalMaterial);
        if (exitDoor != null) exitDoor.isBlocked = false;
        if (exit2Door != null) exit2Door.isBlocked = false;
    }
}
