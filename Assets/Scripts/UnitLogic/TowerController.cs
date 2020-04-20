using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyScripts
{
    public class TowerController: EntityController
    {
        // Start is called before the first frame update
        void Start()
        {
            Health = 250;
            Damage = 50;
            Range = 1.5f;
        }
    
        // Update is called once per frame
        void Update()
        {
    
        }
    }

}

