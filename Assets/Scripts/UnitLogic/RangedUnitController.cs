using UnityEngine;
using UnityEngine.AI;

public class RangedUnitController : UnitController
{
    
    public float bulletForce = 20f;

    public Transform firePoint;
    public GameObject bulletPrefab;

    void Start()
    {
        TestsChecker();
        
        Health = 200;
        Damage = 25;
        Speed = 10;
        TriggerZoneRadius = 5;
        Range = TriggerZoneRadius;

        meshRenderer = GetComponent<MeshRenderer>();
        anim = gameObject.GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        cc = transform.GetChild(1).GetComponent<CapsuleCollider>();

        cc.radius = TriggerZoneRadius;

        SetTarget();
    }

    protected override void Attack()
    {
        /* All of this happens in the UnitController for all units
        // Determine which direction to rotate towards
        Vector3 targetDirection = target.transform.position - transform.position;

        // The step size is equal to speed times frame time.
        float singleStep = 180f * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, newDirection * 5f, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);*/

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        var bulletScript = bullet.GetComponent<BulletController>();
        bulletScript.Damage = Damage;
        bulletScript.OwnerId = OwnerId;
        bulletScript.Owner = GetComponent<RangedUnitController>();
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
    }
}
