using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public LayerMask groundMask;
    public GameObject obj;
    public float riseUp = 0.2f;
    public float maxHit = 50;
    public Vector3 initialScale;

    public int bounceCount = 0;

    private void Update()
    {


        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity,groundMask))
        {
           
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
            }
            obj.transform.position = new Vector3(hit.point.x,hit.point.y+riseUp,hit.point.z);
            obj.transform.localScale = Mathf.Clamp01((hit.distance / maxHit)) * initialScale;

        }
        else
        {
            obj.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "GroundTag")
        {
            bounceCount += 1;
        }

        if (collision.transform.tag == "MiddleBlocker")
        {

            bounceCount = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "MiddleBlocker")
        {
            bounceCount = 0;
        }
    }

}
