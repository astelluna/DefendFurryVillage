using System.Collections.Generic;
using UnityEngine;

public class EquipmentPanel : MonoBehaviour
{
    [SerializeField] EquipmentUI equipmentUIPrefab;
    [SerializeField] Transform makeTransform; //스코롤뷰 위치 지정(프리팹이 생성될 위치)
    [SerializeField] EquipmentInfoPanel equipmentInfoPanel;

    List<EquipmentUI> equipmentUIList = new List<EquipmentUI>();

    private void OnDestroy()
    {
        foreach(var item in equipmentUIList)
        {
            item.OnClickUI -= ShowEquipmentInfo;
        }
        equipmentUIList.Clear();
    }

    public void ShowPanel()
    {
        this.gameObject.SetActive(true);
        //생성을 할거임. 현재 아이템매니저에 있는 장비 갯수만큼 아래 인벤토리에 표시를 할것
        equipmentUIList.Clear();
        //foreach문 돌면서 장비 확인
        IReadOnlyDictionary<string, EquipmentData> k = ItemManager.Instance.EquipmentHaveDataDict; //전체 장비에 대한 정보를 가져왔음.
        foreach(KeyValuePair<string, EquipmentData> item in k)
        {
            var data = item.Value; //현재 장비에 대한 정보. 레벨 데이터도 안에 들어있음.
            IReadOnlyDictionary<int, EquipmentLevelData> leveldataDict = data.EquipmentLevelDataDict;
            foreach(KeyValuePair<int, EquipmentLevelData> levelItem in leveldataDict)
            {
                EquipmentUI equipmentBuf = Instantiate(equipmentUIPrefab, makeTransform);
                equipmentBuf.SetData(levelItem.Value);
                equipmentBuf.OnClickUI += ShowEquipmentInfo;
            }
        }
    }
    private void ShowEquipmentInfo(EquipmentLevelData data)
    {
        //선택한 장비의 정보를 표시한다.
        equipmentInfoPanel.SetAndShowPanel(data.EquipmentDataSO.name, data.GetEquipmentDescription(), data.Level.ToString());
    }

}
