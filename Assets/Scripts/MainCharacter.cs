using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MainCharacter : MonoBehaviour
{
    public GameObject stoneThrowingPoint, throwableStone, acidTrap, playerHead;
    bool canVoicecCaution = true;

    [SerializeField] int voiceCooldown = 8;

    CharacterController characterController;
    Animator animator;
    Vector3 gravity, poseCrouch = new Vector3(0, 0.66f, 0.06f), poseStand = new Vector3(0, 1, 0.06f);
    float yatay, dikey, speed = 1;
    bool stand = true, run = false, isLive = true;
    Rigidbody[] ragdollRb; Collider[] ragdollColl; bool rdState = false;
    RaycastHit hit;
    Ray ray;
    Vector3 looked2Point;


    private void Awake()
    {
        looked2Point.y = transform.position.y;
        gravity.x = 0; gravity.y = 0; gravity.z = 0;
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        ragdollRb = GetComponentsInChildren<Rigidbody>();
        ragdollColl = GetComponentsInChildren<Collider>();
        RagdollState(false);
        playerHead.GetComponent<SphereCollider>().enabled= true;
        GetComponent<BoxCollider>().enabled = true;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Trapping();
        }
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
        //joystick ayarlar� yap�lacak
    }
    void MouseLook()
    {
        ray = GameObject.FindWithTag("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.point.x - transform.position.x > 0.5f || hit.point.z - transform.position.z > 0.5f || hit.point.x - transform.position.x < -0.5f || hit.point.z - transform.position.z < -0.5f)
            {
                looked2Point.x = hit.point.x; looked2Point.z = hit.point.z;

                //Quaternion lookOnLook = Quaternion.LookRotation(looked2Point - transform.position); transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 5f);
                transform.LookAt(looked2Point);
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
                //Debug.Log("de�miyor");
                gravity.y -= 1;
                characterController.Move(gravity * Time.deltaTime);
            }
            //donus = Input.GetAxis("Mouse X");

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
        isLive = !state;
        foreach (Rigidbody rb in ragdollRb)
        {
            rb.isKinematic = !state;
        }
        foreach (Collider coll in ragdollColl)
        {
            coll.enabled = state;
        }
        GetComponent<BoxCollider>().enabled = false;
        playerHead.tag = "Untagged";
        animator.enabled = !state;
        characterController.enabled = !state;
        rdState = state;
    }

    public void Trapping()
    {
        Instantiate(acidTrap, transform.position, acidTrap.transform.rotation);
    }

    public void VoiceCaution()
    {
        if (canVoicecCaution)
        {
            //if (Vector3.Distance(hit.point, transform.position) > 5)
            //{
            //    float dir = Vector3.Angle(transform.position, hit.point);

            //}
            goDes = hit.point;
            StartCoroutine(Trigger());
        }
    }

    GameObject stone;
    Vector3 goDes;
    IEnumerator Trigger(float h = 0.5f, float v = 0.5f, bool visible = true)
    {
        canVoicecCaution = false;// Debug.Log(canVoicecCaution);
        stone = Instantiate(throwableStone, stoneThrowingPoint.transform.position, stoneThrowingPoint.transform.rotation);
        stone.GetComponent<VoiceThrow>().GoDes(goDes, h, v, visible);
        yield return new WaitForSeconds(8);
        canVoicecCaution = true;// Debug.Log(canVoicecCaution);
    }

    private void UpdateAnimator()
    {
        Vector3 axisVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        float forwardBackwardsMagnitude = 0;//ileri geri
        float rightLeftMagnitude = 0;// sa� sol
        if (axisVector.magnitude > 0)//girdi varsa
        {
            Vector3 normalizedLookingAt = looked2Point - transform.position;
            normalizedLookingAt.Normalize();
            forwardBackwardsMagnitude = Mathf.Clamp(Vector3.Dot(axisVector, normalizedLookingAt), -1, 1);//�ki vekt�r�n nokta �arp�m�
            Vector3 perpendicularLookingAt = new Vector3(normalizedLookingAt.z, 0, -normalizedLookingAt.x);// �ne bak��
            rightLeftMagnitude = Mathf.Clamp(Vector3.Dot(axisVector, perpendicularLookingAt), -1, 1);
        }
        animator.SetFloat("Dikey", forwardBackwardsMagnitude);
        animator.SetFloat("Yatay", rightLeftMagnitude);
    }
}
