using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TextCore.Text;

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
        initTask =  InitFirestore();
        await initTask;
        //await SaveItemAsync("Player1","Item1",1,50);
        //await LoadItemAsync("Player1", "Item1");
    }
    private async Task InitFirestore()
    {
        //파이어스토어 초기화
        await FirebaseApp.CheckAndFixDependenciesAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("Firebase 초기화 실패");
                    return;
                }

                if (task.Result != DependencyStatus.Available)
                {
                    Debug.LogError(
                        $"Firebase 사용 불가: {task.Result}");
                    return;
                }

                db = FirebaseFirestore.DefaultInstance;
                isReady = true;

                Debug.Log("Firestore 초기화 완료");
            });
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

    //저장 및 불러오기
    public async Task SaveItemAsync(string uid, string itemmId, int level, int count)
    {
        //초기화 끝났는지 대기
        await WaitForInitAsync();

        //DB의 Ref 주소 생성
        string userText = "users";
        string inventoryText = "inventory";
        //DocumentReference itemRef = db.Collection("users").Document(uid).Collection("inventory").Document(itemmId);
        DocumentReference itemRef = db.Collection(userText).Document(uid).Collection(inventoryText).Document(itemmId);

        string levelText = "level";
        string countText = "count";

        Dictionary<string, object> itemData = new Dictionary<string, object>
        {
            {levelText, level },
            { countText, count}
        };
        await itemRef.SetAsync(itemData);
    }
    public async Task LoadItemAsync(string uid, string itemId)
    {
        //DB의 Ref 주소 생성
        string userText = "users";
        string inventoryText = "inventory";
        DocumentReference itemRef = db.Collection(userText).Document(uid).Collection(inventoryText).Document(itemId);

        DocumentSnapshot snapshot = await itemRef.GetSnapshotAsync();
        if(!snapshot.Exists)
        {
            MyLog.Log("해당 장비 데이터가 없습니다");
            return;
        }
        string levelText = "level";
        string countText = "count";
        int level = snapshot.GetValue<int>(levelText);
        int count = snapshot.GetValue<int>(countText);
        MyLog.Log($"장비 ID: {itemId}");
        MyLog.Log($"레벨: {level}");
        MyLog.Log($"개수: {count}");
    }

    //보유 캐릭터 불러오기
    public async Task LoadAndSetHaveCharacterInfo(string userId)
    {
        //해당 유저가 가지고있는 전체 캐릭터 정보를 불러오고, 정보를 셋팅해준다.
        string userText = "users";
        string characterText = "characters";
        CollectionReference characterRef = db.Collection(userText).Document(userId).Collection(characterText);
        QuerySnapshot result = await characterRef.GetSnapshotAsync();
        string levelText = "level";
        string expText = "exp";
        List<Task> waitTaskList = new List<Task>();
        
        foreach (DocumentSnapshot document in result.Documents)
        {
            string characterId = document.Id;
            int level = document.GetValue<int>(levelText);
            int exp = document.GetValue<int>(expText);
            CharacterLevelDataDB levelDataDB = new CharacterLevelDataDB(characterId,level,exp);
            waitTaskList.Add(MakeCharacterInstance(characterId,levelDataDB));
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
        CharacterDataSO characterSO = allCharacterDict[characterId];
        CharacterInstance characterInstance = new CharacterInstance(characterSO, levelDataDB);
        CharacterManager.Instance.AddHaveCharacterInstance(characterInstance);
    }

    public async void AddCharacter(string userId, string characterId)
    {
        int initLevel = 1;
        int initExp = 0;
        List<Task> addCharacterTask = new List<Task>();

        string userText = "users";
        string characterText = "characters";
        DocumentReference characterRef = db.Collection(userText).Document(userId).Collection(characterText).Document(characterId);
        string levelText = "level";
        string expText = "exp";
        Dictionary<string, object> characterDataDict = new Dictionary<string, object>()
        {
            {levelText,initLevel },
            {expText,initExp }
        };
        addCharacterTask.Add(characterRef.SetAsync(characterDataDict));
        //await characterRef.SetAsync(characterDataDict);

        //캐릭터 인게임에도  정보 필요
        CharacterLevelDataDB characterLevelDataDB = new CharacterLevelDataDB(characterId, initLevel, initExp);
        //await MakeCharacterInstance(characterId, characterLevelDataDB);
        addCharacterTask.Add(MakeCharacterInstance(characterId, characterLevelDataDB));

        await Task.WhenAll(addCharacterTask);

    }

    public void CreateNewAccount(string userId)
    {
        //새로운 계정을 생성할때 계정 정보를 넣어준다.
        AddCharacter(userId, "1001");
    }
}