using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class Thumbnail8Controller : MonoBehaviour
{
    public MatchingObjects[] matchingObjs;
    public Sprite[] cursorSprite;
    public Texture2D[] customCursor;
    public int[] Ans;
    public string[] answerMatchColor;
    public GameObject[] colorObjs;
    public Image selectedColorDisplay;
    public GameObject counterObj;
    public AudioClip slideAudio;
    public TextMeshProUGUI selectedAnswer;
    public GameObject activityCompleted;
    string _currentSelectedColorSTR;
    GameObject _currentColorObj;
    int totalyAnsweredCount = 0;



    #region QA

    private int qIndex;
    public GameObject questionGO;
    public GameObject[] optionsGO;
    public Dictionary<string, Component> additionalFields;
    Component question;
    Component[] options;
    Component[] answers;

    #endregion



    void Start()
    {

        #region DataSetter
        //Main_Blended.OBJ_main_blended.levelno = 3;
        QAManager.instance.UpdateActivityQuestion();
        qIndex = 0;
        GetData(qIndex);
        GetAdditionalData();
        AssignData();
        #endregion


        AudioManager.PlayAudio(slideAudio);
        SetCurrentSelectedColor("", Color.white, Color.white);
        SetCursorSprite(customCursor[0]);
        UpdateCounter();
        _currentColorObj = colorObjs[colorObjs.Length - 1];
        _currentSelectedColorSTR = "NoColor";
        EnableSelectedColor(_currentColorObj);
    }


    public void OnColorClicked()
    {
        var _selectedObj = EventSystem.current.currentSelectedGameObject;
        DisablePrevSelectedColor();
        EnableSelectedColor(_selectedObj);
        _currentColorObj = _selectedObj;
        string selectedColor = _selectedObj.transform.GetChild(0).name;
        _currentSelectedColorSTR = selectedColor;
        Color selectedColorCode = _selectedObj.transform.GetChild(0).GetComponent<Image>().color;
        Debug.Log($"_currentSelectedColorSTR :: {_currentSelectedColorSTR}");
        Debug.Log(_selectedObj.name, _selectedObj);
        Debug.Log(selectedColorCode);


        switch (selectedColor)
        {
            case "Red":
                SetCurrentSelectedColor("10", Color.white, selectedColorCode);
                SetCursorSprite(customCursor[1]);
                break;
            case "Yellow":
                SetCurrentSelectedColor("5", Color.black, selectedColorCode);
                SetCursorSprite(customCursor[2]);
                break;
            case "Green":
                SetCurrentSelectedColor("20", Color.white, selectedColorCode);
                SetCursorSprite(customCursor[3]);
                break;
            case "NoColor":
                SetCurrentSelectedColor("", Color.white, selectedColorCode);
                SetCursorSprite(customCursor[0]);
                break;
        }
    }


    public void OnQuestionClicked()
    {
        var _selectedObj = EventSystem.current.currentSelectedGameObject;
        string selectedQues = _selectedObj.GetComponentInChildren<TextMeshProUGUI>().text;
        foreach (var matchObj in matchingObjs)
        {
            if (matchObj.questionObj.GetComponentInChildren<TextMeshProUGUI>().text == selectedQues)
            {
                if (matchObj.answer == _currentSelectedColorSTR)
                {
                    totalyAnsweredCount++;
                    UpdateCounter();
                    switch (_currentSelectedColorSTR)
                    {
                        case "Red":
                            matchObj.answerObj.GetComponent<Image>().color = Color.red;
                            break;
                        case "Yellow":
                            matchObj.answerObj.GetComponent<Image>().color = Color.yellow;
                            break;
                        case "Green":
                            matchObj.answerObj.GetComponent<Image>().color = Color.green;
                            break;
                    }
                    _selectedObj.SetActive(false);
                }
                else
                {
                    Debug.Log("Wrogn answer");
                }
                break;
            }
        }

        if (totalyAnsweredCount == matchingObjs.Length) Invoke(nameof(EnableActivityCompleted), 1.5f);
    }


    void SetCursorSprite(Texture2D cursorSprite) => Cursor.SetCursor(cursorSprite, Vector2.one, CursorMode.Auto);


    void EnableActivityCompleted() => activityCompleted.SetActive(true);


    void UpdateCounter() => counterObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{totalyAnsweredCount}/{matchingObjs.Length}";


    void SetCurrentSelectedColor(string displayText, Color fontColor, Color displayColor)
    {
        selectedAnswer.text = displayText;
        selectedAnswer.color = fontColor;
        selectedColorDisplay.color = displayColor;
    }


    void EnableSelectedColor(GameObject _selectedColorObj) => _selectedColorObj.GetComponent<Image>().enabled = true;


    void DisablePrevSelectedColor() => _currentColorObj.GetComponent<Image>().enabled = false;


    void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Utilities.Instance.StopAllSounds();
    }


    void OnDestroy()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Utilities.Instance.StopAllSounds();
    }



    #region QA

    int GetOptionID(string selectedOption)
    {
        for (int i = 0; i < options.Length; i++)
        {
            if (options[i].text == selectedOption)
            {
                return options[i].id;
            }
        }
        return -1;
    }

    bool CheckOptionIsAns(Component option)
    {
        for (int i = 0; i < answers.Length; i++)
        {
            if (option.text == answers[i].text) { return true; }
        }
        return false;
    }

    void GetData(int questionIndex)
    {
        Debug.Log(">>>>>" + questionIndex);
        question = QAManager.instance.GetQuestionAt(0, questionIndex);
        //if(question != null){
        options = QAManager.instance.GetOption(0, questionIndex);
        answers = QAManager.instance.GetAnswer(0, questionIndex);
        // }
    }

    void GetAdditionalData()
    {
        additionalFields = QAManager.instance.GetAdditionalField(0);
    }

    void AssignData()
    {
        // Custom code
        for (int i = 0; i < optionsGO.Length; i++)
        {
            optionsGO[i].GetComponent<Image>().sprite = options[i]._sprite;
            optionsGO[i].tag = "Untagged";
            Debug.Log(optionsGO[i].name, optionsGO[i]);
            if (CheckOptionIsAns(options[i]))
            {
                optionsGO[i].tag = "answer";
            }
        }
        // answerCount.text = "/"+answers.Length;
    }

    #endregion


}






[System.Serializable]
public class MatchingObjects
{
    public GameObject questionObj;
    public GameObject answerObj;
    public string answer;
}