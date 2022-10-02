using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceCreate : MonoBehaviour
{
    int enemies = 0, count = 0;
    private void Start()
    {
        StartCoroutine(EnemiesCount());
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<Enemy>().state = Enemy.EnemyState.Alert;
            other.gameObject.GetComponent<Enemy>().lastLoc = transform.position;
            other.gameObject.GetComponent<Enemy>().voiceHear = true;
            enemies++;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            enemies--;
        }
    }
    IEnumerator EnemiesCount()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            count++;
            if (enemies == 0 || count == 3)
            {
                Destroy(gameObject);
            }
        }
    }
}
