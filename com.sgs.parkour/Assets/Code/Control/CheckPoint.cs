using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    CapsuleCollider _cc;
    [SerializeField] string name;
    [TextArea, SerializeField] string description;

    [SerializeField] ulong flagID;
    
    [SerializeField, Range(1,10)] float radius = 1f;
    [SerializeField, Range(0,1)] float heigth = 0;

    private void OnValidate()
    {   
        if(_cc == null)
        {
            _cc = GetComponent<CapsuleCollider>();
        }
        else
        {
            _cc.radius = radius;
        }
    }


    const float RADIUS = .2f;
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(Vector3.up * heigth, RADIUS);
    }

    public Vector3 GetRangeSpawn()
    {
        var range = Random.insideUnitSphere * radius;
        range.y = heigth;
        return range;
    }

}
