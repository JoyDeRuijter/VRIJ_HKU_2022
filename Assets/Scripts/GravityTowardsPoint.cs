using UnityEngine;

[ExecuteAlways]
public class GravityTowardsPoint : MonoBehaviour
{
    [SerializeField] bool useFeature = false;
    public GravityPoints point;
    public static bool boundToPoint = false;
    public static float gravityStrenght = -9.81f;

    private void Update()
    {
        Quaternion targetRotation = Quaternion.FromToRotation(-transform.up / 2, Physics.gravity);
        if (!boundToPoint || !useFeature)
            return;

        Vector3 pointDirection = (point.transform.position - transform.position).normalized;

        inOrOutsideRotation inOrOut = point.inOrOut;
        switch (inOrOut)
        {
            default: 
                targetRotation = Quaternion.identity; 
                break;

            case inOrOutsideRotation.inside:
                targetRotation = Quaternion.FromToRotation(transform.up / 2, pointDirection) * transform.rotation;
                Physics.gravity = (point.transform.position - transform.position).normalized * gravityStrenght;
                break;

            case inOrOutsideRotation.outside:
                targetRotation = Quaternion.FromToRotation(-transform.up / 2, pointDirection) * transform.rotation;
                Physics.gravity = (transform.position - point.transform.position).normalized * gravityStrenght;
                break;
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 50f);
    }


    private void OnTriggerEnter(Collider other)
    {
        GravityPoints gravityPoint = other.GetComponent<GravityPoints>();
        if (gravityPoint != null)
        {
            point = gravityPoint;
            boundToPoint = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GravityPoints gravityPoint = other.GetComponent<GravityPoints>();
        if (gravityPoint != null)
        {
            point = null;
            boundToPoint = false;
        }
    }


    private void OnDrawGizmos()
    {
        if (!boundToPoint)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, point.transform.position);
    }
}

public enum inOrOutsideRotation { inside, outside };
