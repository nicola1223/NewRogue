using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    public CompletedRoom[] RoomPrefabs; //Префабы комнат, из которых будет состоять уровень
    public CompletedRoom StartRoom; // Первая комната на сцене, к ней будут присоединятся остальные комнаты (положение (0, 0, 0))
    [Space] // Пробел в инспекторе, так лучше выглядит
    [Space]
    public int HeshSize = 11; // Длина и ширина поля в котором могут спавнится комнаты, для создания массива комнат
    public int RoomsCount = 13; // Количество создаваемых комнат
    [Space]
    [Space]
    public float RoomScale = 10; // Размеры одинарной обычной комнаты

    //Debug only
    public enum Mode
    {
        StandartMode,
        DebugMode
    }
    [Space]
    [Space]
    public Mode mode;
    public int RoomIndex = 0;
    //Debug only

    private int maxX; // Максимальный x для векторов, чтобы не выйти за рамки массива 
    private int maxY; // Максимальный x для векторов, чтобы не выйти за рамки массива

    private CompletedRoom[,] spawnedRooms; // Массив комнат, которые уже заспавнились
    [HideInInspector] // Прячет public переменную в инспекторе
    public CompletedRoom[] RoomsToPlayer; // Массив всех существующих комнат для игрока, чтобы активировать их

    private IEnumerator Start()
    {
        spawnedRooms = new CompletedRoom[HeshSize, HeshSize]; // Инициализируем переменные
        spawnedRooms[(int)HeshSize / 2, (int)HeshSize / 2] = StartRoom; // Ставим первую комнату в центре массива
        RoomsToPlayer = new CompletedRoom[RoomsCount];
        RoomsToPlayer[RoomsCount - 1] = StartRoom; // Последняя комната - стартовая, чтобы было удобнее использовать for

        maxX = spawnedRooms.GetLength(0) - 1;
        maxY = spawnedRooms.GetLength(1) - 1;

        if (mode == Mode.StandartMode)
        {
            for (int i = 0; i < RoomsCount - 1; i++) // -1 потому что 1 комната уже есть (StartRoom)
            {
                PlaceRooms(i); // Расставляем комнаты, передаем i для записи в RoomsToPlayer, я не придумал ничего лучше
                yield return new WaitForSeconds(0.0000001f); // Задержка, я фиг знает зачем, но без нее открываются ненужные двери
            }
            //Начинаем игру включив триггер у первой комнаты, что активирует ее и отключит другие комнаты
            yield return new WaitForSecondsRealtime(1.2f);
            for (int i = 0; i < StartRoom.transform.childCount; i++) // пробегаемся по всем child первой комнаты
            {
                if (StartRoom.transform.GetChild(i).tag == "trigger") // Если находим триггер
                {
                    StartRoom.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
    }

    // Нужен только для DebugMode
    private void Update()
    {
        if (mode == Mode.DebugMode)
        {
            if (Input.GetKeyDown(KeyCode.Space) && RoomIndex < RoomsCount - 1)
            {
                PlaceRooms(RoomIndex);
                RoomIndex++;
            }
            else if (Input.GetKeyDown(KeyCode.Space) && RoomIndex >= RoomsCount - 1)
            {
                Debug.LogWarning("Больше нельзя!");
            }
        }
    }
    // Нужен только для DebugMode

    private void PlaceRooms(int i)
    {
        int iteration = 0; // Номер итерации, нужен, чтобы в случае если не получиться поставить комнату какое-то количество раз все не зависло и не ушло в бесконечный цикл        
        spawn: // Указатель нужен если не получиться поставить или присоеденить комнату, опять же не придумал ничего лучше

        HashSet<Vector2Int> vacantPlaces = new HashSet<Vector2Int>();
        for (int x = 0; x < spawnedRooms.GetLength(0); x++) // Проходимся по всем x
        {
            for (int y = 0; y < spawnedRooms.GetLength(1); y++) // Проходимся по всем y
            {
                if (spawnedRooms[x, y] == null) continue; // Если нет комнаты продолжаем

                if (x > 0 && spawnedRooms[x - 1, y] == null) vacantPlaces.Add(new Vector2Int(x - 1, y)); // Если нет комнаты слева добавляем координаты
                if (y > 0 && spawnedRooms[x, y - 1] == null) vacantPlaces.Add(new Vector2Int(x, y - 1)); // Если нет комнаты снизу добавляем координаты
                if (x < maxX && spawnedRooms[x + 1, y] == null) vacantPlaces.Add(new Vector2Int(x + 1, y)); // Если нет комнаты справа добавляем координаты
                if (y < maxY && spawnedRooms[x, y + 1] == null) vacantPlaces.Add(new Vector2Int(x, y + 1)); // Если нет комнаты сверху добавляем координаты
            }
        }

        // Начинаем расставлять
        CompletedRoom newRoom = Instantiate(RoomPrefabs[UnityEngine.Random.Range(0, RoomPrefabs.Length)]); // Создаем копию рандомной комнаты из массива
        // newRoom.RotateRandomly() // Функция для рандомного вращения но, по факту нужна лишь сокровищницам, пока закоментирована, потому что будет работать не у всех комнат

        Vector2Int position = vacantPlaces.ElementAt(UnityEngine.Random.Range(0, vacantPlaces.Count)); // Выбираем рандомную позицию, он будет примерно (0,1) или (5,4) то есть в единичной сетке

        switch (newRoom.Type) // В зависимости от типа разный алгоритм
        {
            case CompletedRoom.RoomType.Default: // Стандартная комната
                float posX = (position.x - ((int)HeshSize / 2)) * RoomScale; // Позиция x для комнаты, (position.x - ((int)HeshSize / 2)) нужно для нормального позиционирования, так как первая комната стоит в реальных координатах (0, 0) а в массиве В уентре (5, 5)
                float posZ = (position.y - ((int)HeshSize / 2)) * RoomScale; // Позиция y для комнаты, (position.x - ((int)HeshSize / 2)) нужно для нормального позиционирования, так как первая комната стоит в реальных координатах (0, 0) а в массиве В уентре (5, 5)

                newRoom.transform.position = new Vector3(posX, 0, posZ);
                spawnedRooms[position.x, position.y] = newRoom; // Записываем в массив
                RoomsToPlayer[i] = newRoom; // Передаем игроку

                bool connected = ConnectToRoom(newRoom, position); // Пытаемся присоединиться записываем результат в переменную
                if (!connected)
                {
                    Destroy(newRoom.gameObject); // Удаляем поставленную комнату
                    spawnedRooms[position.x, position.y] = null; // Убираем из массива
                    if (iteration < 500) goto spawn; // Если не прошло 500 итераций пробуем снова
                    else break; // выходим
                }
                break;
            case CompletedRoom.RoomType.BigRoom: // Большая 2x2 
                Vector3 pos = LocateBigRoom2x2(newRoom, position);

                newRoom.transform.position = pos;
                RoomsToPlayer[i] = newRoom;

                bool connected_big = ConnectToRoom(newRoom, position); // Пытаемся присоединиться записываем результат в переменну
                if ((pos.x == 0 && pos.z == 0) || !connected_big)
                {
                    Destroy(newRoom.gameObject); // Удаляем поставленную комнату
                    spawnedRooms[position.x, position.y] = null; // Убираем из массива
                    if (iteration < 500) goto spawn; // Если не прошло 500 итераций пробуем снова
                    else break; // выходим
                }
                break;
        }
    }

    private bool ConnectToRoom(CompletedRoom room, Vector2Int pos)
    {
        Door[] doors = room.GetComponentsInChildren<Door>(); // Получаем все двери комнаты

        Door[] selectedDoors = new Door[2]; // Двери которые будем прорезать, у больших комнат их две

        List<Vector2Int> neighbours = new List<Vector2Int>(); // Лист соседей к которым можно присоединиться

        if (pos.x > 0 && spawnedRooms[pos.x - 1, pos.y] != null && spawnedRooms[pos.x - 1, pos.y] != room) neighbours.Add(Vector2Int.left); // Если слева есть комната добавляем направление, проверяем не равна ли комната сосед комнате которую мы ставим (большие комнаты занимают четыре)
        if (pos.y > 0 && spawnedRooms[pos.x, pos.y - 1] != null && spawnedRooms[pos.x, pos.y - 1] != room) neighbours.Add(Vector2Int.down); // Если снизу есть комната добавляем направление, проверяем не равна ли комната сосед комнате которую мы ставим (большие комнаты занимают четыре)
        if (pos.x < maxX && spawnedRooms[pos.x + 1, pos.y] != null && spawnedRooms[pos.x + 1, pos.y] != room) neighbours.Add(Vector2Int.right); // Если справа есть комната добавляем направление, проверяем не равна ли комната сосед комнате которую мы ставим (большие комнаты занимают четыре)
        if (pos.y < maxY && spawnedRooms[pos.x, pos.y + 1] != null && spawnedRooms[pos.x, pos.y + 1] != room) neighbours.Add(Vector2Int.up); // Если сверху есть комната добавляем направление, проверяем не равна ли комната сосед комнате которую мы ставим (большие комнаты занимают четыре)

        if (neighbours.Count <= 0) // Если нет соседей
        {
            return false;
        }

        Vector2Int selectedDirection = neighbours[UnityEngine.Random.Range(0, neighbours.Count)]; // Выбираем рандомное направление

        float maxP = 0;
        // Отбираем двери
        foreach (var door in doors)
        {
            if (selectedDirection == Vector2Int.up)
            {
                if (door.transform.position.z > room.transform.position.z)
                {
                    if (maxP == 0 || door.transform.position.z > maxP)
                    {
                        selectedDoors[0] = door;
                        maxP = door.transform.position.z;
                    }
                    else if (door.transform.position.z == maxP)
                    {
                        selectedDoors[1] = door;
                    }
                }
            }
            else if (selectedDirection == Vector2Int.down)
            {
                if (door.transform.position.z < room.transform.position.z)
                {
                    if (maxP == 0 || door.transform.position.z < maxP)
                    {
                        selectedDoors[0] = door;
                        maxP = door.transform.position.z;
                    }
                    else if (door.transform.position.z == maxP)
                    {
                        selectedDoors[1] = door;
                    }
                }
            }
            else if (selectedDirection == Vector2Int.right)
            {
                if (door.transform.position.x > room.transform.position.x)
                {
                    if (maxP == 0 || door.transform.position.x > maxP)
                    {
                        selectedDoors[0] = door;
                        maxP = door.transform.position.z;
                    }
                    else if (door.transform.position.z == maxP)
                    {
                        selectedDoors[1] = door;
                    }
                }
            }
            else if (selectedDirection == Vector2Int.left)
            {
                if (door.transform.position.x < room.transform.position.x)
                {
                    if (maxP == 0 || door.transform.position.x < maxP)
                    {
                        selectedDoors[0] = door;
                        maxP = door.transform.position.z;
                    }
                    else if (door.transform.position.z == maxP)
                    {
                        selectedDoors[1] = door;
                    }
                }
            }
        }

        // Если комната большая выбираем одну из двух, если маленькая, то только одну

        int index = UnityEngine.Random.Range(0, selectedDoors.Length);
        if (selectedDoors[index] != null)
        {
            selectedDoors[index].gameObject.GetComponents<BoxCollider>()[1].enabled = true; // Триггер на дверях вырезает дверь напротив
        }
        else
        {
            selectedDoors[0].gameObject.GetComponents<BoxCollider>()[1].enabled = true; // Триггер на дверях вырезает дверь напротив
        }

        return true;
    }

    private Vector3 LocateBigRoom2x2(CompletedRoom newRoom, Vector2Int pos)
    {
        float posX = 0f;
        float posZ = 0f;

        if (pos.x > 0 && pos.y > 0 && pos.x < maxX && pos.y < maxY) // Если не выходим за границы массива
        {
            // Находим комнаты справа, слева, сверху, снизу
            CompletedRoom UpRoom = spawnedRooms[pos.x, pos.y + 1];
            CompletedRoom DownRoom = spawnedRooms[pos.x, pos.y - 1];
            CompletedRoom RightRoom = spawnedRooms[pos.x + 1, pos.y];
            CompletedRoom LeftRoom = spawnedRooms[pos.x - 1, pos.y];

            if (UpRoom == null && pos.x + 1 < maxX && pos.x - 1 > 0 && pos.y + 1 < maxY && pos.y - 1 > 0) // Если нет комнаты сверху
            {
                if (RightRoom == null) // Если нет комнаты справа
                {
                    if (spawnedRooms[pos.x + 1, pos.y + 1] == null) // Если нет комнаты сверху справа, то ставим комнату на эти четыре места
                    {
                        posX = ((pos.x - ((int)HeshSize / 2)) * RoomScale) + RoomScale / 2; // такой же принцип, что и с маленькими, но отступ в 1.5
                        posZ = ((pos.y - ((int)HeshSize / 2)) * RoomScale) + RoomScale / 2; //

                        spawnedRooms[pos.x, pos.y] = newRoom;
                        spawnedRooms[pos.x + 1, pos.y] = newRoom;
                        spawnedRooms[pos.x, pos.y + 1] = newRoom;
                        spawnedRooms[pos.x + 1, pos.y + 1] = newRoom;
                    }
                }
                else if (LeftRoom == null) // Если нет комнаты слева
                {
                    if (spawnedRooms[pos.x - 1, pos.y + 1] == null) // Если нет комнаты сверху слева
                    {
                        posX = ((pos.x - ((int)HeshSize / 2)) * RoomScale) - RoomScale / 2;
                        posZ = ((pos.y - ((int)HeshSize / 2)) * RoomScale) + RoomScale / 2;

                        spawnedRooms[pos.x, pos.y] = newRoom;
                        spawnedRooms[pos.x - 1, pos.y] = newRoom;
                        spawnedRooms[pos.x, pos.y + 1] = newRoom;
                        spawnedRooms[pos.x - 1, pos.y + 1] = newRoom;
                    }
                }
            }
            else if (DownRoom == null && pos.x + 1 < maxX && pos.x - 1 > 0 && pos.y + 1 < maxY && pos.y - 1 > 0) // Если нет комнаты снизу
            {
                if (RightRoom == null) // Если нет комнаты справа
                {
                    if (spawnedRooms[pos.x + 1, pos.y - 1] == null) // Если нет комнаты снизу справа
                    {
                        posX = ((pos.x - ((int)HeshSize / 2)) * RoomScale) + RoomScale / 2;
                        posZ = ((pos.y - ((int)HeshSize / 2)) * RoomScale) - RoomScale / 2;

                        spawnedRooms[pos.x, pos.y] = newRoom;
                        spawnedRooms[pos.x + 1, pos.y] = newRoom;
                        spawnedRooms[pos.x, pos.y - 1] = newRoom;
                        spawnedRooms[pos.x + 1, pos.y - 1] = newRoom;
                    }
                }
                else if (LeftRoom == null) // Если нет комнаты слева
                {
                    if (spawnedRooms[pos.x - 1, pos.y - 1] == null) // Если нет комнаты снизу слева
                    {
                        posX = ((pos.x - ((int)HeshSize / 2)) * RoomScale) - RoomScale / 2;
                        posZ = ((pos.y - ((int)HeshSize / 2)) * RoomScale) - RoomScale / 2;

                        spawnedRooms[pos.x, pos.y] = newRoom;
                        spawnedRooms[pos.x - 1, pos.y] = newRoom;
                        spawnedRooms[pos.x, pos.y - 1] = newRoom;
                        spawnedRooms[pos.x - 1, pos.y - 1] = newRoom;
                    }
                }
            }
        }

        return new Vector3(posX, 0, posZ);
    }
}
