using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoPanel : MonoBehaviour
{
    //캐릭터 전체 초상화, 스킬정보, 장비?(가져올수있나), 이름, 
    CharacterDataSO characterDataSO;

    [SerializeField] Image characterImage;
    [SerializeField] TextMeshProUGUI characterNameText;
    
    [SerializeField] Button closeButton;

    //캐릭터 스텟 및 스킬 표시.
    [SerializeField] StatIconAll statIconAll;
    [SerializeField] SkillIconAll skillIconAll;

    //캐릭터 장비 설정 버튼
    [SerializeField] Button equipmentButton;
    [SerializeField] EquipmentMountPanel equipmentMountPanel;

    private void Awake()
    {
        closeButton.onClick.AddListener(OnClose);
        equipmentButton.onClick.AddListener(OnEquipClick);
    }
    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
    }
    private void OnClose()
    {
        this.gameObject.SetActive(false);
    }
    private void OnEquipClick()
    {
        equipmentMountPanel.SetDataAndShowPanel(characterDataSO);
    }

    public void SetAndShowPanel(CharacterDataSO data)
    {
        characterDataSO = data;
        //플레이어 정보 넣기
        characterImage.sprite = characterDataSO.CharacterBodySprite;
        characterNameText.text = characterDataSO.CharacterName;
        //스킬 및 스탯 정보 셋팅
        skillIconAll.SetIcon(characterDataSO);
        statIconAll.SetIcon(characterDataSO);

        this.gameObject.SetActive(true);
    }

    
}
