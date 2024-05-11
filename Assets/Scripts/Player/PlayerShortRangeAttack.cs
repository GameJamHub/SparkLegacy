using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShortRangeAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.transform.parent.GetComponent<IDamage>().TakeDamage(5f);
        }
    }
}
