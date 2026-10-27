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
        equipmentMountPanel.CharacterStatChangeRequest += UpdateCharacterStat;
    }
    private void UpdateCharacterStat()
    {
        CharacterStat characterTotalStat = CharacterManager.Instance.GetTotalCharacterStat(characterDataSO);
        statIconAll.SetIcon(characterTotalStat);
    }
    private void UpdateCharacterStat_basic()
    {
        //캐릭터를 가지고있지 않으면 그냥 기본 캐릭터 스텟을 화면에 보여준다.
        CharacterStat characterStat =  CharacterManager.Instance.GetBasicCharacterStat(characterDataSO);
        statIconAll.SetIcon(characterStat);
    }
    private void OnDisable()
    {
        OnClose();
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
        equipmentMountPanel.CharacterStatChangeRequest -= UpdateCharacterStat;
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
        
        //만약 캐릭터를 가지고있지않다면? 장비 설정 버튼 비활성화
        if(CharacterManager.Instance.HaveCharacter(characterDataSO.CharacterId))
        {
            //캐릭터를 가지고있음
            UpdateCharacterStat();
            equipmentButton.interactable = true;
            
        }
        else
        {
            UpdateCharacterStat_basic();
            equipmentButton.interactable = false;
        }
        this.gameObject.SetActive(true);
    }

    
}
