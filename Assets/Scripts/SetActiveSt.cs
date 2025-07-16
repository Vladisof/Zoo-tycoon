using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveSt : MonoBehaviour
{
    [SerializeField] private GameObject objectToActivate;
    void Start()
    {
        objectToActivate.SetActive(false);
    }

}
