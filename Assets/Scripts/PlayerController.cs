using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Mana { get; set; }
    public float MaxMana { get; set; }
    public float ManaRegen { get; set; }

    private float RegenTime = 1; // Second count

    protected float Timer;
    public GameObject rangedGroup;
    public GameObject meleeGroup;
    public Transform playerSide;

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

        if (Input.GetMouseButtonDown(0))
        {
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
            Debug.Log(wordPos.x + " " + wordPos.y + " " + wordPos.z);
            GameObject go = Instantiate(meleeGroup, wordPos, Quaternion.identity);
            go.transform.SetParent(playerSide);
            go.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
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
