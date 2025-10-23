using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTest : MonoBehaviour
{
    private float _timer;
    private int _level;
    [SerializeField] private int _allCountLevel;
    private void Update()
    {
        Debug.DrawLine(Vector3.zero, new Vector3(4, 4, 4));
    }

    //private void GetLevel(int level)
    //{
    //    if (level < _allCountLevel && level > 0)
    //    {
    //        _level = level;
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Такого уровня нет");
    //    }
    //}
    
}
