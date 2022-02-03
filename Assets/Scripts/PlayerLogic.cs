using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    public float speed = 10;
    public Room[] rooms;
    public RoomPlacer rp;
    public Transform MapCamera;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(getRooms());
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(h, 0f, v);
        rb.velocity = movement * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 size = other.transform.localScale;
        Vector3 pos = other.transform.position;
        //Debug.Log(other.gameObject.GetComponentInParent<Room>());
        Vector3 dir = new Vector3(pos.x - transform.position.x, 0f, pos.z - transform.position.z).normalized;
        //Debug.Log(pos);
        //Debug.Log(dir);
        Room curRoom = other.gameObject.GetComponentInParent<Room>();
        if (curRoom)
        {
            //curRoom.SetActive(true);
            //foreach (Room room in rooms)
            //{
            //    if (room != curRoom)
            //    {
            //        room.SetActive(false);
            //    }
            //}
        }
        MapCamera.GetComponent<MoveMap>().target = new Vector3(pos.x, MapCamera.position.y, pos.z);
        transform.position += dir / 2;
    }

    IEnumerator getRooms()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        rooms = rp.RoomsToPlayer;
    }
}
