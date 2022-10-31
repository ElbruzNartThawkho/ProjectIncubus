using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public GameObject acidEffect;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Instantiate(acidEffect, transform.position, acidEffect.transform.rotation);
            Destroy(other.gameObject); Destroy(gameObject);
        }
    }
}
