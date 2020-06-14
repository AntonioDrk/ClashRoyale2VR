using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyScripts
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private bool _testingMode;

        public float Mana { get; set;}
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

            
            /*if (OVRInput.GetDown(OVRInput.Button.Four) || Input.GetKeyDown(KeyCode.Q))
                units = meleeGroup;
            else if (OVRInput.GetDown(OVRInput.Button.Three) || Input.GetKeyDown(KeyCode.W))
                units = rangedGroup;
            else*/ 
            if (_testingMode && Input.GetKeyDown(KeyCode.A))
            {
                units = meleeGroup;
                ownerId = 2;
            }
            else if (_testingMode && Input.GetKeyDown(KeyCode.S))
            {
                units = rangedGroup;
                ownerId = 2;
            }

            if (units != null)
            {
                SpawnUnitsAtMouse(mousePos, units, ownerId);
            }
        }
        
        public void SpawnPlayerUnits(Vector3 pos, GameObject units)
        {
            var cost = units.GetComponent<GroupController>().Cost;

            if (!_testingMode)
            {
                if (Mana < cost)
                    return;

                Mana -= cost;
            }
            
            GameObject go = Instantiate(units);
            go.transform.SetParent(transform);
            go.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            Vector3 newPos = new Vector3(pos.x, 3.3f, pos.z);
            
            for (int i = 0; i < go.transform.childCount; i++)
            {
                var child = go.transform.GetChild(i);
                var agent = child.GetComponent<UnityEngine.AI.NavMeshAgent>();
                agent.Warp(newPos);
                agent.enabled = true;
            }
        }

        /// <summary>
        /// Spawns a group of units at mouse position
        /// </summary>
        /// <param name="mousePos"> The current position of the mouse </param>
        /// <param name="units"> The units to be spawned </param>
        /// <param name="ownerId"> The Id of the owner </param>
        public void SpawnUnitsAtMouse(Vector3 mousePos, GameObject units, int ownerId)
        {
            var cost = units.GetComponent<GroupController>().Cost;

            if (!_testingMode)
            {
                if (Mana < cost)
                    return;

                Mana -= cost;
            }

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
        /// Timer for the mana regeneration
        /// </summary>
        /// <returns> Flag for when _regenTime number of seconds passed </returns>
        private IEnumerator ManaTimer()
        {
            while (true)
            {
                yield return new WaitForSeconds(_regenTime);
                if(Mana < MaxMana)
                    Mana += ManaRegen;
                if (Mana > MaxMana)
                    Mana = MaxMana;
            }
        }
    }
}

