using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerMovement : MonoBehaviour
{


    [Header("References")]
    public Rigidbody rBody;
    public Animator anim;
    public Image powerIndicator;
    public Image powerIndicato;
    public KickController kickController;
    public Camera cam;

    [Header("-")]
    public float initialFOV;
    public float zoomedOutFOV;

    [Header("Physics")]
    public float gravity;
    public float groundedDistance = 0.5f;
    public float jumpForce;

    [Header("Controller Options")]
    public float moveSpeed;

    [Header("Mouse Movement")]
    public LayerMask groundMask;
    public float lookSpeed;

    [Header("Testing State")]
    public bool grounded;
    public float h;
    public float v;
    public bool jump;

    float powerUp = 0;
    public float maxHoldPowerup = 2;
    public float powerupMultiplier = 2;
    float holdMultiplier = 1f;

    bool anyMouseDown = false;

    int jumpKicks = 0;

    float init = 0;
    float additive = 0;

    private void Awake()
    {
        kickController.line.enabled = false;
        init = moveSpeed;
    }

    private void Update()
    {
        if (h == 0 && v == 0 && !jump)
        {
            Vector3 v = rBody.velocity;
            v.y *= 0;
            if (v.magnitude > 0)
            {
                rBody.isKinematic = true;
                StartCoroutine(enableRBODY(Time.deltaTime));
            }
        }

       
        if (anim.GetBool("kick") == true)
        {
            anim.SetBool("kick", false);
        }

        if (anim.GetBool("superKick"))
        {
            anim.SetBool("superKick", false);
            anim.SetFloat("kickPower", 0);
        }

        if (cam.fieldOfView != initialFOV && !Input.GetKey(KeyCode.LeftShift))
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, initialFOV, 5 * Time.deltaTime);
        }
        else if (cam.fieldOfView != zoomedOutFOV && Input.GetKey(KeyCode.LeftShift))
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoomedOutFOV, 5 * Time.deltaTime);
        }

        v = Input.GetAxis("Vertical") * holdMultiplier;
        h = Input.GetAxis("Horizontal") * holdMultiplier;
        
        if (Input.GetMouseButton(1) )
        {
            if (kickController.ballInRange)
            {
                Time.fixedDeltaTime = 0.001f;
                Time.timeScale = 0.1f;

                kickController.line.enabled = true;
                kickController.InitiateKick(Mathf.Clamp((powerUp / maxHoldPowerup) * powerupMultiplier, 0, Mathf.Infinity), jump, true, transform.forward, false);


            }
            else
            {
                kickController.line.enabled = false;
                Time.fixedDeltaTime = 0.01f;
                Time.timeScale = 1f;
            }

            powerIndicator.color = Color.Lerp(powerIndicator.color, Color.yellow, (powerUp / maxHoldPowerup));
            powerIndicato.color = Color.Lerp(powerIndicator.color, Color.yellow, (powerUp / maxHoldPowerup));
            powerUp = Mathf.Clamp(powerUp+Time.deltaTime,0,maxHoldPowerup);
            anim.SetFloat("kickPower", Mathf.Clamp((powerUp / maxHoldPowerup) * powerupMultiplier, 1, Mathf.Infinity));
            v = 0;
            h = 0;

            
        }
        else
        {

            Time.fixedDeltaTime = 0.01f;
            Time.timeScale = 1f;
        }

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
            anyMouseDown = true;
        }
        else anyMouseDown = false;

        if (Input.GetMouseButtonUp(1))
        {
            kickController.line.enabled = false;
            
            if (!grounded && jumpKicks < 1 || grounded)
            {
                //kickController.InitiateKick(Mathf.Clamp((powerUp / maxHoldPowerup) * powerupMultiplier, 0, Mathf.Infinity), jump, true, transform.forward, false);

                //Trace(Color.red);
                kickController.InitiateKick(Mathf.Clamp((powerUp / maxHoldPowerup) * powerupMultiplier, 0, Mathf.Infinity), jump, true, transform.forward,true);
                //Trace(Color.red);
                anim.SetBool("superKick", true);
                v = 0;
                h = 0;
                powerUp = 0;
                if (!grounded)
                jumpKicks += 1;
                //StartCoroutine(holdCoroutine(0.001f));
            }
        }
        
     
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            powerIndicator.color = Color.red;
            powerIndicato.color = Color.red;
            moveSpeed = init + 14;
            
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (holdMultiplier != 0)
            {
                powerIndicator.color = Color.white;
                powerIndicato.color = Color.white;
            }
            moveSpeed = init;
        }


        if (Input.GetMouseButtonUp(0))
        {
            if (!grounded && jumpKicks < 1 || grounded)
            {
                //powerIndicator.color = Color.green;
                //powerIndicato.color = Color.green;
                anim.SetBool("kick", true);
                v = 0;
                h = 0;
                kickController.InitiateKick(1, jump, false, transform.forward,true);
                if (!grounded)
                    jumpKicks += 1;

                if (!jump)
                    StartCoroutine(holdCoroutine(0.05f));
            }
        }

        MouseLook();

    }

    void Trace(Color cc)
    {
        float hMult = 0f;
        float fMult = 0f;
        Vector3 offset = Vector3.zero;

        offset = transform.forward;
        offset.y = (kickController.ballTransform.position - transform.position).normalized.y;

        //Debug.DrawRay(ballTransform.position, offset, Color.red, 10);

        float power = Mathf.Clamp((powerUp / maxHoldPowerup) * powerupMultiplier, 1, Mathf.Infinity);

        hMult = 2f;
        fMult = power * 15f;
        Vector3 fce = offset * fMult * kickController.forceMultiplier + Vector3.up * hMult * kickController.heightMultiplier;

        // print("ADDED FAKE : " + fce);

        //FCE
        Vector3 prevPos = kickController.ballTransform.position + (kickController.ballRigidbody.velocity + fce) * Time.fixedDeltaTime;
        Vector3 prevVelocity = (kickController.ballRigidbody.velocity + fce);
        prevVelocity.y -= 30f;

        //print("FAKE PREV POS : " + prevPos + " /  FAKE PREV VELOCITY : " + prevVelocity);
        //  print("FAKE VELLL: " + kickController.ballRigidbody.velocity);

        for (int c = 2; c < 500; c++)
        {
            float i = Time.fixedDeltaTime * c;

            Vector3 accel = Vector3.zero;
            accel.y += ((-30 * (i * i)) / 2);

            prevVelocity.y += (-30 + 1.5f) * i;

            Vector3 curPos = prevPos + (prevVelocity) * i;

            Debug.DrawLine(prevPos, curPos, cc, 10);

            prevPos = curPos;
        }
    }

    IEnumerator enableRBODY (float seconds)
    {
        yield return new WaitForSeconds(seconds);
        rBody.isKinematic = false;
    }

    Vector3 point;
    private void MouseLook ()
    {
        if (!kickController.runAnim)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
            {

                point = hit.point;
                transform.LookAt(new Vector3(point.x, transform.position.y, point.z), Vector3.up);
            }
        }
    }


    IEnumerator holdCoroutine (float time)
    {
        holdMultiplier = 0f;
        yield return new WaitForSeconds(time);
        powerIndicator.color = Color.white;powerIndicato.color = Color.white;
        yield return new WaitForSeconds(time);
        holdMultiplier = 1f;
        
    }

    private void FixedUpdate()
    {
        ApplyGravity();

        if (v < 0) v *= 0.4F;

        if (grounded)
            ApplyMovement(h, v);
        else
            ApplyMovement(h * 0.85f, v * 0.85f);



        if (jump && grounded)
        {
            rBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        jump = false;
    }

    private void ApplyMovement (float h, float v)
    {
        
        anim.SetFloat("H", h);
        anim.SetFloat("V", v);

        Vector3 movementVector = new Vector3(h, 0,v);
        movementVector = transform.TransformDirection(movementVector);

        if (! anyMouseDown)
            kickController.Dribble(movementVector);

        rBody.MovePosition(transform.position + movementVector * moveSpeed * Time.deltaTime);

    }

    private void ApplyGravity()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, groundedDistance, groundMask))
        {
            //if (!jump)
              //  rBody.velocity = new Vector3(rBody.velocity.x, 0, rBody.velocity.y);
            grounded = true;

            jumpKicks = 0;
            anim.SetBool("grounded", true);
            anim.SetLayerWeight(1, 0);

        }
        else
        {
            
            anim.SetBool("grounded", false);
            grounded = false;
            anim.SetLayerWeight(1, 0.5f);

        }

        if (!grounded)
        rBody.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
    }

}
