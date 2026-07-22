using UnityEngine;
using UnityEngine.InputSystem;

public class NewMonoBehaviourScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;
        float move = 0f;
        if(kb.wKey.isPressed) move += 1f;
        if(kb.sKey.isPressed) move -= 1f;
    }
}
