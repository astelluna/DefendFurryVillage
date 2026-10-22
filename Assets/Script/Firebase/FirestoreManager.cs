using Firebase;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using UnityEngine.TextCore.Text;


public enum eItemtype
{
    equipment, item
}

public class FirestoreManager : Singleton<FirestoreManager>
{
    //데이터를 쓰고, 읽는 전용 매니저 (Firestore를 이용한다)

    private FirebaseFirestore db;
    private bool isReady;
    Task initTask;

    //전역 string 객체들
    const string itemType_equipment = "equipment";
    const string itemType_item = "item";
    const string userText = "users";
    const string inventoryText = "inventory";
    const string itemCollect = "items";
    const string countText = "count";
    const string characterText = "characters";
    const string levelText = "level";
    const string expText = "exp";
    const string goldText = "gold";
    const string createdText = "createdAt";

    #region 캐릭터및 계정 생성할때 초기 값들
    //계정 생성할때 쓰는 값
    long initAccountGold = 10000; //초기 골드
    //캐릭터 생성할때 쓰는 값
    int initCharacterLevel = 1;
    int initCharacterExp = 0;
    #endregion

    protected override void OnAwake()
    {
    }
    private async void Start()
    {
        await FirebaseAuthManager.Instance.AuthTcs.Task;
        initTask = InitFirestore();
        await initTask;
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

    private string MakeItemKey(string itemId, int level)
    {
        //저장을 할때 장비의 경우 레벨도 같이 저장해야해서 추가. 소모품은 레벨이 없는데 동일한방식으로 만들건지 생각해봐야함.
        return itemId + "_" + level.ToString();
    }
    private bool MakeItemTypeToKey_ForInventory(eItemtype itemtype, out string itemKey)
    {
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
    public async void SaveItem_Test()
    {
        await SaveItemAsync(eItemtype.equipment, "1001", 3, 5);
    }

    //저장 및 불러오기
    public async Task<bool> SaveItemAsync(eItemtype itemtype, string itemId, int level, int count)
    {
        //초기화 끝났는지 대기
        await WaitForInitAsync();

        string userId = FirebaseAuthManager.Instance.User.UserId;

        //DB의 Ref 주소 생성
        string itemTypeKey = "";
        if(!MakeItemTypeToKey_ForInventory(itemtype, out itemTypeKey))
        {
            return false; 
        }
        string itemKey = MakeItemKey(itemId, level);
        //Ref 찾기 및 저장
        DocumentReference itemRef = db.Collection(userText).Document(userId).Collection(inventoryText).Document(itemTypeKey).Collection(itemCollect).Document(itemKey);
        if (count <= 0)
        {
            MyLog.Log("갯수0개로 인해서 인벤토리에서 삭제합니다");
            await itemRef.DeleteAsync();
            return true;
        }
        //딕셔너리로 저장해서 나중에 꺼내올때 구분이 가능하게 해준다
        await itemRef.SetAsync(new Dictionary<string, object>
        {
            { countText, count }
        });
        return true;
    }
    public async Task<List<ItemDbInfo>> LoadItemAsyncAll_Init(eItemtype itemtype)
    {
        await WaitForInitAsync();
        //특정 타입의 아이템 전체를 불러올거임. 
        string userId = FirebaseAuthManager.Instance.User.UserId;
        List<ItemDbInfo> result = new List<ItemDbInfo>();


        //DB의 Ref 주소 생성
        string itemTypeKey = "";
        if (!MakeItemTypeToKey_ForInventory(itemtype, out itemTypeKey))
        {
            return null;
        }
        CollectionReference itemsRef = db.Collection(userText).Document(userId).Collection(inventoryText).Document(itemTypeKey).Collection(itemCollect);

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
    public async Task LoadAndSetHaveCharacter_Init()
    {
        string userId = FirebaseAuthManager.Instance.User.UserId;
        //초기 1회 해당 유저가 가지고있는 전체 캐릭터 정보를 불러오고, 정보를 셋팅해준다.
        CollectionReference characterRef = db.Collection(userText).Document(userId).Collection(characterText);
        QuerySnapshot result = await characterRef.GetSnapshotAsync();
        List<Task> waitTaskList = new List<Task>();
        
        foreach (DocumentSnapshot document in result.Documents)
        {
            //DB에서 캐릭터 레벨 정보를 불러온뒤, 캐릭터 정보를 CharacterManager에 Set 해준다.
            CharacterDataDb levelDataDB = document.ConvertTo<CharacterDataDb>();
            waitTaskList.Add(MakeRuntimeCharacterInstance_Sync(levelDataDB.id, levelDataDB));
        }
        await Task.WhenAll(waitTaskList);

    }
    private async Task MakeRuntimeCharacterInstance_Sync(string characterId, CharacterDataDb characterData)
    {
        //캐릭터 새로 생성할때 해당데이터를 런타임 데이터에도 추가
        await CharacterManager.Instance.InitalizeCharacterTask;
        IReadOnlyDictionary<string, CharacterDataSO> allCharacterDict = CharacterManager.Instance.AllCharacterSODict;
        if(!allCharacterDict.ContainsKey(characterId))
        {
            MyLog.LogError($"{characterId}가 전체 캐릭터Id에 존재하지 않습니다");
            return;
        }
        //캐릭터매니저에 정보 설정
        CharacterDataSO characterSO = allCharacterDict[characterId];
        CharacterInstance characterInstance = new CharacterInstance(characterSO, characterData);
        CharacterManager.Instance.AddHaveCharacterInstance(characterInstance);
    }

    #region 캐릭터가 장착하고있는 아이템의 정보를 바꿔야 할때 
    public async Task<bool> SaveCharacterEquipment(CharacterDataSO dataSO, EquipmentInstance equipInstance, eEquipmentType equipmentType)
    {
        string userId = FirebaseAuthManager.Instance.User.UserId;
        //ref부터 찾자.
        DocumentReference characterRef = db.Collection(userText).Document(userId).Collection(characterText).Document(dataSO.CharacterId);
        //예외처리
        if(!CharacterManager.Instance.HaveCharacterDict.TryGetValue(dataSO.CharacterId, out CharacterInstance characterInstance))
        {
            MyLog.LogError("캐릭터 id가 없어서 찾기 실패");
            return false;
        }
        //정보 설정
        if(!characterInstance.characterLevelDataDb.SetCharacterEquipmentDB(equipmentType, equipInstance))
        {
            MyLog.LogError("캐릭터 DB에 정보설정 실패");
            return false;
        }
        await characterRef.SetAsync(characterInstance);
        return true;
    }

    public async Task<CharacterDataDb> UnEquipmentChracter(CharacterDataSO characterDataSO, EquipmentInstance equipmentInstance)
    {
        //캐릭터 장비 장착 해제 메서드.
        await WaitForInitAsync();

        string uid = FirebaseAuthManager.Instance.User.UserId;
        //필요한게 뭘까? 일단 주소가 필요함. 1. 캐릭터 주소, 2. 인벤토리 주소
        //1. 인벤토리 주소
        string equipmentKey = MakeItemKey(equipmentInstance.EquipmentDataSO.EquipmentId, equipmentInstance.Level);
        DocumentReference equipmentReference = db
            .Collection(userText).Document(uid)
            .Collection(inventoryText).Document(itemType_equipment)
            .Collection(itemCollect).Document(equipmentKey);
        //2. 캐릭터 주소
        DocumentReference characterRef = db
            .Collection(userText).Document(uid)
            .Collection(characterText).Document(characterDataSO.CharacterId);

        //트랜젝션 진행
        try
        {
            //return await db.RunTransactionAsync<CharacterDataDb>(async transaction =>
            return await db.RunTransactionAsync<CharacterDataDb>(async transaction =>
            {
                //스냅샷으로 기존 장비를 읽고, 정보를 가져온다. 
                DocumentSnapshot inventorySnapshot = await transaction.GetSnapshotAsync(equipmentReference);
                //하지말자. if (!inventorySnapshot.Exists) { return false; } //아이템이 인벤토리에 없을수도있으므로 이거 체크하면 안됨.
                DocumentSnapshot characterSnapshot = await transaction.GetSnapshotAsync(characterRef);
                if (!characterSnapshot.Exists) { return null; }
                //캐릭터는 DB
                CharacterDataDb characterDataDb = characterSnapshot.ConvertTo<CharacterDataDb>();
                if (characterDataDb == null) { return null; }
                //기존 장비를 장착 해제하는 과정 진행 -> 검증부터 하고, 이후 해제한다.
                //1. 장비 가져와서 비교
                eEquipmentType equipmentType = equipmentInstance.EquipmentDataSO.EquipmentType;
                CharacterEquipmentDataDB equippedData =  characterDataDb.GetCharacterEquipmentDataDB(equipmentType);
                if (equippedData == null) { return null; }

                if(equippedData.equipmentId != equipmentInstance.EquipmentDataSO.EquipmentId ||
                equippedData.equipmentLevel != equipmentInstance.Level)
                {
                    return null;
                }
                //2. 검증통과했으면 장착 해제
                CharacterEquipmentDataDB characterEquipmentDataDB = characterDataDb.UnEquipmentCharacterDB(equipmentInstance.EquipmentDataSO.EquipmentType);
                if (characterEquipmentDataDB == null) { return null; }

                
                //쓰기 작업
                //이 장비를 인벤토리에 추가. 장비 해제했으므로 +1 해준다.
                int equipmentCount = inventorySnapshot.Exists ? Convert.ToInt32(inventorySnapshot.GetValue<long>(countText)) : 0;
                int remainCount = equipmentCount + 1;
                transaction.Set(equipmentReference, new Dictionary<string, object>
                {
                    {countText,remainCount }
                });
                transaction.Set(characterRef, characterDataDb);

                return characterDataDb;
            });
        }
        catch (Exception e)
        {
            MyLog.LogError($"장착해제 실패" + e);
            return null;
        }
    }

    public async Task<CharacterDataDb> TryEquipTransactionAsync(CharacterDataSO characterSO, EquipmentInstance newEquipment, EquipmentInstance oldEquipment, eEquipmentType type)
    {
        await WaitForInitAsync();

        string uid = FirebaseAuthManager.Instance.User.UserId;

        // 새 장비의 인벤토리 문서
        string newKey = MakeItemKey(
            newEquipment.EquipmentDataSO.EquipmentId,
            newEquipment.Level);

        DocumentReference newItemRef = db
            .Collection(userText).Document(uid)
            .Collection(inventoryText).Document(itemType_equipment)
            .Collection(itemCollect).Document(newKey);

        // 기존 장비의 인벤토리 문서
        DocumentReference oldItemRef = null;

        if (oldEquipment != null)
        {
            string oldKey = MakeItemKey(
                oldEquipment.EquipmentDataSO.EquipmentId,
                oldEquipment.Level);

            oldItemRef = db
                .Collection(userText).Document(uid)
                .Collection(inventoryText).Document(itemType_equipment)
                .Collection(itemCollect).Document(oldKey);
        }
        //해당 캐릭터 장착을 위해
        DocumentReference characterRef = db
            .Collection(userText).Document(uid)
            .Collection(characterText)
            .Document(characterSO.CharacterId);

        try
        {
            //트렌젝션으로 안의 모든것이 완료될때만 진행하도록 함
            return await db.RunTransactionAsync<CharacterDataDb>(async transaction =>
            {
                // 1. 필요한 문서를 모두 읽는다.
                DocumentSnapshot newSnap =
                    await transaction.GetSnapshotAsync(newItemRef);

                DocumentSnapshot oldSnap = null;

                if (oldItemRef != null)
                {
                    oldSnap = await transaction.GetSnapshotAsync(oldItemRef);
                }

                DocumentSnapshot characterSnap =
                    await transaction.GetSnapshotAsync(characterRef);

                // 2. 데이터 검증
                if (!characterSnap.Exists)
                    return null;

                int newCount = newSnap.Exists
                    ? Convert.ToInt32(newSnap.GetValue<long>(countText))
                    : 0;

                if (newCount <= 0)
                    return null;

                int oldCount = oldSnap != null && oldSnap.Exists
                    ? Convert.ToInt32(oldSnap.GetValue<long>(countText))
                    : 0;

                // 3. 캐릭터 DB 데이터 준비
                CharacterDataDb characterDB =
                    characterSnap.ConvertTo<CharacterDataDb>();

                if (characterDB == null ||
                    !characterDB.SetCharacterEquipmentDB(type, newEquipment))
                {
                    return null;
                }

                // 4. 여기부터 쓰기 작업
                int remainCount = newCount - 1;

                if (remainCount <= 0)
                {
                    transaction.Delete(newItemRef); //더이상 장비없다면 삭제
                }
                else
                {
                    transaction.Set(newItemRef,
                        new Dictionary<string, object>
                        {
                        { countText, remainCount }
                        });
                }

                if (oldItemRef != null)
                {
                    transaction.Set(oldItemRef,
                        new Dictionary<string, object>
                        {
                        { countText, oldCount + 1 }
                        });
                }

                transaction.Set(characterRef, characterDB);

                return characterDB;
            });
        }
        catch (Exception e)
        {
            MyLog.LogError($"장비 장착 트랜잭션 실패: {e.Message}");
            return null;
        }
    }

    #endregion

    #region 계정의 골드전용 메서드
    public async Task<long> AddGold(int amount)
    {
        if(amount < 0) { return -1; } //실패표시, 호출쪽에서 처리하기위한 반환값

        await WaitForInitAsync();
        string userId = FirebaseAuthManager.Instance.User.UserId;
        DocumentReference docRef = db.Collection(userText).Document(userId);
        long result = 0;
        try
        {
            result = await db.RunTransactionAsync<long>(async transaction =>
            {
                DocumentSnapshot snap = await transaction.GetSnapshotAsync(docRef);
                long current = snap.Exists && snap.TryGetValue<long>(goldText, out long g) ? g : 0;
                long addValue = current + amount;
                transaction.Set(docRef, new Dictionary<string, object>()
                {
                    { goldText, addValue}
                }, SetOptions.MergeAll);
                return addValue;
            });
        }
        catch (Exception e)
        {
            MyLog.LogError($"골드 추가 실패: {e.Message}");
            return -1;
        }
        //트랜젝션이 끝나고 난뒤, 동기화 처리를 한다.
        if (result >= 0) SyncWithCurrencyManger(result);
        return result;

    }
    public async Task<bool> TrySpendGold(int cost)
    {
        //0원이면 그냥 true, 아니면 false
        if (cost == 0) return true;
        if (cost < 0) return false;
        //골드 소비 진행. 실패하면 false 반환하는 메서드
        await WaitForInitAsync();
        string userId = FirebaseAuthManager.Instance.User.UserId;
        DocumentReference docRef = db.Collection(userText).Document(userId);
        long result = 0;
        try
        {
            result = await db.RunTransactionAsync<long>(async transaction =>
            {
                DocumentSnapshot snap = await transaction.GetSnapshotAsync(docRef);
                long current = (snap.Exists && snap.TryGetValue<long>(goldText, out long g)) ? g : 0;
                long subValue = current - cost;
                if (subValue < 0) return -1;

                transaction.Set(docRef, new Dictionary<string, object>()
                {
                    { goldText,subValue}
                },SetOptions.MergeAll);
                return subValue;
            });
        }
        catch (Exception e)
        {
            MyLog.LogError($"골드 소비 실패: {e.Message}");
            return false;
        }
        //트랜젝션이 끝나고 난뒤, 동기화 처리를 한다.
        if (result < 0) return false;
        SyncWithCurrencyManger(result);
        return true;
    }
    private void SyncWithCurrencyManger(long amount)
    {
        CurrencyManager.Instance.SetGameMoney(amount);
    }
    public async Task<long> LoadGold()
    {
        await WaitForInitAsync();
        string userId = FirebaseAuthManager.Instance.User.UserId;
        DocumentReference docRef = db.Collection(userText).Document(userId);

        DocumentSnapshot snap = await docRef.GetSnapshotAsync();
        if (!snap.Exists) return 0;
        if (!snap.TryGetValue<long>(goldText, out long resultGold)) { return 0; }

        return resultGold;

    }
    #endregion

    #region 트랜젝션 공통코드 및 처리

    private long AddCharacter_Transaction(Firebase.Firestore.Transaction transaction, 
        DocumentReference characterRef, DocumentSnapshot characterSnap,
        DocumentReference userRef, DocumentSnapshot userSnap,
        CharacterDataDb characterDataDb, string characterId, long price)
    {
        //트랜젝션 공통 코드 - 돈이 충분하면 차감하고, 캐릭터 생성 및 돈을 적용
        if (characterSnap.Exists) { return -1; }
        //돈부터 확인.
        long curGold = userSnap.Exists && userSnap.TryGetValue<long>(goldText, out long g) ? g : 0;
        long subValue = curGold - price;
        if (subValue < 0) { return -1; }
        //쓰기
        transaction.Set(userRef, new Dictionary<string, object>()
            {
                {goldText,subValue }
            }, SetOptions.MergeAll);
        transaction.Set(characterRef, characterDataDb);
        return subValue;
    }
    //해야할거: 트렌젝션 - 캐릭터 및 골드를 얻는 공통 트랜젝션 코드를 넣는다. 
    //신규 캐릭터 생성에 대해서 한번 처리해보자. 
    public async Task<bool> CreateNewAccount()
    {
        await WaitForInitAsync();
        string userId = FirebaseAuthManager.Instance.User.UserId;
        
        ShopProduct shopProduct = new ShopProduct();
        Currency currency = new Currency();
        currency.SetData(eCurrencyType.gameMoney, 0);
        shopProduct.SetData(eProductType.character, "1000", "X이건안씀X", currency, 1);
        string characterId = shopProduct.ProductId;
        CharacterDataDb initCharacterData = new CharacterDataDb(characterId, initCharacterLevel, initCharacterExp);
        //long resMoney = 0;
        bool isTransactionSuccess = false;
        try
        {
            isTransactionSuccess = await db.RunTransactionAsync<bool>(async transaction =>
            {
                //읽기부터 전부 한다.
                DocumentReference characterRef = db.Collection(userText).Document(userId).Collection(characterText).Document(characterId);
                DocumentReference userRef = db.Collection(userText).Document(userId);
                DocumentSnapshot characterSnap = await transaction.GetSnapshotAsync(characterRef);
                DocumentSnapshot userSnap = await transaction.GetSnapshotAsync(userRef);

                //만약 유저가 이미 있었다면? 바로 취소 
                if (userSnap.Exists) { return false; }

                long transactionRes = AddCharacter_Transaction(transaction, characterRef, characterSnap, userRef, userSnap, initCharacterData, characterId, shopProduct.Price.Amount);
                if (transactionRes < 0) { return false; } //실패했으니까 -1이하 값이 들어있을거임
                //캐릭터 성공했으면 이제는 계정에 돈과 생성일자를 넣어주자.
                transaction.Set(userRef, new Dictionary<string, object>()
                {
                    {createdText, FieldValue.ServerTimestamp }
                },SetOptions.MergeAll);
                transaction.Set(userRef, new Dictionary<string, object>()
                {
                    {goldText, initAccountGold }
                },SetOptions.MergeAll);

                return true; 
            });
        }
        catch(Exception e)
        {
            MyLog.LogError($"캐릭터 추가 실패 : {e}");
            return false;
        }
        if (!isTransactionSuccess) { return false; }
        //트랜젝션이 정상적으로 끝났다면? 런타임 데이터도 수정해주자.
        SyncWithCurrencyManger(initAccountGold); //새로 생성해주면서 초기 골드로 넣엇으므로 초기 골드로 넣어줌
        await MakeRuntimeCharacterInstance_Sync(characterId, initCharacterData);
        return true;
    }
    #endregion

    #region 구매 관련 메서드 
    public async Task<bool> PurchaseProduct_Character(ShopProduct shopProduct)
    {
        //얘도 기존 트렌젝션 메서드로 처리해보자.
        await WaitForInitAsync();
        string userId = FirebaseAuthManager.Instance.User.UserId;
        string characterId = shopProduct.ProductId;
        CharacterDataDb initCharacterData = new CharacterDataDb(characterId, initCharacterLevel, initCharacterExp);
        long transactionResult;
        try
        {
            transactionResult = await db.RunTransactionAsync<long>(async transaction =>
            {
                DocumentReference characterRef = db.Collection(userText).Document(userId).Collection(characterText).Document(characterId);
                DocumentReference userRef = db.Collection(userText).Document(userId);
                DocumentSnapshot characterSnap = await transaction.GetSnapshotAsync(characterRef);
                DocumentSnapshot userSnap = await transaction.GetSnapshotAsync(userRef);

                long remainValue = AddCharacter_Transaction(transaction, characterRef, characterSnap, userRef, userSnap, initCharacterData, characterId, shopProduct.Price.Amount);
                if (remainValue < 0) { return -1; }
                return remainValue;
            });
        }
        catch (Exception e)
        {
            MyLog.LogError($"캐릭터 추가 실패 : {e}");
            return false;
        }
        if (transactionResult < 0) { return false; }
        //트랜젝션이 정상적으로 끝났다면? 런타임 데이터동기화
        SyncWithCurrencyManger(transactionResult); //새로 생성해주면서 초기 골드로 넣엇으므로 초기 골드로 넣어줌
        await MakeRuntimeCharacterInstance_Sync(characterId, initCharacterData);
        return true;
    }
    #endregion
}