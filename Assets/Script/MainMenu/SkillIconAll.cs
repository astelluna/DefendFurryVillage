using UnityEngine;

public class SkillIconAll : MonoBehaviour
{
    [SerializeField] SkillIcon basicSkillIcon;
    [SerializeField] SkillIcon specialSkillIcon;

    public void SetIcon(CharacterDataSO characterDataSO)
    {
        basicSkillIcon.SetIconInfo(characterDataSO.CharacterPrefab.GetBasicWeaponIcon(), characterDataSO.CharacterPrefab.GetBasicWeaponDesc());
        specialSkillIcon.SetIconInfo(characterDataSO.CharacterPrefab.GetSpecialWeaponIcon(), characterDataSO.CharacterPrefab.GetSpecialWeaponDesc());
    }
}
