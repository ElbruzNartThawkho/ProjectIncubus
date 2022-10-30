using UnityEngine;

public class ButtonClick : MonoBehaviour
{
    private Camera c;       // c: Camera.
    private RaycastHit rH; // rH: RaycastHit.
    private Touch t;      // t: Touch.

    private void Awake()
    {
        c= GetComponent<Camera>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Input.touchCount is not 0
        {
            if(Physics.Raycast(c.ScreenPointToRay(Input.mousePosition), out rH))
            {
                print(rH.transform.name);
            }
        }
    }
}