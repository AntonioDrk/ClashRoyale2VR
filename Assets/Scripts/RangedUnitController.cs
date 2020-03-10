using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedUnitController : UnitController
{

    public float bulletForce = 20f;

    public Transform firePoint;
    public GameObject bulletPrefab;

    void Start()
    {
        Health = 200;
        Damage = 25;
        Speed = 10;
        TriggerZoneRadius = 5;
        Range = TriggerZoneRadius;

        anim = gameObject.GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        cc = GetComponent<CapsuleCollider>();

        cc.radius = TriggerZoneRadius;

        setTarget();
    }

    protected override void Attack()
    {
        // Determine which direction to rotate towards
        Vector3 targetDirection = target.transform.position - transform.position;

        // The step size is equal to speed times frame time.
        float singleStep = 180f * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        var bulletScript = bullet.GetComponent<BulletController>();
        bulletScript.Damage = Damage;
        bulletScript.OwnerId = OwnerId;
        bulletScript.Owner = this.GetComponent<RangedUnitController>();
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
    }
}
