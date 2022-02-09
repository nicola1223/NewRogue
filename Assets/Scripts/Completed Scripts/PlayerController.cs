using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10; // �������� ���������
    public MoveMap MapCamera; // ������ ������ ��� �����
    public LevelBuilder lb; // ��� ��������� ������ ���� ������

    private Rigidbody rb;
    private CompletedRoom[] rooms;
    private Vector3 lastMovement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(getRooms());
    }

    private void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal"); // �������� ���� �� �����������
        float v = Input.GetAxis("Vertical"); // �������� ���� �� ���������
        Vector3 movement = new Vector3(h, 0f, v);
        if (lastMovement != movement)
        {
            lastMovement = movement.normalized;
        }
        rb.velocity = movement * speed; // ����������� �������� RigidBody
    }

    private void OnTriggerEnter(Collider other) // ��� ����� � ������� �� �������� �������
    {
        Vector3 pos = other.transform.position; // �������� ������� �������
        CompletedRoom curRoom = other.gameObject.GetComponentInParent<CompletedRoom>(); // �������� ������� ��������
        if (curRoom) 
        {
            curRoom.SetActive(true); // ���������� ������� �������
            curRoom.unclocked = true; // ��������� �������
            foreach (var room in rooms)
            {
                if (room != curRoom)
                {
                    room.SetActive(false); // ������������ ���������
                }
            }
        }
        MapCamera.target = new Vector3(pos.x, MapCamera.transform.position.y, pos.z); // ����������� ������ ��� �����
        transform.position += lastMovement / 2; // ���� ���� ����������� ���������,����� ��� �� ������ ������ ���� ��� ���������
    }

    private IEnumerator getRooms()
    {
        yield return new WaitForSecondsRealtime(1f); // ����� ������� ����� ������ �������� �������
        rooms = lb.RoomsToPlayer;
    }
}
