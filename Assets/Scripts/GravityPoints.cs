using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways, RequireComponent(typeof(BoxCollider))]
public class GravityPoints : MonoBehaviour
{
    [SerializeField, Range(0.1f, 10f)] float boxScale;
    [SerializeField] float charterOffset = 0.5f;
    
    private BoxCollider box;
    public inOrOutsideRotation inOrOut = inOrOutsideRotation.inside;

    private void Awake()
    {
        box = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (Application.isPlaying) 
        {

        }
        else
        {
            box = GetComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = new Vector3(1, boxScale, boxScale);
            box.center = new Vector3(0, -boxScale / 2 - charterOffset, boxScale / 2 + charterOffset);
        }

    }

    private void OnDrawGizmos()
    {
        switch (inOrOut)
        {
            case inOrOutsideRotation.inside:
                Gizmos.color = Color.red;
                break;

            case inOrOutsideRotation.outside:
                Gizmos.color = Color.blue;
                break;
        }
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
}
