using UnityEngine;

public class StatIconAll : MonoBehaviour
{
    //캐릭터 스텟표시.
    [SerializeField] StatIcon attackStatIcon;
    [SerializeField] StatIcon criticalPercentStatIcon;
    [SerializeField] StatIcon criticalRatioStatIcon;
    [SerializeField] StatIcon armorPenetrateStatIcon;


    public void SetIcon(CharacterDataSO characterDataSO)
    {
        attackStatIcon.SetText(characterDataSO.CharacterBaseStat.attack.ToString());
        criticalPercentStatIcon.SetText(characterDataSO.CharacterBaseStat.criticalPercent.ToString());
        criticalRatioStatIcon.SetText(characterDataSO.CharacterBaseStat.criticalDamageRate.ToString());
        armorPenetrateStatIcon.SetText(characterDataSO.CharacterBaseStat.armorPenetration.ToString());
    }
}
