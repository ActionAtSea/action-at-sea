using UnityEngine;
using System.Collections;

public interface IDamageable
{
    void InflictDamage(float damage);
    void RepairDamage(float repairAmount);
}