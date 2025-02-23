using Parkour.State;
using UnityEngine;


namespace Parkour.Character
{        
    public class State : MonoBehaviour
    {
        Holder holder;

        [SerializeField] StateObject<CHARACTER_MOVEMENT> _locomotion;
        [SerializeField] StateObject<CHARACTER_LIFE> _life;
        [SerializeField] StateObject<CHARACTER_JUMP> _jump;

        void Awake()
        {
            holder = GetComponent<Holder>();

            _locomotion = new StateObject<CHARACTER_MOVEMENT>();
            _life = new StateObject<CHARACTER_LIFE>();
            _jump = new StateObject<CHARACTER_JUMP>();
        }

        public StateObject<CHARACTER_MOVEMENT> Locomotion => _locomotion;
        public StateObject<CHARACTER_LIFE> Life => _life;
        public StateObject<CHARACTER_JUMP> Jump => _jump;
    }

    public enum CHARACTER_MOVEMENT
    {
        WALK,
        RUN,
        SPRINT,
        FALLING,
        ROCKETING,
    }

    public enum CHARACTER_JUMP
    {
        LOADING,
        CHARGED,
        JUMPING,
    }

    public enum CHARACTER_LIFE
    {
        ALIVE,
        DEAD,
        SPECTATING,
    }

}