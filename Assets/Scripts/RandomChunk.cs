using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomChunk : MonoBehaviour
{
    public int chunkSize = 5;
    public GameObject prefabs;
    public GameObject[] chunkPool;

    public Transform _player;

    // Start is called before the first frame update
    void Start()
    {
        chunkPool = new GameObject[chunkSize];
        //create
        int xPos = 0;
        int yPos = 0;
        for (int i = 0; i < chunkSize; i++)
        {
            xPos++;
            yPos++;
            chunkPool[i] = Instantiate(prefabs, transform.position, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
