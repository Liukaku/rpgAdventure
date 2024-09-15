using System;

namespace RpgAdventure
{
    public interface IAttackListener
    {
        public void MeleeAttackStart();

        public void MeleeAttackEnd();
    }
}