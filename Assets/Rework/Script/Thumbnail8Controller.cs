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
    public int[] answers;
    public string[] answerMatchColor;
    public GameObject[] colorObjs;
    public Image selectedColorDisplay;
    public GameObject counterObj;
    public GameObject activityCompleted;
    string _currentSelectedColorSTR;
    GameObject _currentColorObj;
    int totalyAnsweredCount = 0;

    void Start()
    {
        SetCurrentSelectedColor(cursorSprite[0]);
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
        Debug.Log($"_currentSelectedColorSTR :: {_currentSelectedColorSTR}");

        switch (selectedColor)
        {
            case "Red":
                SetCurrentSelectedColor(cursorSprite[1]);
                SetCursorSprite(customCursor[1]);
                break;
            case "Yellow":
                SetCurrentSelectedColor(cursorSprite[2]);
                SetCursorSprite(customCursor[2]);
                break;
            case "Green":
                SetCurrentSelectedColor(cursorSprite[3]);
                SetCursorSprite(customCursor[3]);
                break;
            case "NoColor":
                SetCurrentSelectedColor(cursorSprite[0]);
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
            if(matchObj.questionObj.GetComponentInChildren<TextMeshProUGUI>().text == selectedQues){
                if(matchObj.answer == _currentSelectedColorSTR){
                    totalyAnsweredCount++;
                    UpdateCounter();
                    switch(_currentSelectedColorSTR) {
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
                }else{
                    Debug.Log("Wrogn answer");
                }
                break;
            }
        }

        if(totalyAnsweredCount == matchingObjs.Length) Invoke(nameof(EnableActivityCompleted), 1.5f);
    }

    void SetCursorSprite(Texture2D cursorSprite) {
        Vector2 _cursorWidthHeight;
        _cursorWidthHeight = new Vector2(1, 1);
        Cursor.SetCursor(cursorSprite, _cursorWidthHeight, CursorMode.Auto);
    }

    void EnableActivityCompleted() => activityCompleted.SetActive(true);
    void UpdateCounter() => counterObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{totalyAnsweredCount}/{matchingObjs.Length}";
    void SetCurrentSelectedColor(Sprite colorSprite) => selectedColorDisplay.sprite = colorSprite;
    void EnableSelectedColor(GameObject _selectedColorObj) => _selectedColorObj.GetComponent<Image>().enabled = true;
    void DisablePrevSelectedColor() => _currentColorObj.GetComponent<Image>().enabled = false;
}

[System.Serializable]
public class MatchingObjects
{
    public GameObject questionObj;
    public GameObject answerObj;
    public string answer;
}