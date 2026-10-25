using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public readonly struct EquipmentKey : IEquatable<EquipmentKey>
{
    //딕셔너리에서 키값으로 쓸수있도록 상속받음

    //장비 아이템에 대한 인스턴스 기록

    public readonly string equipmentId;
    public readonly int level;

    public EquipmentKey(string id, int lvl)
    {
        equipmentId = id;
        level = lvl;
    }


    public bool Equals(EquipmentKey other)
    {
        if (equipmentId == other.equipmentId && level == other.level) return true;
        return false;
    }
    public override bool Equals(object obj)
    {
        return obj is EquipmentKey other && Equals(other);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(equipmentId, level);
    }
}

public class EquipmentManager : Singleton<EquipmentManager>
{
    //전체 장비에 대한 정보를 직렬화 해둠
    [SerializeField] List<EquipmentDataSO> allEquipmentDataList = new List<EquipmentDataSO>(); 
    Dictionary<string, EquipmentDataSO> allEquipmentItemDict = new(); 

    //TODO. DB에서 가져와서 정보를 Set/Get 해주는거 필요
    Dictionary<EquipmentKey, int> equipmentCountDict = new Dictionary<EquipmentKey, int>(); //인벤토리에 몇개 남아있는지 정보를 가지고있음
    Dictionary<EquipmentKey, EquipmentInstance> equipmentInstanceDict = new Dictionary<EquipmentKey, EquipmentInstance>(); //인벤토리 및 캐릭터별 장비를 불러올때, 캐싱해주기 위한 딕셔너리

    //읽기 전용 딕셔너리
    public IReadOnlyDictionary<EquipmentKey, int> EquipmentCountDict => equipmentCountDict;

    public void InitHaveEquipmentItem(List<ItemDbInfo> itemDbList)
    {
        //가지고있는 아이템 리스트 초기화
        equipmentCountDict.Clear();
        equipmentInstanceDict.Clear();
        foreach(var item in itemDbList)
        {
            EquipmentKey key = new EquipmentKey(item.ItemId, item.Level);
            if(equipmentCountDict.ContainsKey(key))
            {
                MyLog.LogError($"{item.ItemId}_{item.Level}의 항목이 중복되었습니다. 뭔가에러");
                continue;
            }

            equipmentCountDict.Add(key, item.Count);
            
            if(equipmentInstanceDict.ContainsKey(key))
            {
                MyLog.LogError($"{item.ItemId}_{item.Level}의 항목이 중복되었습니다. 뭔가에러");
                continue;
            }

            EquipmentInstance equipmentInstance = GetEquipmentInstance(item.ItemId, item.Level); //Get안에서 없으면 새로 만들어줄거임
        }
    }


    public int GetEquipmentCount(string id, int level)
    {
        EquipmentKey instance = new EquipmentKey(id, level);
        return equipmentCountDict.GetValueOrDefault(instance);
    }
    public bool AddEquipmentCount(string id, int level, int amount)
    {
        EquipmentKey instance = new EquipmentKey(id, level);
        int resCount = GetEquipmentCount(id, level) + amount;

        if(resCount < 0)
        {
            return false;
        }
        equipmentCountDict[instance] = resCount;
        return true;
    }
    public bool GetEquipment(string id, int level)
    {
        if (GetEquipmentCount(id, level) <= 0) return false;
        EquipmentKey instance = new EquipmentKey(id, level);
        equipmentCountDict[instance]--;
        return true;
    }

    public EquipmentInstance GetEquipmentInstance(string id, int level)
    {
        //EquipmentInstance를 가져온다, 없으면 생성
        EquipmentKey instance = new EquipmentKey(id, level);
        if (!equipmentInstanceDict.TryGetValue(instance, out EquipmentInstance equipmentInstance))
        {
            //새로 만들어준다.
            EquipmentDataSO equipmentData =  GetEquipmentData(id);
            if (equipmentData == null) 
            {
                MyLog.LogError("장비 ID가 없어서 딕셔너리에 값을 못넣었습니다. null 반환됨");
                return null; 
            }
            equipmentInstance = new EquipmentInstance(equipmentData, level);
            equipmentInstanceDict[instance] = equipmentInstance;
        }
        return equipmentInstance;
    }


    protected override void OnAwake()
    {
    }
    private void Start()
    {
        //모든 장비에 대해서 검색 가능하도록 딕셔너리 만듬
        foreach(var item in allEquipmentDataList)
        {
            allEquipmentItemDict.Add(item.Id,item);
        }
    }
    public EquipmentDataSO GetEquipmentData(string id)
    {
        //딕셔너리에서 장비 정보 검색
        if (!allEquipmentItemDict.ContainsKey(id))
        {
            MyLog.LogError($"{id}에 해당하는 장비가 없습니다. 뭔가 에러");
            return null;
        }

        return allEquipmentItemDict[id];
    }
    public async Task<bool> TryUnEquipAsync(CharacterDataSO characterDataSO, EquipmentInstance curEquipment)
    {
        //장비를 해제한다.
        CharacterEquipment characterEquipment = CharacterManager.Instance.GetCharacterEquipment(characterDataSO.CharacterId);
        if (characterEquipment == null)
        {
            MyLog.LogError("null뜸");
            return false;
        }
        //1. 클라우드 데이터 처리. 클라우드에서 캐릭터 장착 아이템 null 처리하고, 인벤토리에 그 아이템 넣을거임
        CharacterDataDb characterDataDb = await FirestoreManager.Instance.UnEquipmentChracter(characterDataSO, curEquipment);
        if (characterDataDb == null)
        {
            MyLog.LogError($"DB에 저장 실패.");
            return false;
        }
        //2. 런타임 데이터도 수정해준다. (장착 장비와, 인벤토리에 아이템+1)
        characterEquipment.UpdateCharacterEquipment_DB(characterDataDb);
        AddEquipmentCount(curEquipment.EquipmentDataSO.Id, curEquipment.Level, 1);

        return true;
    }

    public async Task<bool> TryEquipAsync(CharacterDataSO characterDataSO , EquipmentInstance newEquipment)
    {
        //장비 장착 요청이 들어오면, 장비를 장착한다. (DB + 런타임 데이터 갱신 필요)
        //현재 캐릭터의 런타임 장비정보 가져오기 
        CharacterEquipment characterEquipment =  CharacterManager.Instance.GetCharacterEquipment(characterDataSO.CharacterId);

        //기존 캐릭터가 장착중인 장비의 정보를 가져오기
        eEquipmentType equipmentType = newEquipment.EquipmentDataSO.EquipmentType;
        EquipmentInstance oldEquipment =  characterEquipment.GetEquipment(equipmentType);

        //동일 장비를 장착하면 false
        if (oldEquipment != null &&
        oldEquipment.EquipmentDataSO.Id == newEquipment.EquipmentDataSO.Id &&
        oldEquipment.Level == newEquipment.Level)
        {
            return false; // 이미 장착 중
        }
        //DB에 트랜젝션으로 데이터 쓰기 
        CharacterDataDb characterDataDb = await FirestoreManager.Instance.TryEquipTransactionAsync(characterDataSO, newEquipment, oldEquipment, equipmentType);
        //db클래스에 값이 정상적으로 들어왔으면 해당 데이터로 런타임 데이터 갱신
        if (characterDataDb == null) 
        {
            MyLog.LogError("DB저장 실패");
            return false; 
        }
        //런타임 데이터 갱신 -> 캐릭터별 장착 아이템 및 인벤토리 item 갯수
        characterEquipment.UpdateCharacterEquipment_DB(characterDataDb);
        if(oldEquipment != null)
        {
            AddEquipmentCount(oldEquipment.EquipmentDataSO.Id, oldEquipment.Level, +1);
        }
        AddEquipmentCount(newEquipment.EquipmentDataSO.Id, newEquipment.Level, -1);
        return true;

    }

}
