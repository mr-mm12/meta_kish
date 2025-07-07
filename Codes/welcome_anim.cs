using UnityEngine;

public class welcome_anim : MonoBehaviour
{
    public Animator animator;  // Reference to the Animator component

    void Start()
    {
        // Invoke PlayAnimation method after a delay of 3.5 seconds
        Invoke("PlayAnimation", 3.5f);
    }

    void PlayAnimation()
    {
        // Play the animation state named "Sitting Talking" on the base layer (layer 0)
        animator.Play("Sitting Talking", 0);
    }
}
