using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;
using Unity.Mathematics;





#if UNITY_EDITOR
using UnityEditor;
#endif

public class RushMode : MonoBehaviour
{

    public static RushMode Instance {get; private set;}

    [Header("Prefab")]
    [SerializeField] CheckPoint startPointPrefab;
    [SerializeField] CheckPoint checkPointPrefab;
    [SerializeField] CheckPoint endPointPrefab;
    [SerializeField] GameObject platformPrefab;

    [Header("Control Parent")]
    [SerializeField] Transform platformParent;
    [SerializeField] Transform controlParent;


    ModeManager modeManager;

    void Awake()
    {
        Instance = this;

        modeManager = GetComponent<ModeManager>();
        modeManager.OnModeStartsCallback += OnModeStartsCallback;
    }

    private void OnModeStartsCallback(ModeType type)
    {
        switch (type)
        {
            case ModeType.RUSH:
                // modeManager.StartGame();        
            break;            
        }
    }

#region Obstacles
    [Header("Obstacles")]
    [SerializeField, Range(0,1000)] float horizontalDistance = 1f;
    [SerializeField, Range(0, 100)] int platformScale = 4;
    [SerializeField, Range(0, 1)] float startPadding = 0.01f;
    [SerializeField, Range(0, 1)] float endPadding = 0.01f;
    [SerializeField, Range(.01f,.5f)] double stepDistanceUnits = 0.05f;
    [SerializeField] double stepOverlapUnits = 0.05f;


    [Space(5)]
    [SerializeField] List<Transform> platformPoints;
    
    float START_PADDING {
        get 
        {
            return startPadding / (distance / 100);
        }
    } 

    public void GenerateObstacles()
    {
        DestroyObstacles();

        double n = START_PADDING;

        Vector3 start = startPoint.Position;
        Vector3 end = endPoint.Position;

        do
        {
            Vector3 normalizedPosition = Vector3.Lerp(start, end, (float)n);

            var endDirection = start - end;

            
                for (int i = 0; i < 2; i++)
                {
                    float random = UnityEngine.Random.Range(0,horizontalDistance);
                    float hDistance = i == 0 ? random : -random;

                    Vector3 position = Quaternion.LookRotation(endDirection) * Vector3.right * hDistance  + normalizedPosition;
                    var platform = Instantiate(platformPrefab, position, default, platformParent).transform;

                    var scale = new Vector3(Utils.Random(2.68f, 2.68f * platformScale),0f,Utils.Random(1.52f,1.52f * platformScale));
                    scale.y = .8f;
                    platform.localScale = scale;
                    platform.rotation = Quaternion.Euler(0,UnityEngine.Random.Range(0,360),0);
                    platformPoints.Add(platform);
                }
            


            n += stepDistanceUnits;
        }
        while (n < endPadding);
    }
    void DestroyObstacles()
    {
                // DESTROY EVERYTHING THAT IS ON LIST BEFORE RESPAWN MORE
        foreach(var obstacles in platformPoints)
        {
            DestroyImmediate(obstacles.gameObject);
        }

        // CLEAR THE LIST
        platformPoints.Clear();


    }
#endregion

#region Control
    [Header("Control Generation")]
    [SerializeField] ControlPoint startPoint;
    [SerializeField] ControlPoint endPoint;
    [SerializeField] List<ControlPoint> checkPoints;
    [SerializeField, Range(0,1000)] int distance = 100;
    [SerializeField, Range(0f, 1f)] float height = .5f;

    public void SetupStartAndEnd()
    {
        DestroyEverything();
        Vector3 start = startPoint.Position;

        Vector3 end = new Vector3(0,height,1f).normalized * distance; 
        Debug.Log($"Distance: {Vector3.Distance(start, end)}");

        var dir = end - start;

            CheckPoint startCheckPoint =InstatiateControl(startPointPrefab, start, dir,false, controlParent);
            startCheckPoint.SetFlagID(-1, CheckPoint.CheckPointType.START);
            startPoint.Control = startCheckPoint;

            // startPoint.control = Instantiate(startPointPrefab, start, rot, controlParent);
            // startPoint.control.transform.localRotation = rot;
            // startPoint.position = start;

            CheckPoint endCheckPoint = InstatiateControl(endPointPrefab, end, dir, true, controlParent);
            endCheckPoint.SetFlagID(-1, CheckPoint.CheckPointType.END);
            endPoint.Control = endCheckPoint;
            // endPoint.control = Instantiate(endPointPrefab,end,rot, controlParent);
            // endPoint.control.transform.localRotation = Quaternion.Euler(0,endPoint.control.transform.rotation.eulerAngles.y + 180f,0);
            // endPoint.position = end;

            int n_checkpoints = distance / 100;
            float[] points = PathCheckPoints(n_checkpoints);

            for (int i = 0; i < points.Length; i++)
            {
                int flagId = i + 1;
                float normalized = points[i];
                Debug.Log(normalized + ", " + flagId);

                //NORMALIZED CHECK POINT POSITION
                Vector3 ncpp = Vector3.Lerp(start, end, normalized);

                CheckPoint checkPoint = InstatiateControl(checkPointPrefab, ncpp, dir, false, controlParent);
                checkPoint.SetFlagID(flagId, CheckPoint.CheckPointType.CHECK);
                checkPoints.Add(new ControlPoint(checkPoint));
            }

    }

    public void DestroyEverything()
    {

        DestroyObstacles();

        if(startPoint?.Control != null)
            DestroyImmediate(startPoint?.Control.gameObject);

        if(endPoint?.Control != null)
            DestroyImmediate(endPoint?.Control.gameObject);

        foreach (var checkPoint in checkPoints)
        {
            DestroyImmediate(checkPoint?.Control.gameObject);
        }

        checkPoints.Clear();

    }

    public BoxCollider GetEndBoxCollider()
    {
        return endPoint.Control.GetComponent<BoxCollider>();
    }
#endregion

#region Utils

    CheckPoint InstatiateControl(CheckPoint prefab, Vector3 position, Vector3 direction, bool inverse, Transform parent)
    {
        var dir = inverse ? Quaternion.LookRotation(direction).eulerAngles.y + 180f : Quaternion.LookRotation(direction).eulerAngles.y;
        var rot = Quaternion.Euler(0,dir,0);

        return Instantiate(prefab, position, rot, parent);
    }

    double Distance(Vector3 a, Vector3 b)
    {
        // d=√((x2 – x1)² + (y2 – y1)²)
      return Math.Sqrt(Pow(b.x - a.x,2) + Pow(b.y - b.y,2) + Pow(b.z- a.z, 2));
    }

    double Pow(double initialValue , double exponent)
    {
            double power = initialValue;
            for (int i = 0; i < exponent ; i++)
            {
                initialValue *= power;
            }
            return initialValue;
    }

    float[] PathCheckPoints(int n)
    {
        float[] points = new float[n];

        for (int i = 0; i < n; i++)
        {
            int index = i + 1;
            float product = 1f - 1f/(n+1) * index;

            points[i] = product;
        }
        return points;
    }
    #endregion

    #region Gizmos
    void OnDrawGizmos()
    {
        Vector3 start = startPoint.Position;


        Vector3 end = new Vector3(0,height,1f).normalized * distance; 


        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(start, .5f);
        Gizmos.DrawSphere(end, .5f);

        Vector3 halfDistance = start + end / 2;
        Vector3 endDirection = end - start;
        Vector3 rangeSpace = Quaternion.LookRotation(endDirection) * Vector3.right * horizontalDistance;

        int n_checkpoints = distance / 100;
        float[] points = PathCheckPoints(n_checkpoints);

        //NORMALIZED CHECK POINT POSITION
        for (int i = 0; i < points.Length; i++)
        {
            float normalized = points[i];
            
            Vector3 ncpp = Vector3.Lerp(start, end, normalized);

        List<char> chars = new List<char>();

            
            var color = Color.green;
            color.a = .4f;
            Gizmos.color = color;
            Gizmos.DrawSphere(ncpp, 4f);
        }


        int k = 0;
        for (float j = START_PADDING; j < endPadding;  j += (float)stepDistanceUnits)
        {
            Vector3 stepPosition = Vector3.Lerp(start, end, j);


            Gizmos.color = Color.red;
            Gizmos.DrawSphere(stepPosition, .5f);

            if(platformPoints.Count > 0 && k < platformPoints.Count){
                Gizmos.DrawLine(stepPosition, platformPoints[k * 2].position);
                Gizmos.DrawLine(stepPosition, platformPoints[1 + k * 2].position);
                Gizmos.DrawSphere(platformPoints[k * 2].position, .8f);
                Gizmos.DrawSphere(platformPoints[1 + k * 2].position, .8f);

            }
            k++;
        }
            
            


            Gizmos.color = Color.blue;
            Gizmos.DrawLine(start + rangeSpace, end + rangeSpace);
            Gizmos.DrawSphere(start + rangeSpace, .4f);;
            Gizmos.DrawSphere(end + rangeSpace, .4f);;



            Gizmos.DrawLine(start - rangeSpace, end - rangeSpace);
            Gizmos.DrawSphere(start - rangeSpace, .4f);;
            Gizmos.DrawSphere(end - rangeSpace, .4f);;

        

    }
    #endregion
}


#if UNITY_EDITOR
[CustomEditor(typeof(RushMode))]
public class RushModeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var rushMode = target as RushMode;
        DrawDefaultInspector();

        if(GUILayout.Button("Setup"))
        {
            rushMode.SetupStartAndEnd();
        }

        if(GUILayout.Button("Generater Objectacles"))
        {
            rushMode.GenerateObstacles();
        }

        if(GUILayout.Button("Destroy"))
        {
            rushMode.DestroyEverything();
        }
    }


}
#endif

