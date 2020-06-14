using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSwitcher : MonoBehaviour
{
    public GameObject plane;

    public void SetActive()
    {
        plane.SetActive(true);
    }

    public void SetInactive()
    {
        plane.SetActive(false);
    }
}
