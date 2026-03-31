using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Gameplay
{
    public interface IAttackCondition
    {
        bool CanAttack();
    }
}
