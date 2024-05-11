using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHealthController : MonoBehaviour,IDamage
{
    [SerializeField] private float m_maxHealth = 100f;
    [SerializeField] private NPC m_characterNPC;
    [SerializeField] private SpriteRenderer m_characterSprite;

    private float m_currentHealth;

    private void Start()
    {
        m_currentHealth = m_maxHealth;
    }

    public void TakeDamage(float amount, float forceX = 0, float forceY = 0, float duration = 0, Transform otherTransform = null)
    {
        m_currentHealth -= amount;
        StartCoroutine(HurtEffect(0.2f));
        if (m_currentHealth <= 0)
        {
            m_characterNPC.PlayDeath();
        }
    }
    private IEnumerator HurtEffect(float duration)
    {
        float currentTime = 0;
        bool isNormalAlpha = false;
        while (currentTime<duration)
        {
            m_characterSprite.color = isNormalAlpha ? Color.white : Color.red;
            currentTime += Time.deltaTime * 5f;
            isNormalAlpha = !isNormalAlpha;
            yield return new WaitForSeconds(0.1f);
        }
        m_characterSprite.color = Color.white;
    }
}
