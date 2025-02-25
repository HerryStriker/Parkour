using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RushMode : MonoBehaviour
{

    [Header("Prefab")]
    [SerializeField] CheckPoint startPointPrefab;
    [SerializeField] CheckPoint checkPointPrefab;
    [SerializeField] CheckPoint endPointPrefab;
    [SerializeField] GameObject platformPrefab;

    [Header("Control")]
    [SerializeField] ControlPoint startPoint;
    [SerializeField] List<ControlPoint> checkPoints;
    [SerializeField] ControlPoint endPoint;
    [SerializeField] List<GameObject> platformPoints;

    ModeManager modeManager;

    void Awake()
    {
    }

    void Start()
    {
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

    [SerializeField, Range(0,1000)] float horizontalDistance = 1f;
    [SerializeField] float stepDistanceUnits = 0.05f;

    public void GenerateObstacles()
    {
        DestroyObstacles();

        float n = 0;

        Vector3 start = startPoint.position;
        Vector3 end = endPoint.position;

        do
        {
            Vector3 normalizedPosition = Vector3.Lerp(start, end, n);

            var endDirection = start - end;

            
                for (int i = 0; i < 2; i++)
                {
                    float random =UnityEngine.Random.Range(0,horizontalDistance);
                    float hDistance = i == 0 ? random : -random;

                    Vector3 position = Quaternion.LookRotation(endDirection) * Vector3.right * hDistance  + normalizedPosition;
                    Debug.DrawLine(normalizedPosition, position, Color.green, 5f);
                    var platform = Instantiate(platformPrefab, position, default).transform;

                    var scale = new Vector3(UnityEngine.Random.Range(2.68f, 2.68f * 4),0f,UnityEngine.Random.Range(1.52f,1.52f * 4));
                    scale.y = .8f;
                    platform.localScale = scale;
                    platform.rotation = Quaternion.Euler(0,UnityEngine.Random.Range(0,360),0);
                    platformPoints.Add(platform.gameObject);
                }
            


            n += 100 / stepDistanceUnits;
        }
        while (n < 1);

        n = 0;

    }


    [Header("Generation")]
    [SerializeField] int MAX_DISTANCE = 100;
    [SerializeField] int MAX_HEIGHT = 10;
    public void SetupStartAndEnd()
    {
        DestroyEverything();
        Vector3 startPosition = startPoint.position;

        Vector3 randomEndPosition = new Vector3(UnityEngine.Random.Range(-1f,1f), 0f, UnityEngine.Random.Range(-1f,1f)).normalized * MAX_DISTANCE;
        randomEndPosition.y = UnityEngine.Random.Range(0f,1f) * MAX_HEIGHT;

        Debug.DrawLine(startPosition, randomEndPosition, Color.red, 5f);

            startPoint.control = Instantiate(startPointPrefab, startPosition, default);
            startPoint.position = startPosition;
            endPoint.control = Instantiate(endPointPrefab,randomEndPosition, default);
            endPoint.position = randomEndPosition;

    }

    public void DestroyEverything()
    {

        DestroyObstacles();

        if(startPoint?.control != null)
            DestroyImmediate(startPoint?.control.gameObject);

        if(endPoint?.control != null)
            DestroyImmediate(endPoint?.control.gameObject);

    }

    void DestroyObstacles()
    {
                // DESTROY EVERYTHING THAT IS ON LIST BEFORE RESPAWN MORE
        foreach(var obstacles in platformPoints)
        {
            DestroyImmediate(obstacles);
        }

        // CLEAR THE LIST
        platformPoints.Clear();


    }
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

[System.Serializable]
public class ControlPoint
{
    public CheckPoint control;
    public Vector3 position;
}