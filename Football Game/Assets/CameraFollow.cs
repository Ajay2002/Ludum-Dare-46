using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float smoothFollowSpeed;

    private Vector3 initialOffset;
    public Transform player;
    public Transform ball;
    public float speed;

    public float weight;
    public float ballWeight;
    private void Start()
    {
        initialOffset = transform.position - player.position;
    }

    private void Update()
    {
        var targetRotation = Quaternion.LookRotation(((player.position*weight + ball.position*ballWeight) / 2) - transform.position);

        // Smoothly rotate towards the target point.

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
        //transform.LookAt();
    }
    private void LateUpdate()
    {
        Vector3 off = (player.position + initialOffset);


       

        transform.position = off;
    }

}
