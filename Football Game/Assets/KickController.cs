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

    bool dribbleLocked = false;
    bool kicked = false;


    public void InitiateKick (float power, bool isJumping, bool superKick, Vector3 relativeForward)
    {
        if (dribbleLocked && superKick)
            return;

        if (kicked)
            return;
        
        if ((ballInRange))
        {
            kicked = true;
            float multip = 1f;
            float forwardMultip = 1f;
            float hMult = 1f;
            if (superKick)
            {
                multip = 5f;
                forwardMultip = 1f;
                hMult = 1f;
            }
            else
            { multip =1f; forwardMultip = 6f; hMult = 0f;}
            Vector3 offset = Vector3.zero;
            if (superKick && controller.grounded)
            {
          
                offset = (transform.forward * forwardMultip).normalized * power * multip;

            }
            else if (superKick && !controller.grounded)
            {
                offset = transform.forward * 6 * power;
                hMult = 1.1f;
            }
            else if (dribbleLocked && !superKick)
            {
               
                offset = transform.forward * forwardMultip * 6;
                hMult = 0.9f;
            }
            else if (!controller.grounded && !superKick && Input.GetKey(KeyCode.LeftShift))
            {
                offset = transform.forward * forwardMultip * 10;
                hMult = 0.5f;
            }
            else if (!controller.grounded && !superKick)
            {
                offset = transform.forward * forwardMultip * 3;
                hMult = 0.4f;
            }
            else
            {
                offset = transform.forward * forwardMultip  * 2;
                hMult = 0.2f;
            }
         
            reLoop = 0f;
            runAnim = true;
            ballRigidbody.velocity = Vector3.zero;
            ballRigidbody.angularVelocity = Vector3.zero;
            setPosition = transform.position + (ballTransform.position - footTransform.position) * 0.7f;
            setPosition.y -= 0.5f;
            initPosition = transform.position;
           
            StartCoroutine(kick(waitTime, offset, power,hMult));
            
        }
    }

    public void Dribble (Vector3 direction)
    {
        if (Input.GetKey(KeyCode.LeftShift) && !kicked)
        {
            if (ballInRange)
            {
                if (Vector3.Distance(footTransform.position, ballTransform.position) <= minDribbleDistance && ballRigidbody.velocity.magnitude <= 45)
                {
                    dribbleLocked = true;
                    if (Input.GetAxis("Vertical") > 0 )
                    {
                        
                        ballRigidbody.AddForce(transform.forward * dribblePower, ForceMode.Impulse);
                        ballRigidbody.velocity = Vector3.ClampMagnitude(ballRigidbody.velocity, 40);
                    }

                }
                else
                {

                    if (Input.GetAxis("Vertical") > 0 && controller.grounded && Vector3.Distance(footTransform.position, ballTransform.position) <= minDribbleDistance*2)
                    {
                      
                        reLoop = 0f;
                        runAnim = true;
                        setPosition = transform.position + (ballTransform.position - footTransform.position) * 0.3f;
                        setPosition.y = transform.position.y;
                        initPosition = transform.position;
                        dribbleLocked = true;
                    }
                    else if (controller.rBody.isKinematic)
                    {
                        dribbleLocked = false; 
                        controller.rBody.isKinematic = false;
                    }
                    else
                    {
                        dribbleLocked = false;
                    }
                }
            }
            else
            {
                dribbleLocked = false;
            }
        }
        else
        {
            dribbleLocked = false;
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

    IEnumerator addForceAfterTime (float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        

    }

    IEnumerator kick(float waitTime, Vector3 offset, float power, float heightHeightMultiplier)
    {
        //yield return new WaitForSeconds(waitTime);

        while (runAnim == true) yield return null;

        ballRigidbody.AddForce(offset * forceMultiplier + Vector3.up * heightMultiplier*power*heightHeightMultiplier, ForceMode.Impulse);

        yield return new WaitForSeconds(waitTime);
        kicked = false;
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
