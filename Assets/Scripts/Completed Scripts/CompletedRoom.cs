using System.Collections;
using UnityEngine;

public class CompletedRoom : MonoBehaviour
{
    public enum RoomType
    {
        Default,
        BigRoom
    }
    [Space]
    [Space]
    public RoomType Type;
    [Space]
    [Space]
    public GameObject Map;
    [Space]
    [Space]
    public Material activeMat; // �������� ������� ����� �� �����, ����� � ��� �����
    public Material notFree; // �������� ����������� ������
    public Material defaultMat;

    [HideInInspector]
    public bool isFree = false;
    [HideInInspector]
    public bool Activated = false;
    // ������� ���������� ���� �� �� �����
    [HideInInspector]
    public bool unclocked = false;
    // ������� ����������, ����� ������ ������������ �����
    private bool CanDo = false;

    private Door[] doors; // ������ ���� ������

    private void Awake()
    {
        // ������ �������
        //defaultMat = Map.GetComponent<Renderer>().material; // ����������� �������� ��� �����
        doors = GetComponentsInChildren<Door>();
    }

    private void Start()
    {
        if (Random.value < 0.91f) // ����������� 91%
        {
            isFree = false; // �� ��������
        }
        else
        {
            isFree = true; // ��������
        }
        StartCoroutine(CanDoIt());
    }

    // �������� ��� ��������
    private IEnumerator CanDoIt()
    {
        yield return new WaitForSeconds(1.1f);
        CanDo = true;
    }
    // �������� ��� ��������

    private void Update()
    {
        if (CanDo)
        {
            foreach (var door in doors)
            {
                if (Activated) // ���� � ��� ������ �����
                {
                    // ������ - ������� ������� (��������)
                    if (Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        isFree = true;
                    }
                    // ������ - ������� ������� (��������)
                    if (isFree)
                    {
                        door.gameObject.SetActive(door.DoorActive);
                    }
                    else
                    {
                        door.gameObject.SetActive(true);
                    }
                }
                else
                {
                    door.gameObject.SetActive(false);
                }
            }
        }
    }

    public void SetActive(bool active)
    {
        Activated = active;
        GameObject camera = gameObject.transform.Find("RoomCamera").gameObject;
        if (active)
        {
            
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = gameObject.transform.GetChild(i).gameObject;
                if (child.tag != "trigger" && child != camera && child.GetComponent<Door>() == null && child.tag != "Map") // �������� ���, ������ �� ������� trigger, ����� � �����
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
                if (child.tag != "trigger" && child != camera && child.tag != "Map") // ��������� ��� ����� trigger�, ������ � �����
                {
                    gameObject.transform.GetChild(i).gameObject.SetActive(active);
                }
                else if (child.tag == "trigger")
                {
                    child.SetActive(true);
                }
            }
            camera.SetActive(active);
            if (!unclocked)
            {
                Map.GetComponent<Renderer>().material = notFree;
                foreach (var door in doors)
                {
                    door.DoorMap.gameObject.GetComponent<Renderer>().material = notFree;
                }
            }
            else if (unclocked)
            {
                Map.GetComponent<Renderer>().material = defaultMat;
                foreach (var door in doors)
                {
                    door.DoorMap.gameObject.GetComponent<Renderer>().material = defaultMat;
                }
            }
        }
    }
}
