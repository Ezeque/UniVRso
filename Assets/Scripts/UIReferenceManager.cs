using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIReferenceManager : MonoBehaviour
{
    public static UIReferenceManager Instance;
    public GameObject EnterCaveUI;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        EnterCaveUI = GameObject.Find("EnterCaveUI");
        EnterCaveUI.SetActive(false);
    }

    void Update()
    {
        
    }
}
