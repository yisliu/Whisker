using UnityEngine;

public class choiceButton : MonoBehaviour
{
    [SerializeField] private string[] responseLines;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public void OnClick()
    {
        dialogueChoices.Instance.OptionChosen(responseLines);
    }
}
