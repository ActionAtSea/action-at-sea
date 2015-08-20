using UnityEngine;
using System.Collections;

public interface IAimable 
{
    void AimWeapon(Vector3 fireDirection);
    void FireWeapon();
}
