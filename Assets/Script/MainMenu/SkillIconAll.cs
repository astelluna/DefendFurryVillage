using UnityEngine;

public class SkillIconAll : MonoBehaviour
{
    [SerializeField] SkillIcon basicSkillIcon;
    [SerializeField] SkillIcon specialSkillIcon;

    public void SetIcon(CharacterDataSO characterDataSO)
    {
        basicSkillIcon.SetIconInfo(characterDataSO.BasicWeaponDataSO.WeaponIcon, characterDataSO.BasicWeaponDataSO.WeaponDescription);
        specialSkillIcon.SetIconInfo(characterDataSO.SpecialWeaponDataSO.WeaponIcon, characterDataSO.SpecialWeaponDataSO.WeaponDescription);
    }
    public void Clear()
    {
        basicSkillIcon.Clear();
        specialSkillIcon.Clear();
    }
}
