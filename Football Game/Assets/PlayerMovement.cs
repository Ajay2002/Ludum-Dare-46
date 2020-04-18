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

        v = Input.GetAxis("Vertical") * holdMultiplier;
        h = Input.GetAxis("Horizontal") * holdMultiplier;
        
        if (Input.GetMouseButton(1) )
        {
            powerIndicator.color = Color.Lerp(powerIndicator.color, Color.yellow, (powerUp / maxHoldPowerup));
            powerIndicato.color = Color.Lerp(powerIndicator.color, Color.yellow, (powerUp / maxHoldPowerup));
            powerUp = Mathf.Clamp(powerUp+Time.deltaTime,0,maxHoldPowerup);
            anim.SetFloat("kickPower", Mathf.Clamp((powerUp / maxHoldPowerup) * powerupMultiplier, 1, Mathf.Infinity));
            v = 0;
            h = 0;
        }

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
            anyMouseDown = true;
        }
        else anyMouseDown = false;

        if (Input.GetMouseButtonUp(1))
        {

            if (!grounded && jumpKicks < 1 || grounded)
            {
                kickController.InitiateKick(Mathf.Clamp((powerUp / maxHoldPowerup) * powerupMultiplier, 1, Mathf.Infinity), jump, true, transform.forward);

                anim.SetBool("superKick", true);
                v = 0;
                h = 0;
                powerUp = 0;
                if (!grounded)
                jumpKicks += 1;
                StartCoroutine(holdCoroutine(0.4f));
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
                powerIndicator.color = Color.green;
                powerIndicato.color = Color.green;
                anim.SetBool("kick", true);
                v = 0;
                h = 0;
                kickController.InitiateKick(1, jump, false, transform.forward);
                if (!grounded)
                    jumpKicks += 1;

                if (!jump)
                    StartCoroutine(holdCoroutine(0.05f));
            }
        }

        MouseLook();

    }

    IEnumerator enableRBODY (float seconds)
    {
        yield return new WaitForSeconds(seconds);
        rBody.isKinematic = false;
    }

    Vector3 point;
    private void MouseLook ()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
        {

            point = hit.point;
            transform.LookAt(new Vector3(point.x, transform.position.y, point.z), Vector3.up);
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
