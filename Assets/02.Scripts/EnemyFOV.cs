using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    public Transform target;
    public float viewRange = 15f;
    [Range(0, 360)]
    public float viewAngle = 120f;

    public Vector3 CirclePoint(float angle)
    {
        angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    public bool isTracePlayer()
    {
        bool isTrace = false;
        var colls = Physics.OverlapSphere(transform.position, viewRange, LayerMask.GetMask("Player"));
        if(colls.Length > 0)
        {
            var dir = target.position - transform.position;
            dir = dir.normalized;
            if(Vector3.Angle(transform.forward, dir) < viewAngle * 0.5f)
            {
                isTrace = true;
            }
        }
        return isTrace;
    }


}
