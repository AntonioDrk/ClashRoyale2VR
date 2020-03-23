using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool testingMode;
    public float Mana;
    public float MaxMana { get; set; }
    public float ManaRegen { get; set; }

    private float RegenTime = 1; // Second count

    protected float Timer;
    public GameObject rangedGroup;
    public GameObject meleeGroup;

    public GameObject EnemySide;
    public Material EnemyMaterial;

    // Start is called before the first frame update
    void Start()
    {
        Mana = 250;
        MaxMana = 1000;
        ManaRegen = 50;

        StartCoroutine(ManaTimer());
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);

        GameObject units = null;
        int ownerId = 1;

        if (Input.GetKeyDown(KeyCode.Q))
            units = meleeGroup;
        else if (Input.GetKeyDown(KeyCode.W))
            units = rangedGroup;
        else if (testingMode && Input.GetKeyDown(KeyCode.A))
        {
            units = meleeGroup;
            ownerId = 2;
        }
        else if (testingMode && Input.GetKeyDown(KeyCode.S))
        {
            units = rangedGroup;
            ownerId = 2;
        }

        if (units != null)
        {
            var cost = units.GetComponent<GroupController>().Cost;

            if(!testingMode)
            {
                if (Mana < cost)
                    return;

                Mana -= cost;
            }

            Vector3 wordPos;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f))
            {
                wordPos = hit.point;
            }
            else
            {
                wordPos = Camera.main.ScreenToWorldPoint(mousePos);
            }
            
            GameObject go = Instantiate(units) as GameObject;

            if(ownerId == 1)
                go.transform.SetParent(this.transform);
            else
                go.transform.SetParent(EnemySide.transform);

            go.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            
            for(int i=0; i<go.transform.childCount; i++)
            {
                var child = go.transform.GetChild(i); 
                child.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(wordPos);

                if (ownerId == 2)
                {
                    child.GetComponent<UnitController>().OwnerId = ownerId;
                    child.GetComponent<MeshRenderer>().material = EnemyMaterial;
                }
            }
        }
    }
    
    private IEnumerator ManaTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(RegenTime);
            if(Mana < MaxMana)
                Mana += ManaRegen;
            if (Mana > MaxMana)
                Mana = MaxMana;
        }
    }
}
