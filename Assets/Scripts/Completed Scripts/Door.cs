using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool DoorActive = true;
    [Space]
    [Space]
    public GameObject DoorMap;

    // Работает без игрока
    private IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(1f);
        DoorMap.SetActive(!DoorActive);
    }

    private void OnTriggerEnter(Collider other)
    {
        Door door = other.gameObject.GetComponent<Door>();
        if (door != null && door.gameObject != gameObject && gameObject.GetComponents<BoxCollider>()[1].enabled == true)
        {
            Debug.Log(other.gameObject.name);
            DoorActive = false;
            gameObject.SetActive(false);
            gameObject.GetComponents<BoxCollider>()[1].enabled = false;
            door.DoorActive = false;
            door.gameObject.SetActive(false);
            door.gameObject.GetComponents<BoxCollider>()[1].enabled = false;
        }
    }
}
