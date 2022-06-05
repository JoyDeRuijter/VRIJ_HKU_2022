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

    [HideInInspector] public Vector3 gravityDirection;

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
                child.isTrigger = true;
                switch (faceDirection)
                {
                    case FaceDirection.xForward:
                        gravityDirection = Vector3.right;
                        child.center = new Vector3(-.5f, 0, 0);
                        child.size = new Vector3(.5f, 1, 1);
                        break;

                    case FaceDirection.xBackward:
                        gravityDirection = Vector3.left;
                        child.center = new Vector3(.5f, 0, 0);
                        child.size = new Vector3(.5f, 1, 1);
                        break;

                    case FaceDirection.yUp:
                        gravityDirection = Vector3.up;
                        child.center = new Vector3(0, -.5f, 0);
                        child.size = new Vector3(1, .5f, 1);
                        break;

                    case FaceDirection.yDown:
                        gravityDirection = Vector3.down;
                        child.center = new Vector3(0, .5f, 0);
                        child.size = new Vector3(1, .5f, 1);
                        break;

                    case FaceDirection.zForward:
                        gravityDirection = Vector3.forward;
                        child.center = new Vector3(0, 0f, -.5f);
                        child.size = new Vector3(1, 1, .5f);
                        break;

                    case FaceDirection.zBackward:
                        gravityDirection = Vector3.back;
                        child.center = new Vector3(0, 0, .5f);
                        child.size = new Vector3(1, 1, .5f);
                        break;

                    case FaceDirection.custom:
                        gravityDirection = child.center - transform.position;
                        break;
                }
                Debug.DrawLine(child.transform.position, child.transform.position + gravityDirection);
            }
        }
    }

    public Vector3 GetNodeFloorPointPosition(int _node)
    {
        return nodes[_node].position + GetNodeCollider(_node).center;
    }

    public BoxCollider GetNodeCollider(int _node)
    {
        return nodes[_node].GetComponent<BoxCollider>();
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

            Vector3 gizmoPosCurrent = new Vector3(currentNode.x, currentNode.y, currentNode.z);
            Vector3 gizmoPosPrevious = new Vector3(previousNode.x, previousNode.y, previousNode.z);

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

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            BoxCollider childBox = child.GetComponent<BoxCollider>();
            if (child != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(child.transform.position + childBox.center, 0.1f);
            }
        }
    }
}

public enum FaceDirection { xForward, xBackward, yUp, yDown, zForward, zBackward, custom }