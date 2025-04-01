using UnityEngine;

[System.Serializable]
public class ControlPoint
{
    public CheckPoint Control;
    public Vector3 Position
    {
        get{
            if(Control != null)
            {
                return Control.transform.position;
            }
            else
            {
                return default;
            }
        }
    }

    public ControlPoint(CheckPoint control)
    {
        Control = control;
    }
        
}