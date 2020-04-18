using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float smoothFollowSpeed;

    private Vector3 initialOffset;
    public Transform player;
    private void Start()
    {
        initialOffset = transform.position - player.position;
    }

    private void LateUpdate()
    {
        Vector3 off = (player.position + initialOffset);
      
        transform.position = off;
    }

}
