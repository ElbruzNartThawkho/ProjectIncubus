using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOV3D : MonoBehaviour
{
    public Transform sightPos;//göz pozisyonu
    [SerializeField] Enemy enemy;
    Transform player;//hedef

    public Vector2 sight = new Vector2(25, 80);

    public float range = 8;

    bool isRed = false, isHit;

    RaycastHit hit;
    Color color;

    private void Awake()
    {
        player = GameObject.FindWithTag("PlayerHead").GetComponent<Transform>();
        color = Color.green;
    }

    //Görüþ çizdirme kýsmý
    private void Update()
    {
        if (isRed)
        {
            if (enemy.state != Enemy.EnemyState.Alert)
            {
                enemy.state = Enemy.EnemyState.Alert;
            }
            enemy.lastLoc = player.position;
            color = Color.red;
        }
        else
        {
            if (enemy.state != Enemy.EnemyState.Patrol && enemy.voiceHear == false)
            {
                enemy.state = Enemy.EnemyState.Patrol;
                if (enemy.enemyType is Enemy.EnemyType.ranger)
                {
                    enemy.enemyAnim.SetTrigger("Return");
                    enemy.agent.SetDestination(enemy.lastLoc);
                    enemy.shotState = false;
                }
            }
            color = Color.green;
        }

        Debug.DrawLine(sightPos.position, player.position, color);

        Vector3 diff = player.position - sightPos.position;

        // sýrayla x eksenine ve y eksenine göre açý hesaplama
        float diffHeigt = diff.y;
        diff.y = 0f;
        Vector2 modifiedDiff=new Vector2(diff.magnitude, diffHeigt);

        float angleX = Mathf.Atan2(modifiedDiff.y, modifiedDiff.x) * Mathf.Rad2Deg;
        float angleY = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg;

        angleX += sightPos.eulerAngles.x;
        angleY -= sightPos.eulerAngles.y;

        if(Physics.Linecast(sightPos.position, player.position, out hit))
        {
            isHit = hit.collider.gameObject.tag == "Player" || hit.collider.gameObject.tag == "PlayerHead";
        }
        isRed = Mathf.Abs(RadiusToDiameter(angleX)) < sight.x && Mathf.Abs(RadiusToDiameter(angleY)) < sight.y && Vector3.Distance(player.position, transform.position) < range && isHit;
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
