using UnityEngine;
using System.Collections.Generic;


public static class Utils
{

    public static int[] GetRandomNoRepetitive(int n)
    {
        List<int> values = new List<int>();
        do
        {
            var r = Random(1, n);
            if(!values.Contains(r))
            {
                values.Add(r);
            }
        }
        while (values.Count < n);

        return values.ToArray();
    }

    public static float Random(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static int Random(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static Vector3 RandomPosition(Vector3 origin)
    {
        return UnityEngine.Random.insideUnitSphere + origin;
    }


    public static ModeManager GetModeManager()
    {
        if(ModeManager.Instance != null)
        {
            return ModeManager.Instance;
        }
        return null;
    }

    public static MatchScoreManager GetMatchScoreManager()
    {
        if(MatchScoreManager.Instance != null)
        {
            return MatchScoreManager.Instance;
        }
        return null;
    }
}