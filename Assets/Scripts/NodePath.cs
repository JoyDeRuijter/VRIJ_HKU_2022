using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class NodePath : MonoBehaviour
{
    #region Variables
    
    [Header("Path Properties")]
    public Color lineColor;
    public int ID;
    public WalkDirection preferedDirection;
    public bool isLoop = false;

    public FaceDirection faceDirection = FaceDirection.yUp;

    [HideInInspector] public List<Transform> nodes = new List<Transform>();

    private void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            BoxCollider child = transform.GetChild(i).GetComponent<BoxCollider>();
            if (child == null)
            {
                transform.GetChild(i).gameObject.AddComponent<BoxCollider>();
            }
            else
            {
                switch (faceDirection)
                {
                    case FaceDirection.xForward:
                        child.center = new Vector3(0, -.5f, 0);
                        child.size = new Vector3(.6f, 1, 1);
                        break;

                    case FaceDirection.xBackward:
                        child.center = new Vector3(0, -.5f, 0);
                        child.size = new Vector3(.6f, 1, 1);
                        break;

                    case FaceDirection.yUp:
                        child.center = new Vector3(0, -.5f, 0);
                        child.size = new Vector3(1, .5f, 1);
                        break;

                    case FaceDirection.yDown:
                        child.center = new Vector3(0, -.5f, 0);
                        child.size = new Vector3(1, .5f, 1);
                        break;

                    case FaceDirection.zForward:
                        child.center = new Vector3(0, -.5f, 0f);
                        child.size = new Vector3(1, 1, .6f);
                        break;

                    case FaceDirection.zBackward:
                        child.center = new Vector3(0, -.5f, 0);
                        child.size = new Vector3(1, 1, .6f);
                        break;
                }
            }
        }
    }

    #endregion
    private void OnDrawGizmos()
    {
        Gizmos.color = lineColor;

        Transform[] pathTransforms = GetComponentsInChildren<Transform>();
        nodes.Clear();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != transform)
                nodes.Add(pathTransforms[i]);
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 currentNode = nodes[i].position;
            Vector3 previousNode = Vector3.zero;

            if (i > 0)
                previousNode = nodes[i - 1].position;
            else if (i == 0 && nodes.Count > 1)
                previousNode = nodes[nodes.Count - 1].position;

            Vector3 gizmoPosCurrent = new Vector3(currentNode.x, currentNode.y - 0.5f, currentNode.z);
            Vector3 gizmoPosPrevious = new Vector3(previousNode.x, previousNode.y - 0.5f, previousNode.z);

            if (isLoop)
            {
                Gizmos.DrawLine(gizmoPosPrevious, gizmoPosCurrent);
            }
            else
            {
                if (previousNode != nodes[nodes.Count - 1].position)
                    Gizmos.DrawLine(gizmoPosPrevious, gizmoPosCurrent);
            }


            Gizmos.DrawSphere(gizmoPosCurrent, 0.2f);
        }
    }
}

public enum FaceDirection { xForward, xBackward, yUp, yDown, zForward, zBackward }