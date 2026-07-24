using UnityEngine;
using System.Collections;
using TMPro;

public class customerDialogue : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static customerDialogue Instance;
    public TextMeshProUGUI textComponent;
    public float textSpeed = 0.05f;
    private string[] currentLines;
    private int index;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == currentLines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = currentLines[index];
            }
        }
    }

    public void StartDialogue(string[] lines)
    {
        currentLines = lines;
        index = 0;
        gameObject.SetActive(true);
        textComponent.text = string.Empty;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in currentLines[index])
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < currentLines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
