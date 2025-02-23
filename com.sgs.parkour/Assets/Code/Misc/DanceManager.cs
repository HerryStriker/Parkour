using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class DanceManager : MonoBehaviour
{
    [SerializeField] Animator[] animatorControllers;
    [SerializeField] int dancersCount = 3;
    const int DANCES = 10;

    void Start()
    {
        if(CharacterSelectionManager.Instance != null)
        {
            // IF IS TRUE MEANS THAT WE ARE AT CHARACTER SELECTION
            var selectionManager = CharacterSelectionManager.Instance;
            selectionManager.SelectionChanged += SelectionManager_SelectionChanged;
            Dance();
        }
        else
        {
            // MEANS WE ARE AT MAIN MENU
            Dance();
        }
    }

    const string DANCE = "Dance";
    private void SelectionManager_SelectionChanged(int obj)
    {
        var randomDance = DANCE + Random.Range(1, DANCES);
        animatorControllers[0].Play(randomDance);
    }

    void Dance()
    {
        var dances = GetDances(dancersCount);

        for (int i = 0; i < dancersCount; i++)
        {
            string danceName = DANCE + dances[i];
            var controller = animatorControllers[i];

            controller.Play(danceName);
        }
    }

    int[] GetDances(int n)
    {
        List<int> dances = new List<int>();
        do
        {
            var random = Random.Range(1, DANCES);
            if(!dances.Contains(random))
            {
                dances.Add(random);
            }
        }
        while (dances.Count < dancersCount);

        return dances.ToArray();
    }
}
