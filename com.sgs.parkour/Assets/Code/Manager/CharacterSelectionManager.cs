using System;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectionManager : MonoBehaviour
{
    [SerializeField] CharacterSelectMesh[] characterSelectMeshes;
    [SerializeField] int index = 0;

    public static CharacterSelectionManager Instance;
    public event Action<int> SelectionChanged;

    [Header("Model")]
    [SerializeField] Text modelName;
    [SerializeField] SkinnedMeshRenderer body;
    [SerializeField] SkinnedMeshRenderer hat;
    [SerializeField] SkinnedMeshRenderer special;

    [Header("Match")]
    [SerializeField] Button readyBtn;



    void Awake()
    {
        Instance = this;
        readyBtn.onClick.AddListener(StartMatch);
    }

    void StartMatch()
    {
        SceneManager.LoadScene("MatchTemp");

        Debug.LogWarning("MOUSE LOCK STATE MUST BE CHANGED HERE.");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Logic()
    {
        index = Mathf.Clamp(index, 0 , characterSelectMeshes.Length - 1);

            var selected = characterSelectMeshes[index];

            if(body.sharedMesh != selected.Mesh)
            {
                SelectionChanged?.Invoke(index);
            }

            modelName.text = !string.IsNullOrEmpty(selected.ModelName) ? selected.ModelName : "NotImplementedYet";
            body.sharedMesh = selected.Mesh != null ? selected.Mesh : null;
            hat.sharedMesh = selected.Hat != null ? selected.Hat : null;
            special.sharedMesh = selected.Special != null ? selected.Special : null;
    }

    public void Next()
    {
        index++;
        Logic();
    }

    public void Previous()
    {
        index--;
        Logic();
    }

}

[System.Serializable]
public class CharacterSelectMesh
{
    [SerializeField] string modelName;
    [SerializeField] Mesh mesh;
    [SerializeField] Mesh hat;
    [SerializeField] Mesh special;

    public bool HasSpecial
    {
        get {
            return special != null;
        }
    }

    public string ModelName => modelName;
    public Mesh Mesh => mesh;
    public Mesh Hat => hat;
    public Mesh Special => special;

}