using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour
{
    [SerializeField] private float m_damageAmount = 10f;
    [SerializeField] private float forceX;
    [SerializeField] private float forceY;
    [SerializeField] private float duration;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Name : "+other.gameObject.name);
        if(other.CompareTag("Player"))
        {
            other.transform.parent.GetComponent<IDamage>().TakeDamage(m_damageAmount,forceX, forceY, duration, transform);
        }
    }
}
