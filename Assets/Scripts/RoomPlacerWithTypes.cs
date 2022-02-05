using System.Collections;
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

    private void Start()
    {
        spawnedRooms = new Room[HeshLen, HeshLen];
        spawnedRooms[(int)HeshLen/2, (int)HeshLen / 2] = StartingRoom;
        RoomsToPlayer = new Room[RoomsCount];
        RoomsToPlayer[RoomsCount - 1] = StartingRoom; 

        for (int i = 0; i < RoomsCount - 1; i++)
        {
            PlaceRooms(i);
        }
    }

    private void PlaceRooms(int i)
    {
        HashSet<Vector2Int> vacantPlaces = new HashSet<Vector2Int>();
        for (int x = 0; x < spawnedRooms.GetLength(0); x++)
        {
            for (int y = 0; y < spawnedRooms.GetLength(1); y++)
            {
                if (spawnedRooms[x, y] == null) continue;

                int maxX = spawnedRooms.GetLength(0) - 1;
                int maxY = spawnedRooms.GetLength(1) - 1;

                if (x > 0 && spawnedRooms[x - 1, y] == null) vacantPlaces.Add(new Vector2Int(x - 1, y));
                if (y > 0 && spawnedRooms[x, y - 1] == null) vacantPlaces.Add(new Vector2Int(x, y - 1));
                if (x < maxX && spawnedRooms[x + 1, y] == null) vacantPlaces.Add(new Vector2Int(x + 1, y));
                if (y < maxY && spawnedRooms[x, y + 1] == null) vacantPlaces.Add(new Vector2Int(x, y + 1));
            }
        }

        Room newRoom = Instantiate(RoomPrefabs[Random.Range(0, RoomPrefabs.Length)]);
        //newRoom.RotateRandomly();

        int limit = 500;
        while (limit-- > 0)
        {
            switch (newRoom.Type)
            {
                case Room.RoomType.Default:
                    // Эту строчку можно заменить на выбор положения комнаты с учётом того насколько он далеко/близко от центра,
                    // или сколько у него соседей, чтобы генерировать более плотные, или наоборот, растянутые данжи
                    Vector2Int position = vacantPlaces.ElementAt(Random.Range(0, vacantPlaces.Count));
                    newRoom.RotateRandomly();

                    if (DefaultConnectToSomething(newRoom, position))
                    {
                        float posX = (position.x - ((int)HeshLen / 2)) * (newRoom.size.x - 0.5f);
                        float posY = (position.y - ((int)HeshLen / 2)) * (newRoom.size.y - 0.5f);
                        //Vector3 pos = TryToLocate(position, newRoom);

                        //if (pos.x != 100 || pos.y != 100)
                        //{
                        newRoom.transform.position = new Vector3(posX, 0, posY);
                        //}
                        //else break;

                        //newRoom.transform.position = new Vector3(posX, 0, posY);
                        spawnedRooms[position.x, position.y] = newRoom;
                        RoomsToPlayer[i] = newRoom;
                        return;
                    }
                    break;
                case Room.RoomType.BigRoom:
                    //Destroy(newRoom.gameObject);

                    break;
            }
        }

        Destroy(newRoom.gameObject);
    }

    private bool DefaultConnectToSomething(Room room, Vector2Int p)
    {
        int maxX = spawnedRooms.GetLength(0) - 1;
        int maxY = spawnedRooms.GetLength(1) - 1;

        List<Vector2Int> neighbours = new List<Vector2Int>();

        if (room.DoorU != null && p.y < maxY && spawnedRooms[p.x, p.y + 1]?.DoorD != null) neighbours.Add(Vector2Int.up);
        if (room.DoorD != null && p.y > 0 && spawnedRooms[p.x, p.y - 1]?.DoorU != null) neighbours.Add(Vector2Int.down);
        if (room.DoorR != null && p.x < maxX && spawnedRooms[p.x + 1, p.y]?.DoorL != null) neighbours.Add(Vector2Int.right);
        if (room.DoorL != null && p.x > 0 && spawnedRooms[p.x - 1, p.y]?.DoorR != null) neighbours.Add(Vector2Int.left);

        if (neighbours.Count == 0) return false;

        Vector2Int selectedDirection = neighbours[Random.Range(0, neighbours.Count)];
        Room selectedRoom = spawnedRooms[p.x + selectedDirection.x, p.y + selectedDirection.y];

        if (selectedDirection == Vector2Int.up)
        {
            room.DoorU.SetActive(false);
            selectedRoom.DoorD.SetActive(false);
            room.DoorU_Active = false;
            selectedRoom.DoorD_Active = false;
        }
        else if (selectedDirection == Vector2Int.down)
        {
            room.DoorD.SetActive(false);
            selectedRoom.DoorU.SetActive(false);
            room.DoorD_Active = false;
            selectedRoom.DoorU_Active = false;
        }
        else if (selectedDirection == Vector2Int.right)
        {
            room.DoorR.SetActive(false);
            selectedRoom.DoorL.SetActive(false);
            room.DoorR_Active = false;
            selectedRoom.DoorL_Active = false;
        }
        else if (selectedDirection == Vector2Int.left)
        {
            room.DoorL.SetActive(false);
            selectedRoom.DoorR.SetActive(false);
            room.DoorL_Active = false;
            selectedRoom.DoorR_Active = false;
        }

        return true;
    }

    private Vector3 TryToLocate(Vector2Int pos, Room newRoom)
    {
        float posX = 0f, posY = 0f;

        Room UpRoom = spawnedRooms[pos.x, pos.y + 1];
        Room DownRoom = spawnedRooms[pos.x, pos.y - 1];
        Room RightRoom = spawnedRooms[pos.x + 1, pos.y];
        Room LeftRoom = spawnedRooms[pos.x - 1, pos.y];

        if (UpRoom && DownRoom)
        {
            float posU;
            posY = UpRoom.transform.position.z - newRoom.size.y / 2 - UpRoom.size.y / 2;
            posU = DownRoom.transform.position.z + newRoom.size.y / 2 + DownRoom.size.y / 2;
            if (posY != posU)
            {
                posY = 100f;
            }
        }
        else if (UpRoom)
        {
            posY = UpRoom.transform.position.z - newRoom.size.y / 2 - UpRoom.size.y / 2;
        }
        else if (DownRoom)
        {
            posY = DownRoom.transform.position.z + newRoom.size.y / 2 + DownRoom.size.y / 2;
        }
        else
        {
            posY = (pos.y - ((int)HeshLen / 2)) * (newRoom.size.y - 0.5f);
        }
        if (RightRoom && LeftRoom)
        {
            float posR;
            posX = RightRoom.transform.position.x - newRoom.size.x / 2 - RightRoom.size.x / 2;
            posR = LeftRoom.transform.position.x + newRoom.size.x / 2 + LeftRoom.size.x / 2;
            if (posX != posR)
            {
                posX = 100f;
            }
        }
        else if (RightRoom)
        {
            posX = RightRoom.transform.position.x - newRoom.size.x / 2 - RightRoom.size.x / 2;
        }
        else if (LeftRoom)
        {
            posX = LeftRoom.transform.position.x + newRoom.size.x / 2 + LeftRoom.size.x / 2;
        }
        else
        {
            posX = (pos.x - ((int)HeshLen / 2)) * (newRoom.size.x - 0.5f);
        }

        return new Vector3(posX, 0, posY);
    }

    //private Vector3 PosToRoom(HashSet<Vector2Int> Places)
    //{
    //    Vector2Int position = Places.ElementAt(Random.Range(0, Places.Count));
    //    Vector3 pos;

    //    return pos;
    //}
}
