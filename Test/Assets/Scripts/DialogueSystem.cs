using System;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] Text targetText;
    [SerializeField] Text nameText;
    [SerializeField] Image portrait;
    DialogueContainer currentDialogue;
    int currentTextLine;
    int lengthOfDialogue;
    [Range(0f,1f)]
    [SerializeField] float visibleTextPercent;
    [SerializeField] float timePerLetter = 0.05f;
    float totalTimeToType,currentTime;
    string lineToShow;



    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            
            PushText();
        }
        TypeOutText();
    }

	private void TypeOutText()
    {
        if(visibleTextPercent >= 1f){return;}
        currentTime += Time.deltaTime;
        visibleTextPercent = currentTime / totalTimeToType;
        visibleTextPercent = Math.Clamp(visibleTextPercent,0, 1f);
        UpdateText();
    }
    void UpdateText(){
        int letterCount = (int)(lineToShow.Length * visibleTextPercent);
        targetText.text = lineToShow.Substring(0, letterCount);
    }

    private void PushText()
    {
        if(visibleTextPercent <1f){

            visibleTextPercent = 1;
            UpdateText();
            return;
        }

        //currentTextLine += 1;
        if(currentTextLine >= currentDialogue.DialogueLines.Count){

            Conclude();
        }
        else{
            CycleLine();
        }
    }

    void CycleLine(){
        lineToShow = currentDialogue.DialogueLines[currentTextLine];
            totalTimeToType = lineToShow.Length * timePerLetter;
            currentTime = 0f;
            visibleTextPercent = 0f;
            targetText.text = "";

        currentTextLine += 1;
    }

    public void Initialize(DialogueContainer dialogueContainer){
        Show(true);
        currentDialogue = dialogueContainer;
		DatabaseManager.instance.PopulateList(currentDialogue);
		currentTextLine = 0;
        CycleLine();
        UpdatePortrait();
    }

    private void UpdatePortrait()
    {
        portrait.sprite = currentDialogue.actor.portrait;
        nameText.text = currentDialogue.actor.Name;
    }

    private void Show(bool v)
    {
        gameObject.SetActive(v);
    }

    private void Conclude()
    {
        Debug.Log("The dialkogue has ended");
        Show(false);
    }
}
