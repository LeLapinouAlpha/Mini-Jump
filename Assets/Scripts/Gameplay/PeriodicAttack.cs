using Assets.Scripts.Gameplay;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PeriodicAttack : MonoBehaviour
{
    [Header("Attack Parameters")]
    public float attackSpeed = 1f;
    public UnityEvent actions = null;
    public MonoBehaviour[] conditions;

    [Header("States")]
    private float attackCounter = 0f;
    private IAttackCondition[] attackConditions;

    void Start()
    {
        this.attackCounter = this.attackSpeed;
        this.attackConditions = this.conditions.OfType<IAttackCondition>().ToArray();
    }

    void Update()
    {
        if (this.attackCounter >= this.attackSpeed && this.attackConditions.All(c => c.CanAttack()))
        {
            this.actions?.Invoke();
            this.attackCounter = 0f;
        }
        else
        {
            this.attackCounter += Time.deltaTime;
        }
    }
}
