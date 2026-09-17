using UnityEngine;

public sealed class AnimationStateLabel : MonoBehaviour
{
    public Animator target;
    public TextMesh label;
    public string[] states;
    string previous;
    void LateUpdate()
    {
        if (target == null || label == null) return;
        AnimatorStateInfo info = target.GetCurrentAnimatorStateInfo(0);
        foreach (string state in states)
            if (info.IsName(state))
            {
                if (previous != state)
                {
                    label.text = state.Replace("Walking", "WALK / ").Replace("Scared", "SCARED / ").ToUpperInvariant();
                    previous = state;
                }
                return;
            }
    }
}
