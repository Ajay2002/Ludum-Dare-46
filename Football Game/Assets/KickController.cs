using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickController : MonoBehaviour
{
    public float subtract = 0f;
    public PlayerMovement controller;

    public LineRenderer line;

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
    public bool runAnim;

    bool dribbleLocked = false;
    bool kicked = false;


    public void InitiateKick(float power, bool isJumping, bool superKick, Vector3 relativeForward, bool realKick)
    {
        if (ballInRange)
        {
            Vector3 offset = transform.forward;
            float hMult = 0f;
            float fMult = 0f;

            if (!superKick)
            {
                
                //Juggling 
                hMult = 5f;
                fMult = 0.5f;
            }

            if (superKick)
            {
                offset = transform.forward;
                offset.y = (ballTransform.position - transform.position).normalized.y;

                //Debug.DrawRay(ballTransform.position, offset, Color.red, 10);

                hMult = 2f;
                fMult = power * 15f;
                Vector3 fce = offset * fMult * forceMultiplier + Vector3.up * hMult * heightMultiplier;
            }

            if (realKick)
            {
                kicked = true;
                reLoop = 0f;
                runAnim = true;

                setPosition = transform.position + (ballTransform.position - footTransform.position) * 0.6f;
                setPosition.y -= 0.5f;
                initPosition = transform.position;
                ballRigidbody.velocity = Vector3.zero;
                ballRigidbody.angularVelocity = Vector3.zero;
            }

            
            Vector3 dir = offset * fMult * forceMultiplier + Vector3.up * hMult * heightMultiplier;
            if (!realKick && superKick)
            {
                
                



                // print("GUESS: " + (ballRigidbody.velocity + dir));
                //  print("REAL VELL : " + ballRigidbody.velocity);
                //  print("ADDED BY : " + dir);
                //  print(Time.fixedDeltaTime);
                Vector3 prevPos = ballTransform.position + (Vector3.zero + dir) * Time.fixedDeltaTime;
                Vector3 prevVelocity = (Vector3.zero + dir);

                List<Vector3> poss = new List<Vector3>();
                //poss.Add(prevPos);

                for (int c = 2; c < 200; c++)
                {
                    float i = Time.fixedDeltaTime * c;

                    Vector3 accel = Vector3.zero;
                    accel.y += ((-30 * (i * i)) / 2);

                    prevVelocity.y += (-30 + subtract) * i;

                    Vector3 curPos = prevPos + (prevVelocity) * i;

                    poss.Add(curPos);

                    prevPos = curPos;
                }

                line.positionCount = (poss.Count);
                line.SetPositions(poss.ToArray());

            }

            if (realKick)
            {
                StartCoroutine(kickForce(waitTime, offset * fMult * forceMultiplier + Vector3.up * hMult * heightMultiplier));

                //ballRigidbody.AddForce(dir, ForceMode.Impulse);
                //kicked = false;
            }
            
        }
    }

    public void Dribble(Vector3 direction)
    {

    }

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

                Vector3 pos = ballTransform.position;
                pos.y = transform.position.y;

                var targetRotation = Quaternion.LookRotation(pos - transform.position);

                // Smoothly rotate towards the target point.

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15 * Time.deltaTime);
            }
            else
            {
                controller.rBody.isKinematic = false;
                runAnim = false;
            }
        }
    }

  
    IEnumerator kick(float waitTime, Vector3 offset, float power, float heightHeightMultiplier)
    {
        //yield return new WaitForSeconds(waitTime);

        while (runAnim == true) yield return null;

        
        ballRigidbody.AddForce(offset * forceMultiplier + Vector3.up * heightMultiplier * power * heightHeightMultiplier, ForceMode.Impulse);

        yield return new WaitForSeconds(waitTime);
        kicked = false;
    }

    IEnumerator kickForce (float waitTime, Vector3 dir)
    {
        
        while (runAnim == true) yield return null;
        ballRigidbody.AddForce(dir, ForceMode.Impulse);

        yield return new WaitForSeconds(waitTime);
        kicked = false;
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
