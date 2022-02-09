using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10; // Скорость персонажа
    public MoveMap MapCamera; // Скрипт камеры для карты
    public LevelBuilder lb; // Для получения списка всех комнат

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
        float h = Input.GetAxis("Horizontal"); // Получаем ввод по горизонтали
        float v = Input.GetAxis("Vertical"); // Получаем ввод по вертикали
        Vector3 movement = new Vector3(h, 0f, v);
        if (lastMovement != movement)
        {
            lastMovement = movement.normalized;
        }
        rb.velocity = movement * speed; // Присваиваем скорость RigidBody
    }

    private void OnTriggerEnter(Collider other) // При входе в комнату мы задеваем триггер
    {
        Vector3 pos = other.transform.position; // Получаем позицию комнаты
        CompletedRoom curRoom = other.gameObject.GetComponentInParent<CompletedRoom>(); // Получаем комнату триггера
        if (curRoom) 
        {
            curRoom.SetActive(true); // Активируем текущую комнату
            curRoom.unclocked = true; // Открываем комнату
            foreach (var room in rooms)
            {
                if (room != curRoom)
                {
                    room.SetActive(false); // Деактивируем остальные
                }
            }
        }
        MapCamera.target = new Vector3(pos.x, MapCamera.transform.position.y, pos.z); // Передвигаем камеру для карты
        transform.position += lastMovement / 2; // Чуть чуть пододвигаем персонажа,чтобы его не зажало дверью если она закроется
    }

    private IEnumerator getRooms()
    {
        yield return new WaitForSecondsRealtime(1f); // Через секунду после старта получаем комнаты
        rooms = lb.RoomsToPlayer;
    }
}
