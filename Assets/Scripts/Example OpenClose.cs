using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Базовый класс врага
public abstract class Enemy : MonoBehaviour
{
    public abstract void Attack();
}

// Добавляем новый тип врага без изменения существующего кода
public class Zombie : Enemy
{
    public override void Attack()
    {
        throw new NotImplementedException();
    }
}
public class Robot : Enemy
{
    public override void Attack()
    {
        throw new NotImplementedException();
    }
}

public class ExampleOpenClose : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
