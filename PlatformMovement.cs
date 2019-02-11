using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour {
    public float speedy;
    public Vector3 from;
    public Vector3 to;
    public Vector3 start;
    Vector3 currentTarget;

    private void Awake()
    {
		from = new Vector2(transform.position.x + from.x,transform.position.y + from.y);
		to = new Vector2(transform.position.x + to.x,transform.localPosition.y + to.y);
		//Vector3 InverseTransformVector = transform.InverseTransformVector(to);
		currentTarget = to;
    }
    void Update () {
        float distanceToNextPath = .2f;
        Vector3 dir = currentTarget - transform.position;
        dir.Normalize();
        transform.position += dir * speedy * Time.deltaTime;
        if (Vector3.Distance(transform.localPosition, currentTarget) < distanceToNextPath)
        {
            if(currentTarget == to)
            {
                currentTarget = from;
            }
            else
            {
                currentTarget = to;
            }
        }
	}
    private void OnDrawGizmosSelected()
    {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.localPosition + (from), 0.3f);
        Gizmos.DrawSphere(transform.localPosition + (to), 0.3f);
        Gizmos.DrawSphere(transform.localPosition + (start), 0.3f);
    }
}
