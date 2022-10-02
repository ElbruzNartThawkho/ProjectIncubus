using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MainCharacter : MonoBehaviour
{
    public GameObject voicePref;
    bool canVoicecCaution = true;

    public float mouseSens = 1;

    GameObject lookPoint, rightPoint;
    CharacterController characterController;
    Animator animator;
    Vector3 frontMove, rightMove, gravity, poseCrouch = new Vector3(0, 0.66f, 0.06f), poseStand = new Vector3(0, 1, 0.06f);
    float yatay, dikey, donus, speed = 1;
    bool stand = true, run = false;
    Rigidbody[] ragdollRb; Collider[] ragdollColl; bool rdState = false;
    private void Awake()
    {
        rightMove.y = 0;
        frontMove.y = 0;
        gravity.x = 0; gravity.y = 0; gravity.z = 0;
        characterController = GetComponent<CharacterController>();
        lookPoint = GameObject.FindWithTag("LookPoint");
        rightPoint = GameObject.FindWithTag("RightPoint");
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

    void Movement()
    {
        if (rdState == false)
        {
            yatay = Input.GetAxis("Horizontal"); dikey = Input.GetAxis("Vertical");

            animator.SetFloat("Yatay", yatay);
            animator.SetFloat("Dikey", dikey);

            frontMove.x = -1 * (gameObject.transform.position.x - lookPoint.transform.position.x);
            frontMove.z = -1 * (gameObject.transform.position.z - lookPoint.transform.position.z);

            rightMove.x = -1 * (gameObject.transform.position.x - rightPoint.transform.position.x);
            rightMove.z = -1 * (gameObject.transform.position.z - rightPoint.transform.position.z);

            if (characterController.isGrounded && gravity.y <= 0)
            {
                characterController.Move(frontMove * dikey * Time.deltaTime * speed);
                characterController.Move(rightMove * yatay * Time.deltaTime * speed);
                animator.SetBool("Grounded", true);
                gravity.y = 0f;
            }
            else if (characterController.isGrounded == false)
            {
                //Debug.Log("de�miyor");
                gravity.y -= 1;
                characterController.Move(gravity * Time.deltaTime);
            }
            donus = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up * donus);
        }
        //if (characterController.collisionFlags == CollisionFlags.None)
        //{
        //    //Havada s�z�l�yor, temas yok!
        //}

        //if ((characterController.collisionFlags & CollisionFlags.Sides) != 0)
        //{
        //    //Etraf�ndan bir �eylere temas ediyor"
        //}

        //if (characterController.collisionFlags == CollisionFlags.Sides)
        //{
        //    //Yaln�zca yanlardan temas var,ba�ka bir temas yok"
        //}

        //if ((characterController.collisionFlags & CollisionFlags.Above) != 0)
        //{
        //    //Tavana de�iyor"
        //}

        //if (characterController.collisionFlags == CollisionFlags.Above)
        //{
        //    //Sadece yukard�an temas,ba�ka bir temas yok
        //}

        //if ((characterController.collisionFlags & CollisionFlags.Below) != 0)
        //{
        //    //Yere de�iyor
        //}

        //if (characterController.collisionFlags == CollisionFlags.Below)
        //{
        //    //Yaln�zca yere de�iyor, ba�ka bir temas yok
        //}
    }

    public void RagdollState(bool state)
    {
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
}
