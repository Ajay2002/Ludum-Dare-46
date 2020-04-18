using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGround : MonoBehaviour
{
    GameObject[] grounds;
    float speed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        // Get the grounds
        grounds = GameObject.FindGameObjectsWithTag("Ground");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = new Vector3(Random.Range(0f, 2f), 0, Random.Range(0f, 2f));
        
        Vector3 posSmooth = Vector3.Lerp(transform.position, pos, 0.5f);
        for (int i = 0; i < grounds.Length; i++)
        {
            grounds[i].transform.position += posSmooth * Time.deltaTime * speed;
        }
    }
}
