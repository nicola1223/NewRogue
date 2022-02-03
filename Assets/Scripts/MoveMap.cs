using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMap : MonoBehaviour
{
    public Vector3 target;
    public float speed = 10;

    void FixedUpdate()
    {
        if (transform.position != target)
        {
            MoveToPoint();
        }
    }

    void MoveToPoint()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed);
    }
}
