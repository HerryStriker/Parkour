using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ModeInteraction : MonoBehaviour
{
    Holder holder;
    public event Action OnGetCheckPointCallback;
    public event Action OnCheckPointEnter;
    [SerializeField] CheckPoint lastestCheckPoint;

    public bool HasCheckPoint
    {
        get
        {
            return lastestCheckPoint != null;
        }
    }

    void Start()
    {
        holder = GetComponent<Holder>();

        InputManager.Instance.EnableInteract();
        
        InputManager.Instance.OnInteractStart += OnInteract;

        holder.CheckPoint_Check.OnEnter += OnCheckEnter;

    }

    private void OnCheckEnter(object sender, EventArgs e)
    {
            Debug.Log("OnCheckEnter");
        if(!holder.EqualCheckPoint(lastestCheckPoint))
        {
            Debug.Log("!EqualCheckPoint");
            OnCheckPointEnter?.Invoke();
        }
    }


    private void OnInteract(object sender, EventArgs e)
    {
        var checkPoint = holder.CheckPoint_Check?.GetData();

        if(checkPoint != null)
        {
            if(lastestCheckPoint != null && lastestCheckPoint.Equals(checkPoint))
            {
                return;
            }
            else
            {
                lastestCheckPoint = checkPoint;
                checkPoint.PlayPickupEffect();
                OnGetCheckPointCallback?.Invoke();
            }
        }
    }



    public Vector3 LastestCheckPointPosition
    {
        get
        {
            if(lastestCheckPoint != null)
            {
                return lastestCheckPoint.GetRangeSpawn();
            }

            return default;
        }
    }
}
