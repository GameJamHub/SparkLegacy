using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ThunderArrowSpawner : MonoBehaviour
{
    [SerializeField] private ThunderArrow m_thunderArrow;

    public void Spawn(Vector3 direction)
    {
        ThunderArrow thunderArrow = Instantiate(m_thunderArrow, transform.position, quaternion.identity);
        thunderArrow.Config(direction);
    }
}
