using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOV3D : MonoBehaviour
{
    public Transform goal;//hedef
    public Transform sightPos;//göz pozisyonu
    public Vector2 sight = new Vector2(25, 80);

    public int sightStep = 15;//Görüþ çizdirme parametresi
    bool IsRed = false;

    //Görüþ çizdirme kýsmý
    private void Update()
    {
        Debug.Log(IsRed);
        Vector3 diff = goal.position - sightPos.position;
        // sýrayla x eksenine ve y eksenine göre açý hesaplama
        float diffHeigt = diff.y;
        diff.y = 0f;
        Vector2 modifiedDiff=new Vector2(diff.magnitude, diffHeigt);

        float angleX = Mathf.Atan2(modifiedDiff.y, modifiedDiff.x) * Mathf.Rad2Deg;
        float angleY = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg;

        angleX += sightPos.eulerAngles.x;
        angleY += sightPos.eulerAngles.x;

        IsRed = Mathf.Abs(RadiusToDiameter(angleX)) < sight.x && Mathf.Abs(RadiusToDiameter(angleY)) < sight.y;
    }
    float RadiusToDiameter(float angle)
    {
        angle %= 360f;
        if (angle > 180f)
            return angle + 360;
        if (angle < -180f)
            return angle + 360;
        return angle;
    }
}
