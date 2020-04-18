using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickController : MonoBehaviour
{
    public PlayerMovement controller;

    public Transform ballTransform;
    public Rigidbody ballRigidbody;

    public Transform footTransform;

    public bool ballInRange = false;

    public float forceMultiplier;
    public float heightMultiplier;
    public float dribblePower;

    public float waitTime;
    public float minDribbleDistance;
    Vector3 initPosition;
    public float moveSpeed;
    Vector3 setPosition;
    bool runAnim;

    public void InitiateKick (float power, bool isJumping, bool superKick, Vector3 relativeForward)
    {
        
        if ((ballInRange && Vector3.Distance(footTransform.position, ballTransform.position) >= minDribbleDistance))
        {
            float multip = 1f;
            if (superKick)
                multip = 5f;
            Vector3 offset = (ballTransform.position - footTransform.position).normalized * power * multip;

            reLoop = 0f;
            runAnim = true;
            ballRigidbody.velocity = Vector3.zero;
            ballRigidbody.angularVelocity = Vector3.zero;
            setPosition = transform.position + (ballTransform.position - footTransform.position) * 0.7f;
            initPosition = transform.position;
            StartCoroutine(kick(waitTime, offset, power));
            
        }
    }

    public void Dribble (Vector3 direction)
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (ballInRange)
            {
                if (Vector3.Distance(footTransform.position, ballTransform.position) <= minDribbleDistance)
                {

                    ballRigidbody.AddForce(direction * dribblePower, ForceMode.Impulse);
                }
            }
        }
    }

    //Running Anim.
    float reLoop = 0f;
    private void Update()
    {
        if (runAnim == true)
        {
            if (Vector3.Distance(transform.position, setPosition) >= 0.05f)
            {
                controller.rBody.isKinematic = true;
                transform.position = (Vector3.Lerp(initPosition, setPosition, reLoop));
                reLoop += moveSpeed * Time.deltaTime;
            }
            else
            {
                controller.rBody.isKinematic = false;
                runAnim = false;
            }
        }
    }

    IEnumerator kick(float waitTime, Vector3 offset, float power)
    {
        //yield return new WaitForSeconds(waitTime);

        while (runAnim == true) yield return null;

        ballRigidbody.AddForce(offset * forceMultiplier + Vector3.up * heightMultiplier*power, ForceMode.Impulse);
    }

    IEnumerator forceAfterDelay (float waitTime, Vector3 force)
    {
        yield return new WaitForSeconds(waitTime);
        ballRigidbody.AddForce(force, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Football")
        {
            ballInRange = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Football")
        {
            ballInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Football")
        {
            ballInRange = false;
        }
    }

}
