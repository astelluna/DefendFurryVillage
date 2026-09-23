using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    //게임의 메인 메뉴를 표시하는 패널. 종류: 캐릭터,장비,상점,인게임 
    //어떤 버튼을 누르면 패널을 열어주고, 그런 안내하는 역할을 하도록 하자
    [SerializeField] Button characterButton;
    [SerializeField] Button equipmentButton;
    [SerializeField] Button shopButton;
    [SerializeField] Button ingmaeButton;

    EquipmentPanel equipmentPanel;

    private void Awake()
    {
        equipmentButton.onClick.AddListener(OnEquipmentButtonClick);
    }

    private void OnDestroy()
    {
        equipmentButton.onClick.RemoveAllListeners();
    }
    private void OnEquipmentButtonClick()
    {
        equipmentPanel.ShowPanel();
    }
    
}
