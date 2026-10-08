using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameResultPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] Button exitButton;

    const string victoryText = "승리!!";
    const string defectText = "패배!!";

    string uiText;


    private void Awake()
    {
        exitButton.onClick.AddListener(OnExitButtonClick);
    }
    private void OnDestroy()
    {
        exitButton.onClick.RemoveAllListeners();
    }
    private void OnExitButtonClick()
    {
        this.gameObject.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneController.Main);
    }

    public void ShowGameResultPanel(bool isVictory)
    {
        if(isVictory) { uiText = victoryText; }
        else { uiText = defectText; }
        resultText.text = uiText;
        this.gameObject.SetActive(true);

    }

}
