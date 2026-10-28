using TMPro;
using UnityEngine;

public class StagePanelInGame : MonoBehaviour
{
    //필요한거: 현재 스테이지가 몇스테이지인지, 전체 스테이지는 몇스테이지 있는지, 남은 몬스터는 몇마리 인지. ->
    [SerializeField] TextMeshProUGUI waveInfoText;
    [SerializeField] TextMeshProUGUI remainEnemyText;


    public void UpdateStageInfo(int curWave, int maxWave, int remainMonster)
    {
        waveInfoText.text = $"{curWave} / {maxWave}";
        remainEnemyText.text = remainMonster.ToString();
    }



}
