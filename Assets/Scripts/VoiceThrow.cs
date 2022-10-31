using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceThrow : MonoBehaviour
{
    float animTime, heightV, animspeedV;
    private Vector3 destination;
    bool visibility = true, desPoint2Arrive = false;
    private Quaternion rot = new Quaternion(0, 0, 0, 0);
    [SerializeField] GameObject voicePref;
    private void Awake()
    {
        destination = transform.localPosition;
    }
    void Update()
    {
        if (Vector3.Distance(transform.localPosition, destination) > 0.1f && desPoint2Arrive)
        {
            animTime += Time.deltaTime;
            animTime %= 5f;
            transform.localPosition = Parabola(transform.localPosition, destination, heightV, animTime / animspeedV);
        }
        else //if (desPoint2Arrive != false)
        {
            if (visibility == true)
            {
                Instantiate(voicePref, gameObject.transform.position, transform.rotation);
                Destroy(gameObject);
            }
            transform.localRotation = rot;
            desPoint2Arrive = false;
        }
    }
    public void GoDes(Vector3 goDes, float h = 2, float v = 1, bool visible = true)
    {
        visibility = visible;
        destination = goDes;
        heightV = h; animspeedV = v;
        desPoint2Arrive = true;
        gameObject.SetActive(true);
    }
    Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }
}
