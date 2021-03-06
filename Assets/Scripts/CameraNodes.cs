using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CameraNodes : MonoBehaviour
{
    #region Variables

    [Header ("Path Properties")]
    public Color lineColor;
    
    [HideInInspector] public List<Transform> nodes = new List<Transform>();

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

            Gizmos.DrawSphere(gizmoPosCurrent, 0.2f);

            if (previousNode != nodes[nodes.Count - 1].position)
                Gizmos.DrawLine(gizmoPosPrevious, gizmoPosCurrent);
        }

    }
}
