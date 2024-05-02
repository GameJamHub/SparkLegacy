using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public float absorbAmount = 10f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            other.transform.parent.GetComponent<IAbsorbElectric>().Detection(this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            other.transform.parent.GetComponent<IAbsorbElectric>().Detection(null);
        }
    }
}
