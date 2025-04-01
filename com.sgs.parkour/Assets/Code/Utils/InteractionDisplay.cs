using UnityEngine;

[System.Serializable]
public class InteractionDisplay
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Animator animator;
    
    public InteractionDisplay(InteractionDisplay interactionDisplay)
    {
        this.animator = interactionDisplay.animator;
        this.canvasGroup = interactionDisplay.canvasGroup;

        Enable(false);
    }

    public void Enable(bool enable)
    {
        if(canvasGroup == null) return;

        int normalized = enable ? 1 : 0;
        canvasGroup.alpha = normalized;

        if(animator != null )
        {
            string param = enable ? "Show" : "Hide";
            animator.SetTrigger(param);
        }
    }
}