using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderArrow : MonoBehaviour
{
    [SerializeField] private float m_lifeTime;
    [SerializeField] private float m_speed;

    private Vector3 m_direction;

    private void Start()
    {
        Destroy(gameObject,m_lifeTime);
    }

    public void Config(Vector3 direction)
    {
        m_direction = Vector3.one;
        m_direction.x *= direction.x;
        transform.localScale = m_direction;
        m_direction = direction;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            other.transform.parent.GetComponent<IDamage>().TakeDamage(10f);
        }
    }

    private void Update() {
        transform.position += m_direction * m_speed * Time.deltaTime;
    }
}
