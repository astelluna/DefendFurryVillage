using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TextCore.Text;


public enum eItemtype
{
    equipment, item
}

public class FirestoreManager : Singleton<FirestoreManager>
{
    private FirebaseFirestore db;
    private bool isReady;
    Task initTask;
    protected override void OnAwake()
    {
    }
    private async void Start()
    {
        await FirebaseAuthManager.Instance.AuthTcs.Task;
        initTask = InitFirestore();
        await initTask;
        //await SaveItemAsync("Player1","Item1",1,50);
        //await LoadItemAsync("Player1", "Item1");
    }
    private async Task InitFirestore()
    {
        //파이어스토어 초기화
        DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (status != DependencyStatus.Available)
            throw new Exception($"Firebase 사용 불가: {status}");

        db = FirebaseFirestore.DefaultInstance;
        isReady = true;
    }
    private async Task WaitForInitAsync()
    {
        //처리할때 파이어스토어가 초기화되어있었는지 확인
        Task timeoutTask = Task.Delay(3000);
        Task completedTask = await Task.WhenAny(initTask, timeoutTask);
        if(completedTask != initTask)
        {
            throw new TimeoutException("Firestore초기화 대기시간 초과(3초)");
        }
        // 초기화 작업 자체가 실패했다면 예외가 여기서 전달됨
        await initTask;
    }
 
    private bool ParseItemKey(string itemKey, out string itemId, out int level)
    {
        //itemKey의 string 형식을 변환하여 입력시켜준다.
        string[] splitKey = itemKey.Split("_");
        itemId = "";
        level = 0;
        if (splitKey[0] =="")
        {
            MyLog.LogError("아이템ID값이 이상함. 공백입니다");
            return false;
        }
        itemId = splitKey[0];
        if(splitKey.Length >= 2)
        {
            //장비의 경우 레벨이 존재한다.
            level = int.Parse(splitKey[1]);
        }
        return true;
    }

    private string MakeItemKey_ForInventory(string itemId, int level)
    {
        return itemId + "_" + level.ToString();
    }
    private bool MakeItemTypeToKey_ForInventory(eItemtype itemtype, out string itemKey)
    {
        string itemType_equipment = "equipment";
        string itemType_item = "item";
        string key = "";
        switch (itemtype)
        {
            case eItemtype.equipment:
                key = itemType_equipment;
                break;
            case eItemtype.item:
                key = itemType_item;
                break;
        }
        if(key == "")
        {
            itemKey = "";
            MyLog.LogError("장비 타입키가 이상함");
            return false;
        }
        else
        {
            itemKey = key;
            return true;
        }

    }
    public async void SaveItem_Test(string uid)
    {
        await SaveItemAsync(uid, eItemtype.equipment, "1001", 3, 5);
    }

    //저장 및 불러오기
    public async Task SaveItemAsync(string uid, eItemtype itemtype, string itemId, int level, int count)
    {
        //초기화 끝났는지 대기
        await WaitForInitAsync();

        //DB의 Ref 주소 생성
        string userText = "users";
        string inventoryText = "inventory";
        string itemType_equipment = "equipment";
        string itemType_item = "item";
        string itemCollect = "items";
        string countText = "count";
        string itemTypeKey = "";
        if(!MakeItemTypeToKey_ForInventory(itemtype, out itemTypeKey))
        {
            return; 
        }
        string itemKey = MakeItemKey_ForInventory(itemId, level);
        //Ref 찾기 및 저장
        DocumentReference itemRef = db.Collection(userText).Document(uid).Collection(inventoryText).Document(itemTypeKey).Collection(itemCollect).Document(itemKey);
        if (count <= 0)
        {
            MyLog.Log("갯수0개로 인해서 인벤토리에서 삭제합니다");
            await itemRef.DeleteAsync();
            return;
        }
        //딕셔너리로 저장해서 나중에 꺼내올때 구분이 가능하게 해준다
        await itemRef.SetAsync(new Dictionary<string, object>
        {
            { countText, count }
        });
    }
    public async Task<List<ItemDbInfo>> LoadItemAsync_All(string uid, eItemtype itemtype)
    {
        await WaitForInitAsync();
        //특정 타입의 아이템 전체를 불러올거임. 
        List<ItemDbInfo> result = new List<ItemDbInfo>();


        //DB의 Ref 주소 생성
        string userText = "users";
        string inventoryText = "inventory";
        string itemCollect = "items";
        string itemTypeKey = "";
        string countText = "count";
        if (!MakeItemTypeToKey_ForInventory(itemtype, out itemTypeKey))
        {
            return null;
        }
        CollectionReference itemsRef = db.Collection(userText).Document(uid).Collection(inventoryText).Document(itemTypeKey).Collection(itemCollect);

        // 해당 컬렉션의 모든 문서 읽기
        QuerySnapshot snapshot = await itemsRef.GetSnapshotAsync();

        foreach(DocumentSnapshot doc in snapshot.Documents)
        {
            if (!doc.Exists) { continue; }
            int count = Convert.ToInt32(doc.GetValue<long>(countText));

            if(!ParseItemKey(doc.Id,out string itemId,out int level))
            {
                //실패하면 넘어감
                continue;
            }
            result.Add(new ItemDbInfo()
            {
                ItemId = itemId,
                Level = level,
                Count = count
            });

        }
        return result;
    }

    //보유 캐릭터 불러오기
    public async Task LoadAndSetHaveCharacter_Init(string userId)
    {
        //초기 1회 해당 유저가 가지고있는 전체 캐릭터 정보를 불러오고, 정보를 셋팅해준다.
        string userText = "users";
        string characterText = "characters";
        CollectionReference characterRef = db.Collection(userText).Document(userId).Collection(characterText);
        QuerySnapshot result = await characterRef.GetSnapshotAsync();
        string levelText = "level";
        string expText = "exp";
        List<Task> waitTaskList = new List<Task>();
        
        foreach (DocumentSnapshot document in result.Documents)
        {
            //DB에서 캐릭터 레벨 정보를 불러온뒤, 캐릭터 정보를 CharacterManager에 Set 해준다.
            CharacterLevelDataDB levelDataDB = document.ConvertTo<CharacterLevelDataDB>();
            waitTaskList.Add(MakeCharacterInstance(levelDataDB.id, levelDataDB));
        }
        await Task.WhenAll(waitTaskList);

    }
    private async Task MakeCharacterInstance(string characterId, CharacterLevelDataDB levelDataDB)
    {
        await CharacterManager.Instance.InitalizeCharacterTask;
        IReadOnlyDictionary<string, CharacterDataSO> allCharacterDict = CharacterManager.Instance.AllCharacterSODict;
        if(!allCharacterDict.ContainsKey(characterId))
        {
            MyLog.LogError($"{characterId}가 전체 캐릭터Id에 존재하지 않습니다");
            return;
        }
        //캐릭터매니저에 정보 설정
        CharacterDataSO characterSO = allCharacterDict[characterId];
        CharacterInstance characterInstance = new CharacterInstance(characterSO, levelDataDB);
        CharacterManager.Instance.AddHaveCharacterInstance(characterInstance);
    }

    private async Task<bool> AddNewCharacter(string userId, string characterId)
    {
        int initLevel = 1;
        int initExp = 0;
        //List<Task> addCharacterTask = new List<Task>();

        string userText = "users";
        string characterText = "characters";
        DocumentReference characterRef = db.Collection(userText).Document(userId).Collection(characterText).Document(characterId);
        string levelText = "level";
        string expText = "exp";
        
        CharacterLevelDataDB characterLevelDataDB = new CharacterLevelDataDB(characterId, initLevel, initExp);
        Dictionary<string, object> characterDataDict = new Dictionary<string, object>()
        {
            {levelText,initLevel },
            {expText,initExp }
        };
        try
        {
            await characterRef.SetAsync(characterLevelDataDB);
        }
        catch(Exception e)
        {
            MyLog.LogError($"캐릭터 DB 저장 실패 ({characterId}): {e.Message}");
            return false;
        }
        try
        {
            //캐릭터 인게임에도  정보 필요
            await MakeCharacterInstance(characterId, characterLevelDataDB);
        }
        catch(Exception e)
        {
            //실패하면 DB에서 지워버림
            MyLog.LogError($"캐릭터 인스턴스 생성 실패 ({characterId}): {e.Message}");
            await RollbackCharacterAdd(characterRef);
            return false;
        }

        return true;
    }
    private async Task RollbackCharacterAdd(DocumentReference characterRef)
    {
        try { await characterRef.DeleteAsync(); }
        catch (Exception e) { MyLog.LogError($"롤백 실패, 수동 확인 필요: {e.Message}"); }
    }

    public async Task CheckAndCreateNewAccount(string userId)
    {
        await initTask;

        //새로운 계정인지 체크
        DocumentReference docRef = db.Collection("users").Document(userId);
        DocumentSnapshot docSnapShot = await docRef.GetSnapshotAsync();
        if (docSnapShot.Exists) return;

        //새로운 계정을 생성할때 할 행동들 진행
        //기본 캐릭터 넣어주기
        if (!await AddNewCharacter(userId, "1001"))
        {
            //실패했으면 문서 안만들어줌 -> 다음에 다시 시도함
            return;
        }
        // 2) 성공했을 때만 계정 문서 생성
        await docRef.SetAsync(new Dictionary<string, object>
        {
            { "createdAt", FieldValue.ServerTimestamp }
        });
    }
}