using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectRandomChildrens : MonoBehaviour
{
    public int CountOfChildren = 1;

    void Start()
    {
        while (transform.childCount > CountOfChildren)
        {
            Transform DestroyChild = transform.GetChild(Random.Range(0, transform.childCount));
            DestroyImmediate(DestroyChild.gameObject);
        }
    }
}
