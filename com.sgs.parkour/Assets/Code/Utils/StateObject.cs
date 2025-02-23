using Unity.VisualScripting;
using UnityEngine;

namespace Parkour.State
{
    [System.Serializable]
    public class StateObject<T>
    {
        public delegate void StateAction(T currentState);
        public event StateAction OnStateChangedCallback;

        [SerializeField] T state;

        public StateObject(T startState = default)
        {
            state = startState;
        }

        public StateObject()
        {
            state = default;
        }

        public bool Change(T target)
        {
            if(!state.Equals(target))
            {
                state = target;
                OnStateChangedCallback?.Invoke(target);
                return true;
            }
            
            return false;
        }

        public bool Reset()
        {
            return Change(default);
        }

        public T State => state;

    }

}
