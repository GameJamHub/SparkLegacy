using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbsorbElectric
{
    public void Absorb(float amount);
    public void Detection(Collectable collectable);
}
