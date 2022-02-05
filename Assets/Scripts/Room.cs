using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public enum RoomType
    {
        Default,
        BigRoom
    }
    [Space]
    [Space]
    [HideInInspector]
    public RoomType Type;

    [HideInInspector]
    public bool isFree = false;
    [HideInInspector]
    public bool Activated = false;
    
    [Space]
    [Space]
    public Material activeMat;
    private Material defaultMat;

    [Space]
    [Space]
    [HideInInspector]
    public GameObject DoorU;
    [HideInInspector]
    public GameObject DoorD;
    [HideInInspector]
    public GameObject DoorL;
    [HideInInspector]
    public GameObject DoorR;

    [HideInInspector]
    public GameObject DoorUR;
    [HideInInspector]
    public GameObject DoorDR;
    [HideInInspector]
    public GameObject DoorLU;
    [HideInInspector]
    public GameObject DoorRU;
    [HideInInspector]
    public GameObject DoorUL;
    [HideInInspector]
    public GameObject DoorDL;
    [HideInInspector]
    public GameObject DoorLD;
    [HideInInspector]
    public GameObject DoorRD;

    [HideInInspector]
    public bool DoorU_Active = true;
    [HideInInspector]
    public bool DoorD_Active = true;
    [HideInInspector]
    public bool DoorL_Active = true;
    [HideInInspector]
    public bool DoorR_Active = true;

    [Space]
    [Space]
    public GameObject Map;
    [HideInInspector]
    public GameObject DoorUMap;
    [HideInInspector]
    public GameObject DoorDMap;
    [HideInInspector]
    public GameObject DoorLMap;
    [HideInInspector]
    public GameObject DoorRMap;

    [HideInInspector]
    public GameObject DoorURMap;
    [HideInInspector]
    public GameObject DoorDRMap;
    [HideInInspector]
    public GameObject DoorLUMap;
    [HideInInspector]
    public GameObject DoorRUMap;
    [HideInInspector]
    public GameObject DoorULMap;
    [HideInInspector]
    public GameObject DoorDLMap;
    [HideInInspector]
    public GameObject DoorLDMap;
    [HideInInspector]
    public GameObject DoorRDMap;

    [Space]
    [Space]
    public Vector2 size;

    private void Awake()
    {
        defaultMat = Map.GetComponent<Renderer>().material;
        Transform floor = transform.Find("Floor").transform;
        size = new Vector2(floor.localScale.x, floor.localScale.z);
    }

    //void Start()
    //{
    //    if (Random.value < 0.7f)
    //    {
    //        isFree = false;
    //    }
    //    else
    //    {
    //        isFree = true;
    //    }
    //    Debug.Log(isFree);
    //    StartCoroutine(SetMap());
    //}

    //private void Update()
    //{
    //    if (Activated)
    //    {
    //        if (Input.GetKeyDown(KeyCode.Space))
    //        {
    //            isFree = !isFree;
    //        }
    //        if (isFree)
    //        {
    //            DoorD.SetActive(DoorD_Active);
    //            DoorU.SetActive(DoorU_Active);
    //            DoorR.SetActive(DoorR_Active);
    //            DoorL.SetActive(DoorL_Active);
    //        }
    //        else
    //        {
    //            DoorD.SetActive(true);
    //            DoorU.SetActive(true);
    //            DoorR.SetActive(true);
    //            DoorL.SetActive(true);
    //        }
    //    }
    //    else
    //    {
    //        DoorD.SetActive(false);
    //        DoorU.SetActive(false);
    //        DoorR.SetActive(false);
    //        DoorL.SetActive(false);
    //    }
    //}

    public void SetActive(bool active)
    {
        Activated = active;
        GameObject camera = gameObject.transform.Find("RoomCamera").gameObject;
        if (active)
        {
            DoorD.SetActive(DoorD_Active);
            DoorU.SetActive(DoorU_Active);
            DoorR.SetActive(DoorR_Active);
            DoorL.SetActive(DoorL_Active);
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = gameObject.transform.GetChild(i).gameObject;
                if (child.tag != "trigger" && child != camera && child != DoorD && child != DoorL && child != DoorR && child != DoorU && child.tag != "Map")
                {
                    gameObject.transform.GetChild(i).gameObject.SetActive(active);
                }
                else if (child.tag == "trigger")
                {
                    child.SetActive(false);
                }
            }
            camera.SetActive(active);
            Map.GetComponent<Renderer>().material = activeMat;
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = gameObject.transform.GetChild(i).gameObject;
                if (child.tag != "trigger" && child != camera && child.tag != "Map")
                {
                    gameObject.transform.GetChild(i).gameObject.SetActive(active);
                }
                else if (child.tag == "trigger")
                {
                    child.SetActive(true);
                }
            }
            camera.SetActive(active);
            Map.GetComponent<Renderer>().material = defaultMat;
        }
    }

    public void RotateRandomly()
    {
        Transform camera = gameObject.transform.Find("RoomCamera");

        int count = Random.Range(0, 4);

        for (int i = 0; i < count; i++)
        {
            transform.Rotate(0, 90, 0);
            camera.RotateAround(transform.position, new Vector3(0, 1, 0), -90);

            switch (Type)
            {
                case RoomType.Default:
                    GameObject tmp = DoorL;
                    DoorL = DoorD;
                    DoorD = DoorR;
                    DoorR = DoorU;
                    DoorU = tmp;

                    GameObject tmp1 = DoorLMap;
                    DoorLMap = DoorDMap;
                    DoorDMap = DoorRMap;
                    DoorRMap = DoorUMap;
                    DoorUMap = tmp1;

                    break;
                case RoomType.BigRoom:
                    GameObject tmpbU = DoorLU;
                    GameObject tmpbD = DoorLD;
                    DoorLU = DoorDR;
                    DoorLD = DoorDL;
                    DoorDL = DoorRD;
                    DoorDR = DoorRU;
                    DoorRD = DoorUR;
                    DoorRU = DoorUL;
                    DoorUR = tmpbU;
                    DoorUL = tmpbD;

                    GameObject tmpbU1 = DoorLUMap;
                    GameObject tmpbD1 = DoorLDMap;
                    DoorLUMap = DoorDRMap;
                    DoorLDMap = DoorDLMap;
                    DoorDLMap = DoorRDMap;
                    DoorDRMap = DoorRUMap;
                    DoorRDMap = DoorURMap;
                    DoorRUMap = DoorULMap;
                    DoorURMap = tmpbU1;
                    DoorULMap = tmpbD1;

                    break;
            }
        }
    }

    IEnumerator SetMap()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        DoorDMap.SetActive(!DoorD_Active);
        DoorUMap.SetActive(!DoorU_Active);
        DoorRMap.SetActive(!DoorR_Active);
        DoorLMap.SetActive(!DoorL_Active);
    }
}
