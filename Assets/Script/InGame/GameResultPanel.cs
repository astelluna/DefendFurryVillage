using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameResultPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] Button exitButton;

    //보상UI
    [SerializeField] RewardItemUI rewardItemUIPrefab;
    [SerializeField] Transform makePosition;

    const string victoryText = "승리!!";
    const string defectText = 
        "패배!! \n" +
        "장비를 장착, 강화하거나 상점에서 캐릭터를 구매해보세요";
    string uiText;

    List<RewardItemUI> rewardItemUiList = new List<RewardItemUI>();


    private void Awake()
    {
        exitButton.onClick.AddListener(OnExitButtonClick);
    }
    private void OnDestroy()
    {
        exitButton.onClick.RemoveAllListeners();
        Init();
    }
    private void OnExitButtonClick()
    {
        //this.gameObject.SetActive(false); //어짜피 씬 이동할거니까 닫지말자.
        Time.timeScale = 1f;
        Init();
        SceneManager.LoadScene(SceneController.Main);
    }

    public void ShowGameResultPanel(bool isVictory, RewardDataSO rewardData)
    {
        if(isVictory) { uiText = victoryText; }
        else { uiText = defectText; }
        resultText.text = uiText;

        if(rewardData != null)
        {
            //새로 생성해주자.

            foreach(var item in rewardData.RewardItemList)
            {
                RewardItemUI rewardItemUIBuf = Instantiate(rewardItemUIPrefab, makePosition);
                rewardItemUIBuf.SetData(item);
                rewardItemUiList.Add(rewardItemUIBuf);
            }
        }

        this.gameObject.SetActive(true);

    }
    private void Init()
    {
        foreach(var item in rewardItemUiList)
        {
            Destroy(item.gameObject);
        }
        rewardItemUiList.Clear();
    }

}
