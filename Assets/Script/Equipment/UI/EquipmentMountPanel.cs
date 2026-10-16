using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class EquipmentMountPanel : MonoBehaviour
{
    [Header("장비 슬롯")]
    [SerializeField] EquipmentSlotUI[] equipmentSlotArray;
    Dictionary<eEquipmentType, EquipmentSlotUI> equipmentSlotDict = new Dictionary<eEquipmentType, EquipmentSlotUI>();

    [Header("가지고있는 아이템이 생성될 위치, 아이템 정보 표시")]
    [SerializeField] Transform makeTransform; //스코롤뷰 위치 지정(프리팹이 생성될 위치)
    [SerializeField] EquipmentInfoPanel equipmentInfoPanel;
    [SerializeField] Button closeButton;
    List<EquipmentUI> equipmentUIList = new List<EquipmentUI>();



    //어떤 캐릭터인지 표시
    [SerializeField] Image characterImage;
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] StatIconAll statIconAll;
    //캐릭터 장비 표시.

    //아이템을 정렬하기 위한 버튼들 (추후 하자)
    [SerializeField] Button allItemShowButton;
    [SerializeField] Button weaponAlignButton;
    [SerializeField] Button armorAlignButton;
    [SerializeField] Button accessoriesAlignButton;

    //장비 해제 확인 패널
    [SerializeField] EquipmentUnEquipPanel equipmentUnEquipPanel;
    public event Action<EquipmentInstance> UnEquipRequest;

    //현재 선택중인 플레이어
    CharacterDataSO curCharacterData;

    private void Awake()
    {
        closeButton.onClick.AddListener(OnCloseButtonClick);
        equipmentInfoPanel.MountEquipmentRequest += MountEquipment;

        //장비 슬롯 설정
        foreach (EquipmentSlotUI item in equipmentSlotArray)
        {
            if(equipmentSlotDict.ContainsKey(item.EquipmentType))
            {
                MyLog.LogError("장비 슬롯이 중복이 되어있습니다. 확인해주세요");
                continue;
            }
            equipmentSlotDict[item.EquipmentType] = item;
            item.EquipmentSlotUIRequest += UnEquipmentMethod;
        }
    }
    private void MountEquipment(EquipmentInstance data)
    {
        EquipmentSlotUI ui = equipmentSlotDict[data.EquipmentDataSO.EquipmentType];
        //이미 슬롯에 장착했던 아이템이 있었는지 확인하기
        if(ui.HaveEquipmentData())
        {
            //데이터 처리 (인벤토리에 기존 아이템을 +1 하자.)
            EquipmentManager.Instance.AddEquipmentCount(data.EquipmentDataSO.EquipmentId, data.Level, 1);
        }
        ui.ClearData();

        //장비 장착
        CharacterEquipment characterEquipment = EquipmentManager.Instance.GetCharacterEquipmentInfo(curCharacterData.CharacterId);
        //장착 (데이터 설정)
        if (EquipmentManager.Instance.TryEquip(characterEquipment, data))
        {
            ui.SetData(data); //장착(UI에 표시)
        }
        
    }

    private void UnEquipmentMethod(EquipmentSlotUI equipmentSlotUI)
    {
        if (!equipmentSlotUI.HaveEquipmentData()) { return; }

        var slotData = equipmentSlotUI;
        equipmentUnEquipPanel.ShowPanel(() => UnEquipmentFunc(slotData));
    }
    private void UnEquipmentFunc(EquipmentSlotUI equipmentSlotUI)
    {
        //아이템 해제 처리

        //아이템 매니저에 해제하는 아이템 알려주기
        string id = equipmentSlotUI.equipmentInstanceData.EquipmentDataSO.EquipmentId;
        int level = equipmentSlotUI.equipmentInstanceData.Level;
        EquipmentManager.Instance.AddEquipmentCount(id, level, 1);

        //데이터 처리 끝나고 데이터 초기화 및 화면에 보이는 데이터 처리할거임
        equipmentSlotUI.ClearData();
        UpdateEquipmentInventory(); //갱신
        //TODO. 2. 클라우드 데이터 처리. 클라우드에서 캐릭터 장착 아이템 null 처리하고, 인벤토리에 그 아이템 넣을거임


    }

    //캐릭터 장비 표시하는 스코롤 뷰
    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
        equipmentInfoPanel.MountEquipmentRequest -= MountEquipment;

        foreach (EquipmentSlotUI item in equipmentSlotArray)
        {
            item.EquipmentSlotUIRequest -= UnEquipmentMethod;
        }
        ClearEquipmentUI();


    }
    private void OnCloseButtonClick()
    {
        this.gameObject.SetActive(false);
    }
    public void SetDataAndShowPanel(CharacterDataSO data)
    {
        curCharacterData = data;
        //캐릭터 정보 표시
        characterImage.sprite = curCharacterData.CharacterBodySprite;
        characterName.text = curCharacterData.CharacterName;
        statIconAll.SetIcon(curCharacterData);

        //스코롤뷰에 가지고있는 장비 표시
        UpdateEquipmentInventory();

        this.gameObject.SetActive(true);
    }
    private void ClearEquipmentUI()
    {
        foreach (var item in equipmentUIList)
        {
            item.OnClickUI -= ShowEquipmentInfo;
            EquipmentButtonSpawner.Instance.ReturnPrefabToPool(item);
        }
        equipmentUIList.Clear();
    }
    private void UpdateEquipmentInventory()
    {
        //하기전에 미리 다 clear
        ClearEquipmentUI();

        //생성을 할거임. 현재 아이템매니저에 있는 장비 갯수만큼 아래 인벤토리에 표시를 할것
        IReadOnlyDictionary<EquipmentKey, int> equipmentDict = EquipmentManager.Instance.EquipmentCountDict;
        foreach (KeyValuePair<EquipmentKey, int> item in equipmentDict)
        {
            string id = item.Key.equipmentId; //아이템별 id
            int level = item.Key.level;
            int count = item.Value;
            if (count <= 0) { continue; } //0이하이면 안함
            EquipmentInstance equipmentInstance = EquipmentManager.Instance.GetEquipmentInstance(id, level);
            EquipmentUI equipmentBuf = EquipmentButtonSpawner.Instance.GetPrefabByPool(makeTransform);
            equipmentUIList.Add(equipmentBuf);
            equipmentBuf.SetData(equipmentInstance);
            equipmentBuf.OnClickUI += ShowEquipmentInfo;
        }
    }


    private void ShowEquipmentInfo(EquipmentInstance data)
    {
        //선택한 장비의 정보를 표시한다.
        equipmentInfoPanel.SetAndShowPanel(data);
    }
}
