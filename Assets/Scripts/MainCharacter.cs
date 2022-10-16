using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MainCharacter : MonoBehaviour
{
    public GameObject voicePref;
    bool canVoicecCaution = true;

    public float mouseSens = 1;

    CharacterController characterController;
    Animator animator;
    Vector3 gravity, poseCrouch = new Vector3(0, 0.66f, 0.06f), poseStand = new Vector3(0, 1, 0.06f);
    float yatay, dikey, speed = 1;
    bool stand = true, run = false, isLive = true;
    Rigidbody[] ragdollRb; Collider[] ragdollColl; bool rdState = false;
    RaycastHit hit;
    Ray ray;
    Vector3 lookedAtPoint;

    private void Awake()
    {
        lookedAtPoint.y = transform.position.y;
        gravity.x = 0; gravity.y = 0; gravity.z = 0;
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        ragdollRb = GetComponentsInChildren<Rigidbody>();
        ragdollColl = GetComponentsInChildren<Collider>();
        RagdollState(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            VoiceCaution();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RagdollState(true);
            //Jump();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Pose();
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Run();
        }
        else
        {
            Walk();
        }
        if (isLive)
        {
            MouseLook();
        }
        Movement();
    }

    void Pose()
    {
        if (rdState == false)
        {
            if (stand == true)
            {
                stand = false;
                characterController.height = 1.2f;
                characterController.center = poseCrouch;
            }
            else
            {
                stand = true;
                characterController.height = 1.8f;
                characterController.center = poseStand;
            }
            animator.SetBool("Stand", stand);
        }
    }

    void Run()
    {
        if (rdState == false)
        {
            if (run == false)
            {
                run = true;
                speed = 2;
            }
            animator.SetBool("Run", run);
        }
    }

    //void Jump()
    //{
    //    if (characterController.isGrounded)
    //    {
    //        gravity.y = 30;
    //        animator.SetBool("Grounded", false);
    //    }
    //}

    void Walk()
    {
        if (rdState == false)
        {
            if (run == true)
            {
                run = false;
                speed = 1;
            }
            animator.SetBool("Run", run);
        }
    }

    void JoyStick()
    {
        //joystick ayarlarý yapýlacak
    }
    void MouseLook()
    {
        ray = GameObject.FindWithTag("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.point.x - transform.position.x > 0.5f || hit.point.z - transform.position.z > 0.5f || hit.point.x - transform.position.x < -0.5f || hit.point.z - transform.position.z < -0.5f)
            {
                lookedAtPoint.x = hit.point.x; lookedAtPoint.z = hit.point.z;
                transform.LookAt(lookedAtPoint);
            }
        }
    }

    void Movement()
    {
        if (rdState == false)
        {
            yatay = Input.GetAxis("Horizontal"); dikey = Input.GetAxis("Vertical");

            UpdateAnimator();

            if (characterController.isGrounded && gravity.y <= 0)
            {
                characterController.Move(4f * Vector3.forward * dikey * Time.deltaTime * speed);
                characterController.Move(4f * Vector3.right * yatay * Time.deltaTime * speed);
                animator.SetBool("Grounded", true);
                gravity.y = 0f;
            }
            else if (characterController.isGrounded == false)
            {
                //Debug.Log("deðmiyor");
                gravity.y -= 1;
                characterController.Move(gravity * Time.deltaTime);
            }
            //donus = Input.GetAxis("Mouse X");

        }
        //if (characterController.collisionFlags == CollisionFlags.None)
        //{
        //    //Havada süzülüyor, temas yok!
        //}

        //if ((characterController.collisionFlags & CollisionFlags.Sides) != 0)
        //{
        //    //Etrafýndan bir þeylere temas ediyor"
        //}

        //if (characterController.collisionFlags == CollisionFlags.Sides)
        //{
        //    //Yalnýzca yanlardan temas var,baþka bir temas yok"
        //}

        //if ((characterController.collisionFlags & CollisionFlags.Above) != 0)
        //{
        //    //Tavana deðiyor"
        //}

        //if (characterController.collisionFlags == CollisionFlags.Above)
        //{
        //    //Sadece yukardýan temas,baþka bir temas yok
        //}

        //if ((characterController.collisionFlags & CollisionFlags.Below) != 0)
        //{
        //    //Yere deðiyor
        //}

        //if (characterController.collisionFlags == CollisionFlags.Below)
        //{
        //    //Yalnýzca yere deðiyor, baþka bir temas yok
        //}
    }

    public void RagdollState(bool state)
    {
        isLive = !state;
        foreach (Rigidbody rb in ragdollRb)
        {
            rb.isKinematic = !state;
        }
        foreach (Collider coll in ragdollColl)
        {
            coll.enabled = state;
        }
        animator.enabled = !state;
        characterController.enabled = !state;
        rdState = state;
    }

    public void VoiceCaution()
    {
        if (canVoicecCaution)
        {
            StartCoroutine(Trigger());
        }
    }

    IEnumerator Trigger()
    {
        canVoicecCaution = false;// Debug.Log(canVoicecCaution);
        Instantiate(voicePref, gameObject.transform.position, transform.rotation);
        yield return new WaitForSeconds(3);
        canVoicecCaution = true;// Debug.Log(canVoicecCaution);
    }

    private void UpdateAnimator()
    {
        Vector3 axisVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        float forwardBackwardsMagnitude = 0;//ileri geri
        float rightLeftMagnitude = 0;// sað sol
        if (axisVector.magnitude > 0)//girdi varsa
        {
            Vector3 normalizedLookingAt = lookedAtPoint - transform.position;
            normalizedLookingAt.Normalize();
            forwardBackwardsMagnitude = Mathf.Clamp(Vector3.Dot(axisVector, normalizedLookingAt), -1, 1);//Ýki vektörün nokta çarpýmý
            Vector3 perpendicularLookingAt = new Vector3(normalizedLookingAt.z, 0, -normalizedLookingAt.x);// öne bakýþ
            rightLeftMagnitude = Mathf.Clamp(Vector3.Dot(axisVector, perpendicularLookingAt), -1, 1);
        }
        animator.SetFloat("Dikey", forwardBackwardsMagnitude);
        animator.SetFloat("Yatay", rightLeftMagnitude);
    }
}
