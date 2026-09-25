using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoPanel : MonoBehaviour
{
    //캐릭터 전체 초상화, 스킬정보, 장비?(가져올수있나), 이름, 
    CharacterDataSO characterDataSO;

    [SerializeField] Image characterImage;
    [SerializeField] TextMeshProUGUI characterNameText;
    [SerializeField] SkillIcon basicSkillIcon;
    [SerializeField] SkillIcon specialSkillIcon;
    [SerializeField] Button closeButton;

    //캐릭터 스텟표시.
    [SerializeField] StatIcon attackStatIcon;
    [SerializeField] StatIcon criticalPercentStatIcon;
    [SerializeField] StatIcon criticalRatioStatIcon;
    [SerializeField] StatIcon armorPenetrateStatIcon;

    private void Awake()
    {
        closeButton.onClick.AddListener(OnClose);
    }
    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
    }
    private void OnClose()
    {
        this.gameObject.SetActive(false);
    }

    public void SetAndShowPanel(CharacterDataSO data)
    {
        characterDataSO = data;
        //플레이어 정보 넣기
        characterImage.sprite = characterDataSO.CharacterBodySprite;
        characterNameText.text = characterDataSO.CharacterName;
        //스킬 정보 셋팅
        basicSkillIcon.SetIconInfo(characterDataSO.CharacterPrefab.GetBasicWeaponIcon(), characterDataSO.CharacterPrefab.GetBasicWeaponDesc());
        specialSkillIcon.SetIconInfo(characterDataSO.CharacterPrefab.GetSpecialWeaponIcon(), characterDataSO.CharacterPrefab.GetSpecialWeaponDesc());
        //스탯 정보 설정
        attackStatIcon.SetText(characterDataSO.attack.ToString());
        criticalPercentStatIcon.SetText(characterDataSO.criticalPercent.ToString());
        criticalRatioStatIcon.SetText(characterDataSO.criticalDamageRate.ToString());
        armorPenetrateStatIcon.SetText(characterDataSO.armorPenetration.ToString());
        

        this.gameObject.SetActive(true);
    }

    
}
