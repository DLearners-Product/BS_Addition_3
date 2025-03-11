using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Thumbnail3Controller : MonoBehaviour
{
    [Header("Activity 1")]
    public string[] questionTexts;
    public Transform[] questionSpawnPositions;
    public GameObject questionSpawnPrefab;
    public Transform questionSpawnParent;
    public Button nextBTN;
    public int totalAnswerCount;
    int totalAnsweredCount = 0;
    List<GameObject> spawnedQuestionObjects;

    [Header("Activity 2")]
    public Transform position1;
    public Transform position2;
    public Transform answerSpawnPosition;
    public Transform marketShaft;
    public Transform basket1, basket2;
    public Transform vegetableSpawnPoint1, vegetableSpawnPoint2;
    public GameObject vegetableSpawnPrefab;
    public Sprite[] vegetableSprites;
    public string[] questionSTR;
    public GameObject symbol1, symbol2;
    public Transform answerContainPanel;
    public GameObject[] countDisplayPanels;
    int currentIndex = 0;

    void Start()
    {
        spawnedQuestionObjects = new List<GameObject>();
        nextBTN.interactable = false;
        // SpawnQuestion();
        ResetActivity2();
        OpenShopShaft();
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

#region ACTIVITY_2

    void ResetActivity2()
    {
        Utilities.Instance.ANIM_ShrinkObject(symbol1.transform, 0f);
        Utilities.Instance.ANIM_ShrinkObject(symbol2.transform, 0f);
        for (int i = 0; i < countDisplayPanels.Length; i++)
        {
            Utilities.Instance.ANIM_ShrinkObject(countDisplayPanels[i].transform, 0f);
        }
        Utilities.Instance.ANIM_ShrinkOnPosition(marketShaft.transform, new Vector3(1, 0, 1), 0f);
    }

    void OpenShopShaft()
    {
        Utilities.Instance.ANIM_BounceEffect(marketShaft, callback : () => {
            MoveBasket(basket1, position1);
        });
    }

    void MoveBasket(Transform basketObj, Transform moveTransform, int dropCount = 0)
    {
        if(dropCount == 2) { StartSpawningVegetables(); return;}
        Utilities.Instance.ANIM_Move(basketObj, moveTransform.position, callBack: () => {
            MoveBasket(basket2, position2, ++dropCount);
        });
    }

    void StartSpawningVegetables()
    {
        int[] questionCount = GetQuestionInt();
        StartCoroutine(SpawnVegetables(vegetableSpawnPoint1, basket1, questionCount[0], () => {
            StartCoroutine(SpawnVegetables(vegetableSpawnPoint2, basket2, questionCount[1], () => {
                SpawnSymbols(symbol1.transform);
            }));
        }));
    }

    IEnumerator SpawnVegetables(Transform spawnParent, Transform basketObj, int spawnCount, Action func=null)
    {
        int count = 0;
        while (count < spawnCount)
        {
            var spawnedVegetable = Instantiate(vegetableSpawnPrefab, spawnParent);
            spawnedVegetable.transform.position = new Vector3(
                spawnParent.position.x + UnityEngine.Random.Range(-1f, 1f),
                spawnParent.position.y,
                spawnParent.position.z
            );
            spawnedVegetable.GetComponent<Image>().sprite = vegetableSprites[currentIndex];
            spawnedVegetable.GetComponent<Image>().preserveAspect = true;

            var endPosition = new Vector3(
                spawnedVegetable.transform.position.x,
                position1.position.y,
                position1.position.z
            );

            spawnedVegetable.transform.parent = basketObj;
            spawnedVegetable.transform.SetSiblingIndex(1);

            Utilities.Instance.ANIM_MoveWithRandomRotate(spawnedVegetable.transform, endPosition);
            yield return new WaitForSeconds(0.5f);
            count++;
        }
        if(func != null) func();
    }

    void SpawnSymbols(Transform symbolTransform, int index = 0)
    {
        if(index == 2) { SpawnAnswerPanel(); return; }
        Utilities.Instance.ANIM_ShowBounceNormal(symbolTransform, callback: () => {
            SpawnSymbols(symbol2.transform, ++index);
        });
    }

    void SpawnAnswerPanel()
    {
        Utilities.Instance.ANIM_Move(answerContainPanel, answerSpawnPosition.position);
    }

    int[] GetQuestionInt()
    {
        int[] tmp = new int[2];
        string[] quesSplit = questionSTR[currentIndex].Split('+');

        tmp[0] = Int32.Parse(quesSplit[0]);
        tmp[1] = Int32.Parse(quesSplit[1]);

        return tmp;
    }

#endregion
}
