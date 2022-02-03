using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkPlacer : MonoBehaviour
{
    public Transform Player;
    public Chunk[] ChunkPrefabs;
    public Chunk FirstChunk;

    public List<Chunk> spawned = new List<Chunk>();

    void Start()
    {
        spawned.Add(FirstChunk);
    }

    void Update()
    {
        if (Player.position.z > spawned[spawned.Count - 1].transform.Find("End").position.z - 1)
        {
            SpawnChunk();
            //    //Chunk last = spawned[spawned.Count - 1];
            //    //Transform End = spawned[spawned.Count - 1].transform.Find("End");
            //    //spawned.Add(Instantiate(ChunkPrefabs[Random.Range(0, ChunkPrefabs.Length)], End));

            //    //Chunk Chunk = Instantiate(ChunkPrefabs[Random.Range(0, ChunkPrefabs.Length)]);
            //    //Transform End = spawned[spawned.Count - 1].transform.Find("End");
            //    //Transform Begin = Chunk.transform.Find("Begin");
            //    //Chunk.transform.position = End.position - Begin.localPosition;
            //    //spawned.Add(Chunk);
        }
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    SpawnChunk();
        //}
    }

    private void SpawnChunk()
    {
        Chunk chunk = Instantiate(ChunkPrefabs[Random.Range(0, ChunkPrefabs.Length)]);
        Debug.Log(spawned[spawned.Count - 1].gameObject.transform.Find("End").transform.position);
        Debug.Log(chunk.gameObject.transform.Find("Start").gameObject.transform.localPosition);
        chunk.gameObject.transform.position = spawned[spawned.Count - 1].gameObject.transform.Find("End").transform.position - chunk.gameObject.transform.Find("Start").gameObject.transform.localPosition;
        //Debug.Log(chunk.gameObject.transform.position);
        //chunk.position = spawned[spawned.Count - 1].Find("End").transform.position - chunk.Find("Begin").transform.localPosition;
        spawned.Add(chunk);

        if (spawned.Count >= 3)
        {
            Destroy(spawned[0].gameObject);
            spawned.RemoveAt(0);
        }
    }
}
