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
    public Material activeMat; // Материал который будет на карте, когда в ней игрок
    public Material notFree; // Материал неизведаных комнат
    public Material defaultMat;

    [HideInInspector]
    public bool isFree = false;
    [HideInInspector]
    public bool Activated = false;
    // Булевая переменная были ли мы здесь
    [HideInInspector]
    public bool unclocked = false;
    // Булевая переменная, чтобы успела прогрузиться карта
    private bool CanDo = false;

    private Door[] doors; // Список всех дверей

    private void Awake()
    {
        // Задаем вручную
        //defaultMat = Map.GetComponent<Renderer>().material; // Стандартный материал для карты
        doors = GetComponentsInChildren<Door>();
    }

    private void Start()
    {
        if (Random.value < 0.91f) // Вероятность 91%
        {
            isFree = false; // Не свободна
        }
        else
        {
            isFree = true; // Свободна
        }
        StartCoroutine(CanDoIt());
    }

    // Куротина для задержки
    private IEnumerator CanDoIt()
    {
        yield return new WaitForSeconds(1.1f);
        CanDo = true;
    }
    // Куротина для задержки

    private void Update()
    {
        if (CanDo)
        {
            foreach (var door in doors)
            {
                if (Activated) // Если в ней сейчас игрок
                {
                    // Пробел - открыть комнаты (временно)
                    if (Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        isFree = true;
                    }
                    // Пробел - открыть комнаты (временно)
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
                if (child.tag != "trigger" && child != camera && child.GetComponent<Door>() == null && child.tag != "Map") // Включаем все, только не трогаем trigger, карту и двери
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
                if (child.tag != "trigger" && child != camera && child.tag != "Map") // Выключаем все кроме triggerа, камеры и карты
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
