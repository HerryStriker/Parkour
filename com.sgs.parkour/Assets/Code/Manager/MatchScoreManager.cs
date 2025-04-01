using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class MatchScoreManager : MonoBehaviour
{

    public static MatchScoreManager Instance {get; private set;}
    
    [SerializeField] Animator[] scoreDisplays;

    [SerializeField] List<Holder> playerScore;

    [SerializeField] BoxCollider endCollider;

    void Awake()
    {
        Instance = this;

    }

    void Start()
    {
        if(TryGetComponent(out RushMode rushMode))
        {
           endCollider = rushMode.GetEndBoxCollider();
        }
    }
    
    void Update()
    {
        var endTransform = endCollider.transform;
        var allRaycasts = Physics.OverlapBox(endTransform .position + endCollider.center, endCollider.size, endTransform .rotation, LayerMask.GetMask("Character"));
        // Debug.Log(allRaycasts.Length);
        foreach (var character in allRaycasts)
        {
            if(character.TryGetComponent(out Holder holderComponent))
            {
                PlayerFinish(holderComponent);
            }

        }
    }

    public void PlayerFinish(Holder playerHolder)
    {
        if(!playerScore.Contains(playerHolder))
        {
            playerScore.Add(playerHolder);
            ScoreDisplayLogic(playerHolder);
        }
    }

    void ScoreDisplayLogic(Holder playerHolder)
    {
        int playerScorePosition = playerScore.IndexOf(playerHolder);

       if(playerScorePosition == 0)
       {
            Show(ScoreType.WIN);
       }
       else
       {
            ShowText(ScoreType.PLACE);
       }
    }

    void Show(ScoreType type)
    {
        scoreDisplays[GetScoreTypeIndex(type)].SetTrigger("Show");
    }

    void ShowText(ScoreType type)
    {
        int index = GetScoreTypeIndex(type);
        Animator animator = scoreDisplays[index];
        
        Text label = animator.GetComponentInChildren<Text>();
        if(label != null)
        {
            string placeSuffix = index == 2 ? " st" : " th"; 

            label.text = $"{index}" + placeSuffix;
        }

        animator.SetTrigger("Show");
    }

    void Hide(ScoreType type)
    {
        scoreDisplays[GetScoreTypeIndex(type)].SetTrigger("Hide");
    }

    int GetScoreTypeIndex(ScoreType type)
    {
        string[] typeArr = Enum.GetNames(typeof(ScoreType));

        
        for (int i = 0; i < typeArr.Length; i++)
        {
            if(typeArr[i] == type.ToString())
            {
                return i;
            }
        }

        return -1;
    }

    enum ScoreType
    {
        WIN,
        LOSE,
        PLACE,
        NC,
    }
}
