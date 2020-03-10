using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float Damage;
    public int OwnerId;

    public UnitController Owner;

    private void Start()
    {
        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        var tag = other.gameObject.tag;

        if (tag == "Unit" || tag == "RangedUnit" || tag == "Tower")
        {
            var targetScript = getTargetScript(other.gameObject);

            if (targetScript.OwnerId != OwnerId)
            {
                targetScript.TakeDamage(Damage);
                Destroy(gameObject); 

                if (targetScript.Health <= 0 && Owner.targets.Contains(other.gameObject))
                    Owner.targets.Remove(other.gameObject);
            }
        }
    }

    private EntityController getTargetScript(GameObject entity)
    {
        var targetType = entity.gameObject.tag;
        if (targetType == "Unit")
        {
            return entity.GetComponent<MeleeUnitController>();
        }
        else if (targetType == "RangedUnit")
        {
            return entity.GetComponent<RangedUnitController>();
        }
        else
        {
            return entity.GetComponent<TowerController>();
        }
    }
}
