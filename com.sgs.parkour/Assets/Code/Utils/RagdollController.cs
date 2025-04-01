using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    public event Action OnRagdollEnable;
    public event Action OnRagdollDisable;

    public List<RagdollObject> RagdollObject;
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody characterRigidbody;
    [SerializeField] Transform dummyTransform;

    [field: SerializeField] public bool IsEnable {get; private set;}
    [SerializeField, Range(0, 20)] float inpulseVelocity = 2f;
    void Awake()
    {
        characterRigidbody = GetComponentInParent<Rigidbody>();
        OnRagdollDisable += OnRagdollDisableCallback;

    }

    void Start()
    {
        RagdollObject.Clear();
        LoadColliders();
    }

    private void OnRagdollDisableCallback()
    {   
        foreach (var rd in RagdollObject)
        {
            rd.ResetPosition();
        }
    }

    public void EnableRagdoll(bool enable, bool inAir = default)
    {
        IsEnable = enable;

        var direction = inAir ? characterRigidbody.linearVelocity : Vector3.up * inpulseVelocity;

        foreach (var rd in RagdollObject)
        {
           rd.ResetVelocity(direction);
        }
        animator.enabled = !IsEnable;

        // FIRE THE EVENT BASED ON UPCOMING STATE FOR THE RAGDOLL
        Action ragdollAction = IsEnable ? OnRagdollEnable : OnRagdollDisable;
        ragdollAction?.Invoke();
    }

    public void LoadColliders()
    {

        var bodyColliders = transform.GetComponentsInChildren<Collider>();

        foreach (var collider in bodyColliders)
        {
            if(collider.TryGetComponent(out Rigidbody rb))
            {
                RagdollObject.Add(new RagdollObject(collider.name, rb, collider.transform));
            }
        }

            
    }
}


[System.Serializable] 
public class RagdollObject
{
    [SerializeField] string bodyName;
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] Transform transform;

    public RagdollObject(string colliderName, Rigidbody rigidbody, Transform transform)
    {
        this.bodyName = colliderName;
        this.rigidbody = rigidbody;
        this.transform = transform;
    }

    public void ResetVelocity(Vector3 direction = default)
    {
        rigidbody.linearVelocity = direction.magnitude > 0 ? direction : Vector3.zero;
        rigidbody.angularDamping = 0;
    }

    public void ResetPosition()
    {
        transform.localPosition = Vector3.zero;
    }
}


#if UNITY_EDITOR

    [CustomEditor(typeof(RagdollController))]
    public class RagdollControllerEdito : Editor
    {
        void Clear()
        {
            var ragdollController = (RagdollController)target;

            ragdollController.RagdollObject.Clear();
        }

        public override void OnInspectorGUI()
        {
            var ragdollController = (RagdollController)target;

            DrawDefaultInspector();

            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Load Colliders"))
            {
                Clear();
                ragdollController.LoadColliders();
            }
            if(GUILayout.Button("Clear Colliders"))
            {
                Clear();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Enable Ragdoll"))
            {
                ragdollController.EnableRagdoll(true);
            }
            if(GUILayout.Button("Disable Ragdoll"))
            {
                ragdollController.EnableRagdoll(false);
            }
            EditorGUILayout.EndHorizontal();
        }
    }


#endif