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
        attackStatIcon.SetText(characterDataSO.attack.ToString());
        criticalPercentStatIcon.SetText(characterDataSO.criticalPercent.ToString());
        criticalRatioStatIcon.SetText(characterDataSO.criticalDamageRate.ToString());
        armorPenetrateStatIcon.SetText(characterDataSO.armorPenetration.ToString());
    }
}
