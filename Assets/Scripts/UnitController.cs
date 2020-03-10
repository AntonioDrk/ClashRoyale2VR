using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : EntityController
{
    public float Speed;
    public float TriggerZoneRadius;

    protected NavMeshAgent agent;

    protected float distance = 999;
    protected float attackCooldown = 1f;
    protected float cooldown = 0;

    protected GameObject mainTarget;
    protected GameObject target;
    protected Animator anim;
    protected CapsuleCollider cc;

    public List<GameObject> targets = new List<GameObject>();

    void Update()
    {
        if (distance > Range && target != null)
        {
            agent.destination = target.transform.position;
        }
        else
        {
            agent.destination = transform.position;

            if (mainTarget == null)
            {
                setTarget();
            }
            else if (target == null)
            {
                if (targets.Count > 0)
                {
                    target = targets[0];
                    targets.RemoveAt(0);
                }
                else
                    target = mainTarget;
            }
            else
            {
                cooldown += Time.deltaTime;
                if (cooldown > attackCooldown)
                {
                    cooldown = 0;
                    Attack();
                }
            }
        }

        if (target)
        {
            distance = Vector3.Distance(transform.position, target.transform.position);
        }
    }

    protected virtual void Attack() { }

    private void OnTriggerEnter(Collider other)
    {
        var tag = other.gameObject.tag;

        if (tag == "Unit" || tag == "RangedUnit" || tag == "Tower")
        {
            var targetScript = getTargetScript(other.gameObject);

            if (other != target && targetScript.OwnerId != OwnerId)
            {
                if (target != null && target.gameObject.tag == "Tower")
                    target = other.gameObject;
                else
                    targets.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (targets.Contains(other.gameObject))
            targets.Remove(other.gameObject);
    }

    protected void setTarget()
    {
        distance = 999;
        var possibleTargets = GameObject.FindGameObjectsWithTag("Tower");

        foreach (var pt in possibleTargets)
        {
            var targetScript = pt.GetComponent<TowerController>();

            if (targetScript.OwnerId != OwnerId && targetScript.Health > 0)
            {
                var currentDistance = Vector3.Distance(transform.position, pt.transform.position);

                if (currentDistance < distance)
                {
                    mainTarget = pt;
                    target = mainTarget;
                    distance = currentDistance;
                }
            }
        }
    }

    protected EntityController getTargetScript(GameObject entity)
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
