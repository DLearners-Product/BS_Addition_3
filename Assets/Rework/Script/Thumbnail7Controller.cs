using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Thumbnail7Controller : MonoBehaviour
{
    public Transform numberBlockParent;
    public TextMeshProUGUI upperTens, upperOnes, lowerTens, lowerOnes, tensPower;
    public Transform counterObj;
    public string[] _quesitons;
    public GameObject[] answerObjs;
    public Transform boardObj;
    public Transform startPoint, endPoint;
    public GameObject activityCompleted;
    int currentIndex = 0;
    Vector3 boardCurrentPosition;
    string[] currentQuesSplit;

    void Start()
    {
        boardCurrentPosition = boardObj.transform.position;
        ShowNextQues();
        UpdateCounter();
        DisableActivityCompleted();
    }

    private void OnEnable() {
        ImageDropSlot.onDropInSlot += OnObjectDrop;
    }

    bool ShowNextQues()
    {
        if(currentIndex == _quesitons.Length) { EnableActivityCompleted(); return false; }

        string currentQues = _quesitons[currentIndex];
        currentQuesSplit = currentQues.Split('+');
        upperTens.text = currentQuesSplit[0].Trim()[0].ToString();
        upperOnes.text = currentQuesSplit[0].Trim()[1].ToString();

        lowerTens.text = currentQuesSplit[1].Trim()[0].ToString();
        lowerOnes.text = currentQuesSplit[1].Trim()[1].ToString();

        tensPower.text = "";
        answerObjs[0].GetComponentInChildren<TextMeshProUGUI>().text = "";
        answerObjs[1].GetComponentInChildren<TextMeshProUGUI>().text = "";
        return true;
    }

    void UpdateCounter() => counterObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{currentIndex + 1}/{_quesitons.Length}";

    void OnObjectDrop(GameObject draggedObject, GameObject dropSlotObject)
    {
        dropSlotObject.GetComponentInChildren<TextMeshProUGUI>().text = draggedObject.GetComponentInChildren<TextMeshProUGUI>().text;

        var tensValue = answerObjs[0].GetComponentInChildren<TextMeshProUGUI>().text;
        var onesValue = answerObjs[1].GetComponentInChildren<TextMeshProUGUI>().text;

        bool tensValueCheck = tensValue == "";
        bool onesValueCheck = onesValue == "";

        if(dropSlotObject.name.Equals("Ones") && (int.Parse(upperOnes.text) + int.Parse(lowerOnes.text) >= 10))
        {
            tensPower.text = "+1";
        }

        if(tensValueCheck || onesValueCheck) return;

        bool rightAnswer = (int.Parse(currentQuesSplit[0].Trim()) + int.Parse(currentQuesSplit[1].Trim())) == int.Parse($"{tensValue}{onesValue}"); 

        if(rightAnswer)
        {
            Invoke(nameof(MoveBoardLeft), 1f);
        }else{
            Utilities.Instance.ANIM_WrongShakeEffect(answerObjs[0].transform, callback: () => {
                answerObjs[0].GetComponentInChildren<TextMeshProUGUI>().text = "";
            });
            Utilities.Instance.ANIM_WrongShakeEffect(answerObjs[1].transform, callback: () => {
                answerObjs[1].GetComponentInChildren<TextMeshProUGUI>().text = "";
                tensPower.text = "";
            });
        }
    }

    void DisableActivityCompleted() => activityCompleted.SetActive(false);
    void EnableActivityCompleted() => activityCompleted.SetActive(true);

    void MoveBoardLeft()
    {
        Utilities.Instance.ANIM_Move(boardObj, new Vector3(endPoint.position.x, boardObj.position.y, endPoint.position.z), callBack: () => {
            boardObj.transform.position = new Vector3(startPoint.position.x, boardObj.position.y, startPoint.position.z);

            currentIndex++;

            if(ShowNextQues())
                Utilities.Instance.ANIM_Move(boardObj, new Vector3(boardCurrentPosition.x, boardObj.position.y, boardCurrentPosition.z), callBack : UpdateCounter);
        });
    }
}
