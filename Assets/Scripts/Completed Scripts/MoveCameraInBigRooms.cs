using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCameraInBigRooms : MonoBehaviour
{
    private Transform Player;
    public Transform Room;
    [Space]
    [Space]
    public float maxX, maxZ, minX, minZ;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        transform.position = new Vector3(Player.position.x, transform.position.y, Player.position.z - 3);

        float x = Mathf.Clamp(transform.position.x, Room.position.x - minX, Room.position.x + maxX);
        float z = Mathf.Clamp(transform.position.z, Room.position.z - minZ, Room.position.z + maxZ);

        transform.position = new Vector3(x, transform.position.y, z);
    }
}
