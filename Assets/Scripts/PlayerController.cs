using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class PlayerController : MonoBehaviour
{
    System.Random ran;


    [SerializeField]
    private bool _testingMode;

    public float Mana_human { get; set;}
    public float Mana_computer { get; set;}
    public float MaxMana { get; set; }
    public float ManaRegen { get; set; }

    private float _regenTime = 1; // Seconds count

    [SerializeField]
    private GameObject rangedGroup;
    [SerializeField]
    private GameObject meleeGroup;

    [SerializeField]
    private GameObject EnemySide;
    [SerializeField]
    private Material EnemyMaterial;

    [SerializeField] 
    private ManaBar manaBar; 
    [SerializeField] 
    private Text manaText;

    public List<GameObject> playerUnits = new List<GameObject>();
    public List<GameObject> enemyUnits = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        ran = new System.Random();

        Mana_human = 250;
        Mana_computer = 250;
        MaxMana = 1000;
        ManaRegen = 50;

        manaBar.SetMaxMana(MaxMana);
        manaBar.SetMana(Mana_human);
        manaText.text = Mana_human.ToString() + " / " + MaxMana.ToString();

        AddRandomUnit(playerUnits);
        AddRandomUnit(playerUnits);
        AddRandomUnit(playerUnits);

        AddRandomUnit(enemyUnits);
        AddRandomUnit(enemyUnits);
        AddRandomUnit(enemyUnits);

        StartCoroutine(ManaTimer());
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f); 

        GameObject units = null;
        int ownerId = 1;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            units = meleeGroup;
        }
        else if (Input.GetKeyDown(KeyCode.W))
            units = rangedGroup;
        else if (_testingMode && Input.GetKeyDown(KeyCode.A))
        {
            units = meleeGroup;
            ownerId = 2;
        }
        else if (_testingMode && Input.GetKeyDown(KeyCode.S))
        {
            units = rangedGroup;
            ownerId = 2;
        }

        //when testing mode is true, the computer is not allowed to choose his moves
        if (Mana_computer >= rangedGroup.GetComponent<GroupController>().Cost &&  _testingMode==false)//computerul face o mutare
        {
            Vector3 AIpos = new Vector3(396f, 249f, 0f);
            
            int val = ran.Next(0, 10);
            Debug.Log("val = " + val);
            if (val % 2 == 0)
                SpawnUnits(AIpos, rangedGroup, ownerId=2);
            else
                SpawnUnits(AIpos, meleeGroup, ownerId=2);
        }

        if (units != null)
        {
            SpawnUnits(mousePos, units, ownerId);
        }
    }

    /// <summary>
    /// Spawns a group of units at mouse position
    /// </summary>
    /// <param name="mousePos"> The current position of the mouse </param>
    /// <param name="units"> The units to be spawned </param>
    /// <param name="ownerId"> The Id of the owner </param>
    public void SpawnUnits(Vector3 mousePos, GameObject units, int ownerId)
    {
        var cost = units.GetComponent<GroupController>().Cost;

        //I think the code below was meant to temporarily replace the AI so I commented it (Smit) 
        /*if (!_testingMode)
        {
            if (!playerUnits.Contains(units) || Mana_human < cost)
                return;

            Mana_human -= cost;

            playerUnits.Remove(units);//was this line important?
            AddRandomUnit(playerUnits);
        }*/


        if (ownerId == 1 && Mana_human < cost)
            return;
        if (ownerId == 2 && Mana_computer < cost)
            return;
        if (ownerId == 1)
                Mana_human -= cost;
        if (ownerId == 2)
            Mana_computer -= cost;
            
        playerUnits.Remove(units);//nu stiu ce face linia asta


        Vector3 worldPos;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f))
        {
            worldPos = hit.point;
        }
        else
        {
            worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            return;
        }

        GameObject go = Instantiate(units) as GameObject;

        if (ownerId == 1)
            go.transform.SetParent(this.transform);
        else
            go.transform.SetParent(EnemySide.transform);

        go.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        worldPos = new Vector3(worldPos.x, 3.3f, worldPos.z);

        for (int i = 0; i < go.transform.childCount; i++)
        {
            var child = go.transform.GetChild(i);
            var agent = child.GetComponent<UnityEngine.AI.NavMeshAgent>();
            agent.Warp(worldPos);
            agent.enabled = true;

            if (ownerId == 2)
            {
                child.GetComponent<UnitController>().OwnerId = ownerId;
                child.GetComponent<MeshRenderer>().material = EnemyMaterial;
            }
        }
    }

    /// <summary>
    /// Add a random unit to the units list
    /// </summary>
    /// <param name="units"> The units list </param>
    private void AddRandomUnit(List<GameObject> units)
    {
        float val = Random.Range(1, 100);

        if (val < 50)
            units.Add(meleeGroup);
        else
            units.Add(rangedGroup);
    }

    /// <summary>
    /// Timer for the mana regeneration
    /// </summary>
    /// <returns> Flag for when _regenTime number of seconds passed </returns>
    private IEnumerator ManaTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(_regenTime);

            //update Human's mana
            if(Mana_human < MaxMana)
            {
                Mana_human += ManaRegen;
                manaBar.SetMana(Mana_human);//human has a visible manaBar, computer has not
                manaText.text = Mana_human.ToString() + " / " + MaxMana.ToString();
            } 
            if (Mana_human > MaxMana)
            {
                Mana_human = MaxMana;
                manaBar.SetMana(Mana_human);
                manaText.text = Mana_human.ToString() + " / " + MaxMana.ToString();
            }

            //update computer's mana
            if (Mana_computer < MaxMana)
            {
                Mana_computer += ManaRegen;
            }
            if (Mana_computer > MaxMana)
            {
                Mana_computer = MaxMana;
            }

        }
    }
}
