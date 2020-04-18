using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("References")]
    public Rigidbody rBody;

    [Header("Physics")]
    public float gravity;
    public float groundedDistance = 0.5f;
    public float jumpForce;

    [Header("Controller Options")]
    public float moveSpeed;

    [Header("Testing State")]
    public bool grounded;
    public float h;
    public float v;
    public bool jump;

    private void Update()
    {
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }
    }

    private void FixedUpdate()
    {
        ApplyGravity();

        if (grounded)
            ApplyMovement(h, v);
        else
            ApplyMovement(h * 0.85f, v * 0.85f);

        if (jump)
        {
            rBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        jump = false;
    }

    private void ApplyMovement (float h, float v)
    {
        Vector3 movementVector = new Vector3(h, 0,v);
        movementVector = transform.TransformDirection(movementVector);

        rBody.MovePosition(transform.position + movementVector * moveSpeed * Time.deltaTime);

    }

    private void ApplyGravity()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, groundedDistance))
        {
            if (!jump)
                rBody.velocity = new Vector3(rBody.velocity.x, 0, rBody.velocity.y);
            grounded = true;
        }
        else
        {
            grounded = false;
        }

        if (!grounded)
        rBody.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
    }

}
