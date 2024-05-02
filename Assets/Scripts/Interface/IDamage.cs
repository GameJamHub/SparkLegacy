using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    public void TakeDamage(float amount, float forceX = 0f, float forceY = 0f, float duration = 0f, Transform otherTransform = null);
}
