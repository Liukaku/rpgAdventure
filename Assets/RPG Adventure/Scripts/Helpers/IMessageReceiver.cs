using System;

namespace RpgAdventure
{
    public interface IMessageReceiver
    {
        public enum MessageType
        {
            DAMAGED,
            DEAD
        }
        void OnReceiveMessage(MessageType type, Damageable sender, Damageable.DamageMessage message);
    }
}