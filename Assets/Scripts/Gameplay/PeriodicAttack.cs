using Assets.Scripts.Gameplay;
using Assets.Scripts.Utils;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PeriodicAttack : MonoBehaviour
{
    [Header("Timer Parameters")]
    public float attackSpeed = 1f;

    [Header("Actions Parameters")]
    public UnityEvent actions = null;

    [Header("Conditions Parameters")]
    public MonoBehaviour[] conditions;
    public ConditionCheckType conditionCheckType = ConditionCheckType.All;

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
        bool conditionsMet = this.conditionCheckType switch
        {
            ConditionCheckType.All => this.attackConditions.All(c => c.CanAttack()),
            ConditionCheckType.Any => this.attackConditions.Any(c => c.CanAttack()),
            _ => throw new System.NotImplementedException(),
        };

        if (this.attackCounter >= this.attackSpeed && conditionsMet)
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
