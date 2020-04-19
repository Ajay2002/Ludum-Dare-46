using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{

    public Vector3 center;
    public Vector3 size;
    public GameObject[] rockFormationPrefab;
    public GameObject particleSys;
    public int numOfFormations = 4;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numOfFormations; i++)
        {
            CreateFormation();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateFormation()
    {
        Vector3 pos = gameObject.transform.position + new Vector3(Random.Range(-size.x / 2, size.x / 2), 0.1f, Random.Range(-size.z / 2, size.z / 2));
        Instantiate(rockFormationPrefab[Random.Range(0,rockFormationPrefab.Length)], pos, Quaternion.identity).transform.parent = gameObject.transform;
        Instantiate(particleSys, pos, Quaternion.Euler(-90, 0, 0)).transform.parent = gameObject.transform;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawCube(center, size);
    }
}
