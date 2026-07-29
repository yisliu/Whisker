/*
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class dialogueOptions
{
    public string buttonLabel;
    public string[] options;
}

public class dialogueChoices : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static dialogueChoices Instance {get; private set;}
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private string nextScene;
    [SerializeField] private dialogueOptions[] choices;
    [SerializeField] private string[] triggerLines;
    private readonly List<Button> activeButton = new List<Button>();

    void Awake()
    {
        Instance = this;
        choicePanel.SetActive(false);
    }

    void Start()
    {
        if (customerDialogue.Instance != null && triggerLines.Length > 0)
        {
            customerDialogue.Instance.onDialogueComplete = () => DisplayChoices(choices);
            customerDialogue.Instance.StartDialogue(triggerLines);
        }
    }

    public void DisplayChoices(dialogueOptions[] options)
    {
        foreach (var b in activeButton)
        {
            Destroy(b.gameObject);
        }

        activeButton.Clear();
        foreach (var option in options)
        {
            Button bt = Instantiate(buttonPrefab, buttonContainer);
            bt.GetComponentInChildren<TextMeshProUGUI>().text = option.buttonLabel;
            dialogueOptions captured = option;
            bt.onClick.AddListener(()=>OptionChosen(captured));
            activeButton.Add(bt);
        }

        choicePanel.SetActive(true);
    }

    private void OptionChosen(dialogueOptions option)
    {
        choicePanel.SetActive(false);
        if (customerDialogue.Instance != null)
        {
            customerDialogue.Instance.onDialogueComplete = () => SceneManager.LoadScene(nextScene);
            customerDialogue.Instance.StartDialogue(option.options);
        }
        else
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
*/

using UnityEngine;
using UnityEngine.SceneManagement;
public class dialogueChoices : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static dialogueChoices Instance { get; private set; }
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private string nextScene;
    [SerializeField] private string[] triggerLines;

    void Awake()
    {
        Instance = this;
        choicePanel.SetActive(false);
    }

    void Start()
    {
        if (customerDialogue.Instance != null && triggerLines.Length > 0)
        {
            customerDialogue.Instance.onDialogueComplete = () => choicePanel.SetActive(true);
            customerDialogue.Instance.StartDialogue(triggerLines);
        }
    }

    public void OptionChosen(string[] responseLines)
    {
        choicePanel.SetActive(false);
        if (customerDialogue.Instance != null)
        {
            customerDialogue.Instance.onDialogueComplete = () => SceneManager.LoadScene(nextScene);
            customerDialogue.Instance.StartDialogue(responseLines);
        }
        else
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}