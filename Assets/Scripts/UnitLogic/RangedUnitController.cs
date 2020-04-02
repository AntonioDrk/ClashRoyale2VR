using UnityEngine;
using UnityEngine.AI;

public class RangedUnitController : UnitController
{
    [SerializeField]
    private Transform firePoint;
    [SerializeField]
    public GameObject bulletPrefab;

    void Start()
    {
        TestsChecker();
        
        Health = 200;
        Damage = 25;
        Speed = 10;
        TriggerZoneRadius = 10;
        Range = 2.5f;

        meshRenderer = GetComponent<MeshRenderer>();
        anim = gameObject.GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        cc = transform.GetChild(1).GetComponent<CapsuleCollider>();

        cc.radius = TriggerZoneRadius;

        SetTarget();
    }

    /// <summary>
    /// Attacks the current target
    /// </summary>
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

        if(target)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            bullet.transform.parent = null;
            var bulletScript = bullet.GetComponent<BulletController>();
            bulletScript.Speed = 2f;
            bulletScript.ArcHeightCoef = .75f;
            bulletScript.Damage = Damage;
            bulletScript.ownerId = OwnerId;
            bulletScript.TargetPos = target.transform.position;
        }
        //Rigidbody rb = bullet.GetComponent<Rigidbody>();
        //rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
    }
}
