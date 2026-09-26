using UnityEngine;
using UnityEngine.UI;

public interface IMainMenuPanel
{
    public void ShowPanel(bool isShow);
}

public class MainPanelChanger : MonoBehaviour
{    
    //어떤 버튼을 누르면 패널을 열어주고, 그런 안내하는 역할을 하도록 하자
    [SerializeField] Button characterButton;
    [SerializeField] Button equipmentButton;
    [SerializeField] Button mainMenuButton;
    [SerializeField] Button shopButton;
    [SerializeField] Button characterSelectButton;

    //게임의 메인 메뉴를 표시하는 패널. 종류: 캐릭터,장비,상점,인게임 
    [SerializeField] CharacterShowPanel characterShowPanel;
    [SerializeField] EquipmentPanel equipmentPanel;
    [SerializeField] MainMenuPanel mainMenuPanel;
    [SerializeField] CharacterSelectPanel characterSelectPanel;

    IMainMenuPanel[] mainMenuPanelArray;

    private void Awake()
    {
        //메인화면에서 쓰는 패널들 모아둠 (한번에 하나의 패널만 껏다 켜기 위해서)
        mainMenuPanelArray = new IMainMenuPanel[]
        {
            characterShowPanel,
            equipmentPanel,
            mainMenuPanel,
            characterSelectPanel
        };
        //버튼 이벤트들 추가
        characterButton.onClick.AddListener(ShowCharacterShowPanel);
        equipmentButton.onClick.AddListener(ShowEquipmentPanel);
        mainMenuButton.onClick.AddListener(ShowMainMenuPanel);
        characterSelectButton.onClick.AddListener(ShowCharacterSelectPanel);
    }

    private void OnDestroy()
    {
        equipmentButton.onClick.RemoveAllListeners();
    }

    private void ShowMainMenuPanel(IMainMenuPanel selectedPanel)
    {
        foreach(var item in mainMenuPanelArray)
        {
            if(item == selectedPanel)
            {
                item.ShowPanel(true);
            }
            else
            {
                item.ShowPanel(false);
            }
        }
    }

    private void ShowCharacterShowPanel()
    {
        ShowMainMenuPanel(characterShowPanel);
    }
    private void ShowEquipmentPanel()
    {
        ShowMainMenuPanel(equipmentPanel);
    }
    private void ShowMainMenuPanel()
    {
        ShowMainMenuPanel(mainMenuPanel);
    }
    private void ShowCharacterSelectPanel()
    {
        ShowMainMenuPanel(characterSelectPanel);
    }


}
