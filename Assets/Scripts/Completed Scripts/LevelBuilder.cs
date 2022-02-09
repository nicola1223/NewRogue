using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    public CompletedRoom[] RoomPrefabs; //������� ������, �� ������� ����� �������� �������
    public CompletedRoom StartRoom; // ������ ������� �� �����, � ��� ����� ������������� ��������� ������� (��������� (0, 0, 0))
    [Space] // ������ � ����������, ��� ����� ��������
    [Space]
    public int HeshSize = 11; // ����� � ������ ���� � ������� ����� ��������� �������, ��� �������� ������� ������
    public int RoomsCount = 13; // ���������� ����������� ������
    [Space]
    [Space]
    public float RoomScale = 10; // ������� ��������� ������� �������

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

    private int maxX; // ������������ x ��� ��������, ����� �� ����� �� ����� ������� 
    private int maxY; // ������������ x ��� ��������, ����� �� ����� �� ����� �������

    private CompletedRoom[,] spawnedRooms; // ������ ������, ������� ��� ������������
    [HideInInspector] // ������ public ���������� � ����������
    public CompletedRoom[] RoomsToPlayer; // ������ ���� ������������ ������ ��� ������, ����� ������������ ��

    private IEnumerator Start()
    {
        spawnedRooms = new CompletedRoom[HeshSize, HeshSize]; // �������������� ����������
        spawnedRooms[(int)HeshSize / 2, (int)HeshSize / 2] = StartRoom; // ������ ������ ������� � ������ �������
        RoomsToPlayer = new CompletedRoom[RoomsCount];
        RoomsToPlayer[RoomsCount - 1] = StartRoom; // ��������� ������� - ���������, ����� ���� ������� ������������ for

        maxX = spawnedRooms.GetLength(0) - 1;
        maxY = spawnedRooms.GetLength(1) - 1;

        if (mode == Mode.StandartMode)
        {
            for (int i = 0; i < RoomsCount - 1; i++) // -1 ������ ��� 1 ������� ��� ���� (StartRoom)
            {
                PlaceRooms(i); // ����������� �������, �������� i ��� ������ � RoomsToPlayer, � �� �������� ������ �����
                yield return new WaitForSeconds(0.0000001f); // ��������, � ��� ����� �����, �� ��� ��� ����������� �������� �����
            }
            //�������� ���� ������� ������� � ������ �������, ��� ���������� �� � �������� ������ �������
            yield return new WaitForSecondsRealtime(1.2f);
            for (int i = 0; i < StartRoom.transform.childCount; i++) // ����������� �� ���� child ������ �������
            {
                if (StartRoom.transform.GetChild(i).tag == "trigger") // ���� ������� �������
                {
                    StartRoom.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
    }

    // ����� ������ ��� DebugMode
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
                Debug.LogWarning("������ ������!");
            }
        }
    }
    // ����� ������ ��� DebugMode

    private void PlaceRooms(int i)
    {
        int iteration = 0; // ����� ��������, �����, ����� � ������ ���� �� ���������� ��������� ������� �����-�� ���������� ��� ��� �� ������� � �� ���� � ����������� ����        
        spawn: // ��������� ����� ���� �� ���������� ��������� ��� ������������ �������, ����� �� �� �������� ������ �����

        HashSet<Vector2Int> vacantPlaces = new HashSet<Vector2Int>();
        for (int x = 0; x < spawnedRooms.GetLength(0); x++) // ���������� �� ���� x
        {
            for (int y = 0; y < spawnedRooms.GetLength(1); y++) // ���������� �� ���� y
            {
                if (spawnedRooms[x, y] == null) continue; // ���� ��� ������� ����������

                if (x > 0 && spawnedRooms[x - 1, y] == null) vacantPlaces.Add(new Vector2Int(x - 1, y)); // ���� ��� ������� ����� ��������� ����������
                if (y > 0 && spawnedRooms[x, y - 1] == null) vacantPlaces.Add(new Vector2Int(x, y - 1)); // ���� ��� ������� ����� ��������� ����������
                if (x < maxX && spawnedRooms[x + 1, y] == null) vacantPlaces.Add(new Vector2Int(x + 1, y)); // ���� ��� ������� ������ ��������� ����������
                if (y < maxY && spawnedRooms[x, y + 1] == null) vacantPlaces.Add(new Vector2Int(x, y + 1)); // ���� ��� ������� ������ ��������� ����������
            }
        }

        // �������� �����������
        CompletedRoom newRoom = Instantiate(RoomPrefabs[UnityEngine.Random.Range(0, RoomPrefabs.Length)]); // ������� ����� ��������� ������� �� �������
        // newRoom.RotateRandomly() // ������� ��� ���������� �������� ��, �� ����� ����� ���� �������������, ���� ���������������, ������ ��� ����� �������� �� � ���� ������

        Vector2Int position = vacantPlaces.ElementAt(UnityEngine.Random.Range(0, vacantPlaces.Count)); // �������� ��������� �������, �� ����� �������� (0,1) ��� (5,4) �� ���� � ��������� �����

        switch (newRoom.Type) // � ����������� �� ���� ������ ��������
        {
            case CompletedRoom.RoomType.Default: // ����������� �������
                float posX = (position.x - ((int)HeshSize / 2)) * RoomScale; // ������� x ��� �������, (position.x - ((int)HeshSize / 2)) ����� ��� ����������� ����������������, ��� ��� ������ ������� ����� � �������� ����������� (0, 0) � � ������� � ������ (5, 5)
                float posZ = (position.y - ((int)HeshSize / 2)) * RoomScale; // ������� y ��� �������, (position.x - ((int)HeshSize / 2)) ����� ��� ����������� ����������������, ��� ��� ������ ������� ����� � �������� ����������� (0, 0) � � ������� � ������ (5, 5)

                newRoom.transform.position = new Vector3(posX, 0, posZ);
                spawnedRooms[position.x, position.y] = newRoom; // ���������� � ������
                RoomsToPlayer[i] = newRoom; // �������� ������

                bool connected = ConnectToRoom(newRoom, position); // �������� �������������� ���������� ��������� � ����������
                if (!connected)
                {
                    Destroy(newRoom.gameObject); // ������� ������������ �������
                    spawnedRooms[position.x, position.y] = null; // ������� �� �������
                    if (iteration < 500) goto spawn; // ���� �� ������ 500 �������� ������� �����
                    else break; // �������
                }
                break;
            case CompletedRoom.RoomType.BigRoom: // ������� 2x2 
                Vector3 pos = LocateBigRoom2x2(newRoom, position);

                newRoom.transform.position = pos;
                RoomsToPlayer[i] = newRoom;

                bool connected_big = ConnectToRoom(newRoom, position); // �������� �������������� ���������� ��������� � ���������
                if ((pos.x == 0 && pos.z == 0) || !connected_big)
                {
                    Destroy(newRoom.gameObject); // ������� ������������ �������
                    spawnedRooms[position.x, position.y] = null; // ������� �� �������
                    if (iteration < 500) goto spawn; // ���� �� ������ 500 �������� ������� �����
                    else break; // �������
                }
                break;
        }
    }

    private bool ConnectToRoom(CompletedRoom room, Vector2Int pos)
    {
        Door[] doors = room.GetComponentsInChildren<Door>(); // �������� ��� ����� �������

        Door[] selectedDoors = new Door[2]; // ����� ������� ����� ���������, � ������� ������ �� ���

        List<Vector2Int> neighbours = new List<Vector2Int>(); // ���� ������� � ������� ����� ��������������

        if (pos.x > 0 && spawnedRooms[pos.x - 1, pos.y] != null && spawnedRooms[pos.x - 1, pos.y] != room) neighbours.Add(Vector2Int.left); // ���� ����� ���� ������� ��������� �����������, ��������� �� ����� �� ������� ����� ������� ������� �� ������ (������� ������� �������� ������)
        if (pos.y > 0 && spawnedRooms[pos.x, pos.y - 1] != null && spawnedRooms[pos.x, pos.y - 1] != room) neighbours.Add(Vector2Int.down); // ���� ����� ���� ������� ��������� �����������, ��������� �� ����� �� ������� ����� ������� ������� �� ������ (������� ������� �������� ������)
        if (pos.x < maxX && spawnedRooms[pos.x + 1, pos.y] != null && spawnedRooms[pos.x + 1, pos.y] != room) neighbours.Add(Vector2Int.right); // ���� ������ ���� ������� ��������� �����������, ��������� �� ����� �� ������� ����� ������� ������� �� ������ (������� ������� �������� ������)
        if (pos.y < maxY && spawnedRooms[pos.x, pos.y + 1] != null && spawnedRooms[pos.x, pos.y + 1] != room) neighbours.Add(Vector2Int.up); // ���� ������ ���� ������� ��������� �����������, ��������� �� ����� �� ������� ����� ������� ������� �� ������ (������� ������� �������� ������)

        if (neighbours.Count <= 0) // ���� ��� �������
        {
            return false;
        }

        Vector2Int selectedDirection = neighbours[UnityEngine.Random.Range(0, neighbours.Count)]; // �������� ��������� �����������

        float maxP = 0;
        // �������� �����
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

        // ���� ������� ������� �������� ���� �� ����, ���� ���������, �� ������ ����

        int index = UnityEngine.Random.Range(0, selectedDoors.Length);
        if (selectedDoors[index] != null)
        {
            selectedDoors[index].gameObject.GetComponents<BoxCollider>()[1].enabled = true; // ������� �� ������ �������� ����� ��������
        }
        else
        {
            selectedDoors[0].gameObject.GetComponents<BoxCollider>()[1].enabled = true; // ������� �� ������ �������� ����� ��������
        }

        return true;
    }

    private Vector3 LocateBigRoom2x2(CompletedRoom newRoom, Vector2Int pos)
    {
        float posX = 0f;
        float posZ = 0f;

        if (pos.x > 0 && pos.y > 0 && pos.x < maxX && pos.y < maxY) // ���� �� ������� �� ������� �������
        {
            // ������� ������� ������, �����, ������, �����
            CompletedRoom UpRoom = spawnedRooms[pos.x, pos.y + 1];
            CompletedRoom DownRoom = spawnedRooms[pos.x, pos.y - 1];
            CompletedRoom RightRoom = spawnedRooms[pos.x + 1, pos.y];
            CompletedRoom LeftRoom = spawnedRooms[pos.x - 1, pos.y];

            if (UpRoom == null && pos.x + 1 < maxX && pos.x - 1 > 0 && pos.y + 1 < maxY && pos.y - 1 > 0) // ���� ��� ������� ������
            {
                if (RightRoom == null) // ���� ��� ������� ������
                {
                    if (spawnedRooms[pos.x + 1, pos.y + 1] == null) // ���� ��� ������� ������ ������, �� ������ ������� �� ��� ������ �����
                    {
                        posX = ((pos.x - ((int)HeshSize / 2)) * RoomScale) + RoomScale / 2; // ����� �� �������, ��� � � ����������, �� ������ � 1.5
                        posZ = ((pos.y - ((int)HeshSize / 2)) * RoomScale) + RoomScale / 2; //

                        spawnedRooms[pos.x, pos.y] = newRoom;
                        spawnedRooms[pos.x + 1, pos.y] = newRoom;
                        spawnedRooms[pos.x, pos.y + 1] = newRoom;
                        spawnedRooms[pos.x + 1, pos.y + 1] = newRoom;
                    }
                }
                else if (LeftRoom == null) // ���� ��� ������� �����
                {
                    if (spawnedRooms[pos.x - 1, pos.y + 1] == null) // ���� ��� ������� ������ �����
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
            else if (DownRoom == null && pos.x + 1 < maxX && pos.x - 1 > 0 && pos.y + 1 < maxY && pos.y - 1 > 0) // ���� ��� ������� �����
            {
                if (RightRoom == null) // ���� ��� ������� ������
                {
                    if (spawnedRooms[pos.x + 1, pos.y - 1] == null) // ���� ��� ������� ����� ������
                    {
                        posX = ((pos.x - ((int)HeshSize / 2)) * RoomScale) + RoomScale / 2;
                        posZ = ((pos.y - ((int)HeshSize / 2)) * RoomScale) - RoomScale / 2;

                        spawnedRooms[pos.x, pos.y] = newRoom;
                        spawnedRooms[pos.x + 1, pos.y] = newRoom;
                        spawnedRooms[pos.x, pos.y - 1] = newRoom;
                        spawnedRooms[pos.x + 1, pos.y - 1] = newRoom;
                    }
                }
                else if (LeftRoom == null) // ���� ��� ������� �����
                {
                    if (spawnedRooms[pos.x - 1, pos.y - 1] == null) // ���� ��� ������� ����� �����
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
