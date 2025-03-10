using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Thumbnail3Controller : MonoBehaviour
{
    public string[] questionTexts;
    public Transform[] questionSpawnPositions;
    public GameObject questionSpawnPrefab;
    public Transform questionSpawnParent;
    public Button nextBTN;
    public int totalAnswerCount;
    int totalAnsweredCount = 0;
    List<GameObject> spawnedQuestionObjects;

    void Start()
    {
        spawnedQuestionObjects = new List<GameObject>();
        nextBTN.interactable = false;
        SpawnQuestion();
    }

#region ACTIVITY_1

    void SpawnQuestion(int spawnIndex = 0)
    {
        Debug.Log($"spawnIndex : {spawnIndex}");

        if(spawnIndex == questionTexts.Length) return;

        GameObject spawnedQuestion = Instantiate(questionSpawnPrefab, questionSpawnParent);
        spawnedQuestionObjects.Add(spawnedQuestion);
        spawnedQuestion.transform.GetComponentInChildren<TextMeshProUGUI>().text = questionTexts[spawnIndex];
        spawnedQuestion.AddComponent<Button>().onClick.AddListener(OnQuestionPanelClicked);
        Utilities.Instance.ANIM_Move(spawnedQuestion.transform, questionSpawnPositions[spawnIndex].position, callBack: () => {
            SpawnQuestion(++spawnIndex);
        });
    }

    void OnQuestionPanelClicked()
    {
        var selectedObj = EventSystem.current.currentSelectedGameObject;
        var clickedObjText = selectedObj.transform.GetComponentInChildren<TextMeshProUGUI>().text;
        string[] splitedText = clickedObjText.Split('+');
        bool sumIs10 = Int32.Parse(splitedText[0]) + Int32.Parse(splitedText[1]) == 10;

        if(sumIs10){
            Utilities.Instance.ANIM_CorrectScaleEffect(selectedObj.transform, callback: () => {
                selectedObj.transform.GetChild(0).gameObject.SetActive(true);
                totalAnsweredCount++;
                if(totalAnsweredCount == totalAnswerCount) nextBTN.interactable = true;
            });
        }
        else
            Utilities.Instance.ANIM_WrongShakeEffect(selectedObj.transform);
    }

    public void OnNextBtnClick()
    {
        ShrinkQuestion();
    }

    void ShrinkQuestion(int index = 1)
    {
        if(index == spawnedQuestionObjects.Count) return;

        Utilities.Instance.ANIM_Move(spawnedQuestionObjects[spawnedQuestionObjects.Count - index].transform, Vector3.zero, callBack: () => {
            spawnedQuestionObjects[spawnedQuestionObjects.Count - index].SetActive(false);
            ShrinkQuestion(++index);
        });
    }

#endregion
}
