using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveList : MonoBehaviour
{
    public List<MisionObjectiveSO> objectives;
    private int currentObjective = 0;
    public static event Action<string, string> ShowObjective;

    public static ObjectiveList Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //ShowObjective?.Invoke(objectives[currentObjective].title, objectives[currentObjective].description);
    }

    public void ShowNext()
    {
        if (currentObjective <= objectives.Count)
        {
            
            ShowObjective?.Invoke(objectives[currentObjective].title, objectives[currentObjective].description);
            currentObjective++;
        }

    }



}
