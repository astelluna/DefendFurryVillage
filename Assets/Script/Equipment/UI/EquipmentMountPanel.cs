using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentMountPanel : MonoBehaviour
{
    [Header("장비 슬롯")]
    [SerializeField] EquipmentSlotUI[] equipmentSlotArray;
    Dictionary<eEquipmentType, EquipmentSlotUI> equipmentSlotDict;

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

    bool isInitalized = false;

    private async void MountEquipment(EquipmentInstance data)
    {
        //어떤 행동을 해달라고 위임을 해버리자. 장비 장착 행동이니까 EquipmentManager에
        bool isSuccess = await EquipmentManager.Instance.TryEquipAsync(curCharacterData, data);
        if(!isSuccess)
        {
            MyLog.LogError("장비 장착 실패. ");
            return;
        }

        //갱신
        UpdateEquipmentInventory();
        UpdateEquipmentUI();
    }

    private void UnEquipmentMethod(EquipmentSlotUI equipmentSlotUI)
    {
        if (!equipmentSlotUI.HaveEquipmentData()) { return; }

        var slotData = equipmentSlotUI;
        equipmentUnEquipPanel.ShowPanel(() => UnEquipmentFunc(slotData));
    }
    private async Task UnEquipmentFunc(EquipmentSlotUI equipmentSlotUI)
    {
        //1. db데이터 설정 및 런타임 데이터 갱신을 매니저에 위임
        bool isSuccessDataSet = await EquipmentManager.Instance.TryUnEquipAsync(curCharacterData, equipmentSlotUI.equipmentInstanceData);

        if (!isSuccessDataSet) { return; }


        //2. UI갱신
        //데이터 처리 끝나고 데이터 초기화 및 화면에 보이는 데이터 갱신
        //equipmentSlotUI.ClearData();
        UpdateEquipmentInventory(); 
        UpdateEquipmentUI();

    }
    private void Init()
    {
        //초기 1회 켜질때 해야할 행동
        if (isInitalized) { return; }
        isInitalized = true;
        closeButton.onClick.AddListener(OnCloseButtonClick);
        equipmentInfoPanel.MountEquipmentRequest += MountEquipment;

        equipmentSlotDict = new Dictionary<eEquipmentType, EquipmentSlotUI>();
        //장비 슬롯 설정
        foreach (EquipmentSlotUI item in equipmentSlotArray)
        {
            if (equipmentSlotDict.ContainsKey(item.EquipmentType))
            {
                MyLog.LogError("장비 슬롯이 중복이 되어있습니다. 확인해주세요");
                continue;
            }
            equipmentSlotDict[item.EquipmentType] = item;
            item.EquipmentSlotUIRequest += UnEquipmentMethod;
        }
    }
    private void UnInit()
    {
        if(!isInitalized) { return; }
        isInitalized = false;
        closeButton.onClick.RemoveAllListeners();
        equipmentInfoPanel.MountEquipmentRequest -= MountEquipment;

        foreach (EquipmentSlotUI item in equipmentSlotArray)
        {
            item.EquipmentSlotUIRequest -= UnEquipmentMethod;
        }
        ClearEquipmentUI();
    }
    //캐릭터 장비 표시하는 스코롤 뷰
    private void OnDestroy()
    {
        UnInit();

    }
    private void OnCloseButtonClick()
    {
        equipmentInfoPanel.ClosePanel();
        equipmentUnEquipPanel.ClosePanel();
        this.gameObject.SetActive(false);
    }
    
    public void SetDataAndShowPanel(CharacterDataSO data)
    {
        Init();
        curCharacterData = data;
        //캐릭터 정보 표시
        characterImage.sprite = curCharacterData.CharacterBodySprite;
        characterName.text = curCharacterData.CharacterName;
        

        //변경사항 : 기본 능력치가 아니라, 장비까지 포함한 캐릭터의 능력치가 필요함. 
        CharacterInstance characterInstance = CharacterManager.Instance.HaveCharacterDict[data.CharacterId];
        CharacterStat characterTotalStat = characterInstance.GetCharacterTotalStat();
        statIconAll.SetIcon(characterTotalStat);

        //현재 장비와 인벤토르 장비 표시
        UpdateEquipmentInventory();
        UpdateEquipmentUI();

        this.gameObject.SetActive(true);
    }
    private void ClearEquipmentUI()
    {
        bool canReturn = EquipmentButtonSpawner.Instance != null;
        foreach (var item in equipmentUIList)
        {
            item.OnClickUI -= ShowEquipmentInfo;
            if (canReturn) EquipmentButtonSpawner.Instance.ReturnPrefabToPool(item);
        }
        equipmentUIList.Clear();
    }

    private void UpdateEquipmentUI()
    {
        //개선필요: UI안의 데이터가 아니라 런타임 데이터를 읽어서 표시해야한다.
        //
        CharacterEquipment characterEquipment = CharacterManager.Instance.GetCharacterEquipment(curCharacterData.CharacterId);
        //런타임 데이터를 읽어서 

        foreach(KeyValuePair<eEquipmentType, EquipmentSlotUI> pair in equipmentSlotDict)
        {
            EquipmentInstance equipmentInstance = characterEquipment.GetEquipment(pair.Key);
            if (equipmentInstance == null)
            {
                //정보가 없으면 UI 초기화
                pair.Value.ClearData();
                continue;
            }
            //정보 있으면 해당정보로 설정
            pair.Value.UpdateUI(equipmentInstance, curCharacterData);
        }
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
            //풀에서 꺼내서 장비를 하나씩 인벤토리에 추가해준다.
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
