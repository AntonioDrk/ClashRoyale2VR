using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeUnitController : UnitController
{
     
    void Start()
    {
        Health = 100;
        Damage = 20;
        Range = 1.5f;
        Speed = 10;
        TriggerZoneRadius = 5;

        anim = gameObject.GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>(); 
        cc = GetComponent<CapsuleCollider>();

        cc.radius = TriggerZoneRadius;

        setTarget();
    }

    protected override void Attack()
    {
        var targetType = target.gameObject.tag;
        var targetScript = getTargetScript(target);
        targetScript.TakeDamage(Damage);

        if(targetScript.Health <= 0 && targets.Contains(target.gameObject)) 
            targets.Remove(target.gameObject); 
    }

}
