using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomPlacerWithTypes : MonoBehaviour
{
    public Room[] RoomPrefabs;
    public Room StartingRoom;

    public int HeshLen = 11;
    public int RoomsCount = 13;

    private Room[,] spawnedRooms;
    public Room[] RoomsToPlayer;

    private int maxX;
    private int maxY;

    private void Start()
    {
        spawnedRooms = new Room[HeshLen, HeshLen];
        spawnedRooms[(int)HeshLen / 2, (int)HeshLen / 2] = StartingRoom;
        RoomsToPlayer = new Room[RoomsCount];
        RoomsToPlayer[RoomsCount - 1] = StartingRoom;

        for (int i = 0; i < RoomsCount - 1; i++)
        {
            PlaceRooms(i);
        }
    }

    private void PlaceRooms(int i)
    {
        spawn:
        HashSet<Vector2Int> vacantPlaces = new HashSet<Vector2Int>();
        for (int x = 0; x < spawnedRooms.GetLength(0); x++)
        {
            for (int y = 0; y < spawnedRooms.GetLength(1); y++)
            {
                if (spawnedRooms[x, y] == null) continue;

                maxX = spawnedRooms.GetLength(0) - 1;
                maxY = spawnedRooms.GetLength(1) - 1;

                if (x > 0 && spawnedRooms[x - 1, y] == null) vacantPlaces.Add(new Vector2Int(x - 1, y));
                if (y > 0 && spawnedRooms[x, y - 1] == null) vacantPlaces.Add(new Vector2Int(x, y - 1));
                if (x < maxX && spawnedRooms[x + 1, y] == null) vacantPlaces.Add(new Vector2Int(x + 1, y));
                if (y < maxY && spawnedRooms[x, y + 1] == null) vacantPlaces.Add(new Vector2Int(x, y + 1));
            }
        }

    

        Room newRoom = Instantiate(RoomPrefabs[Random.Range(0, RoomPrefabs.Length)]);
        newRoom.RotateRandomly();
        //newRoom.RotateRandomly();

        switch (newRoom.Type)
        {
            case Room.RoomType.Default:
                // Эту строчку можно заменить на выбор положения комнаты с учётом того насколько он далеко/близко от центра,
                // или сколько у него соседей, чтобы генерировать более плотные, или наоборот, растянутые данжи
                Vector2Int position = vacantPlaces.ElementAt(Random.Range(0, vacantPlaces.Count));

                //if (/*DefaultConnectToSomething(newRoom, position)*/ConnectToRoom(newRoom, position))
                //{
                    float posX = (position.x - ((int)HeshLen / 2)) * (newRoom.size.x);
                    float posY = (position.y - ((int)HeshLen / 2)) * (newRoom.size.y);
                    //Vector3 pos = TryToLocate(position, newRoom);

                    //if (pos.x != 100 || pos.y != 100)
                    //{
                    newRoom.transform.position = new Vector3(posX, 0, posY);
                    //}
                    //else break;

                    //newRoom.transform.position = new Vector3(posX, 0, posY);
                    spawnedRooms[position.x, position.y] = newRoom;
                    RoomsToPlayer[i] = newRoom;
                //return;
                //}
                //if (ConnectToRoom(newRoom, position))
                //{

                //}
                bool connected = ConnectToRoom(newRoom, position);
                if (/*!DefaultConnectToSomething(newRoom, position)*/!connected)
                {
                    Destroy(newRoom.gameObject);
                    goto spawn;
                }
                break;
            case Room.RoomType.BigRoom:
                //Destroy(newRoom.gameObject);
                //locate:
                Vector2Int position1 = vacantPlaces.ElementAt(Random.Range(0, vacantPlaces.Count));
                //newRoom.RotateRandomly();

                Vector3 pos = LocateBigRoom(newRoom, position1);
                //Debug.Log(pos);

                if (pos.x == 0 && pos.z == 0)
                {
                    Destroy(newRoom.gameObject);
                    goto spawn;
                }
                //else if (ConnectToRoom(newRoom, position1))
                //{
                newRoom.transform.position = pos;
                RoomsToPlayer[i] = newRoom;
                //}
                if (!ConnectToRoom(newRoom, position1))
                {
                    Destroy(newRoom.gameObject);
                    goto spawn;
                }
                

                break;
        }

        //Destroy(newRoom.gameObject);
    }

    private bool ConnectToRoom(Room room, Vector2Int p)
    {
        Door[] doors = room.GetComponentsInChildren<Door>();

        Door[] selectedDoors = new Door[2];
        Door[] newSelectedDoors = new Door[2];

        List<Vector2Int> neighbours = new List<Vector2Int>();

        if (p.y < maxY && spawnedRooms[p.x, p.y + 1] != null && spawnedRooms[p.x, p.y + 1] != room) neighbours.Add(Vector2Int.up);
        if (p.y > 0 && spawnedRooms[p.x, p.y - 1] != null && spawnedRooms[p.x, p.y - 1] != room) neighbours.Add(Vector2Int.down);
        if (p.x < maxX && spawnedRooms[p.x + 1, p.y] != null && spawnedRooms[p.x + 1, p.y] != room) neighbours.Add(Vector2Int.right);
        if (p.x > 0 && spawnedRooms[p.x - 1, p.y] != null && spawnedRooms[p.x - 1, p.y] != room) neighbours.Add(Vector2Int.left);

        if (neighbours.Count <= 0)
        {
            return false;
        }

        Vector2Int selectedDirection = neighbours[Random.Range(0, neighbours.Count)];

        Vector3 direction = new Vector3();

        Debug.Log(selectedDirection);

        float maxP = 0;

        foreach (var door in doors)
        {
            if (selectedDirection == Vector2Int.up)
            {
                direction = Vector3.forward;
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
                direction = -Vector3.forward;
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
                direction = Vector3.right;
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
                direction = Vector3.left;
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


        //int index = 0;
        foreach (var door in selectedDoors)
        {
            if (door != null)
            {
                Debug.Log(door.gameObject.name);
                //Ray ray = new Ray(door.transform.position, direction);
                //RaycastHit hit;
                //if (Physics.Raycast(ray, out hit))
                //{
                //    newSelectedDoors[index] = hit.collider.gameObject.GetComponent<Door>();
                //    index++;
                //    Debug.Log(hit.collider.gameObject);
                //}
                //if (index < newSelectedDoors.Length)
                //{
                //    if (Physics.OverlapSphere(door.transform.position, 0.7f).Length > 0)
                //    {
                //        newSelectedDoors[index] = Physics.OverlapSphere(door.transform.position, 0.7f)[0].gameObject.GetComponent<Door>();
                //        index++;
                //        Debug.Log(Physics.OverlapSphere(door.transform.position, 0.7f)[0].gameObject.name);
                //    }

                //}
                //Debug.Log(door.transform.position);

                

            }
        }

        int maxI = Random.Range(0, selectedDoors.Length);

        for (int i = 0; i < selectedDoors.Length; i++)
        {
            if (selectedDoors[i] != null)
            {
                selectedDoors[i].gameObject.GetComponents<BoxCollider>()[1].enabled = true;
            }
        }

        return true;
    }

    //private bool DefaultConnectToSomething(Room room, Vector2Int p)
    //{
    //    int maxX = spawnedRooms.GetLength(0) - 1;
    //    int maxY = spawnedRooms.GetLength(1) - 1;

    //    List<Vector2Int> neighbours = new List<Vector2Int>();

    //    if (room.DoorU != null && p.y < maxY && spawnedRooms[p.x, p.y + 1]?.DoorD != null) neighbours.Add(Vector2Int.up);
    //    if (room.DoorD != null && p.y > 0 && spawnedRooms[p.x, p.y - 1]?.DoorU != null) neighbours.Add(Vector2Int.down);
    //    if (room.DoorR != null && p.x < maxX && spawnedRooms[p.x + 1, p.y]?.DoorL != null) neighbours.Add(Vector2Int.right);
    //    if (room.DoorL != null && p.x > 0 && spawnedRooms[p.x - 1, p.y]?.DoorR != null) neighbours.Add(Vector2Int.left);

    //    if (neighbours.Count == 0) return false;

    //    Vector2Int selectedDirection = neighbours[Random.Range(0, neighbours.Count)];
    //    Room selectedRoom = spawnedRooms[p.x + selectedDirection.x, p.y + selectedDirection.y];

    //    if (selectedDirection == Vector2Int.up)
    //    {
    //        room.DoorU.SetActive(false);
    //        selectedRoom.DoorD.SetActive(false);
    //        room.DoorU_Active = false;
    //        selectedRoom.DoorD_Active = false;
    //    }
    //    else if (selectedDirection == Vector2Int.down)
    //    {
    //        room.DoorD.SetActive(false);
    //        selectedRoom.DoorU.SetActive(false);
    //        room.DoorD_Active = false;
    //        selectedRoom.DoorU_Active = false;
    //    }
    //    else if (selectedDirection == Vector2Int.right)
    //    {
    //        room.DoorR.SetActive(false);
    //        selectedRoom.DoorL.SetActive(false);
    //        room.DoorR_Active = false;
    //        selectedRoom.DoorL_Active = false;
    //    }
    //    else if (selectedDirection == Vector2Int.left)
    //    {
    //        room.DoorL.SetActive(false);
    //        selectedRoom.DoorR.SetActive(false);
    //        room.DoorL_Active = false;
    //        selectedRoom.DoorR_Active = false;
    //    }

    //    return true;
    //}

    private Vector3 LocateBigRoom(Room newRoom, Vector2Int pos)
    {
        float posX = 0f;
        float posY = 0f;

        if (pos.x + 1 < maxX && pos.x - 1 > 0 && pos.y + 1 < maxY && pos.y - 1 > 0)
        {
            Room UpRoom = spawnedRooms[pos.x, pos.y + 1];
            Room DownRoom = spawnedRooms[pos.x, pos.y - 1];
            Room RightRoom = spawnedRooms[pos.x + 1, pos.y];
            Room LeftRoom = spawnedRooms[pos.x - 1, pos.y];

            if (UpRoom == null && pos.x + 1 < maxX && pos.x - 1 > 0 && pos.y + 1 < maxY && pos.y - 1 > 0)
            {
                if (RightRoom == null)
                {
                    if (spawnedRooms[pos.x + 1, pos.y + 1] == null)
                    {
                        posX = ((pos.x - ((int)HeshLen / 2)) * 10f) + 5f;
                        posY = ((pos.y - ((int)HeshLen / 2)) * 10f) + 5f;

                        spawnedRooms[pos.x, pos.y] = newRoom;
                        spawnedRooms[pos.x + 1, pos.y] = newRoom;
                        spawnedRooms[pos.x, pos.y + 1] = newRoom;
                        spawnedRooms[pos.x + 1, pos.y + 1] = newRoom;
                    }
                }
                else if (LeftRoom == null)
                {
                    if (spawnedRooms[pos.x - 1, pos.y + 1] == null)
                    {
                        posX = ((pos.x - ((int)HeshLen / 2)) * 10f) - 5f;
                        posY = ((pos.y - ((int)HeshLen / 2)) * 10f) + 5f;

                        spawnedRooms[pos.x, pos.y] = newRoom;
                        spawnedRooms[pos.x - 1, pos.y] = newRoom;
                        spawnedRooms[pos.x, pos.y + 1] = newRoom;
                        spawnedRooms[pos.x - 1, pos.y + 1] = newRoom;
                    }
                }
            }
            else if (DownRoom == null && pos.x + 1 < maxX && pos.x - 1 > 0 && pos.y + 1 < maxY && pos.y - 1 > 0)
            {
                if (RightRoom == null)
                {
                    if (spawnedRooms[pos.x + 1, pos.y - 1] == null)
                    {
                        posX = ((pos.x - ((int)HeshLen / 2)) * 10f) + 5f;
                        posY = ((pos.y - ((int)HeshLen / 2)) * 10f) - 5f;

                        spawnedRooms[pos.x, pos.y] = newRoom;
                        spawnedRooms[pos.x + 1, pos.y] = newRoom;
                        spawnedRooms[pos.x, pos.y - 1] = newRoom;
                        spawnedRooms[pos.x + 1, pos.y - 1] = newRoom;
                    }
                }
                else if (LeftRoom == null)
                {
                    if (spawnedRooms[pos.x - 1, pos.y - 1] == null)
                    {
                        posX = ((pos.x - ((int)HeshLen / 2)) * 10f) - 5f;
                        posY = ((pos.y - ((int)HeshLen / 2)) * 10f) - 5f;

                        spawnedRooms[pos.x, pos.y] = newRoom;
                        spawnedRooms[pos.x - 1, pos.y] = newRoom;
                        spawnedRooms[pos.x, pos.y - 1] = newRoom;
                        spawnedRooms[pos.x - 1, pos.y - 1] = newRoom;
                    }
                }
            }
        }

        return new Vector3(posX, 0, posY);
    }
}
