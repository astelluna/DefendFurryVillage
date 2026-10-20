using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

public class AugmentManager : MonoBehaviour
{
    public static AugmentManager Instance { get; private set; }

    [SerializeField] AugmentPanel augmentPanel;

    List<WeaponAugment> selectedAugmentList = new List<WeaponAugment>();
    //List<Character> haveAugmentCharacterList = new List<Character>();
    //List<Character> selectedCharacterList = new List<Character>();

    List<Character> haveAugmentCharacterList = new List<Character>();
    List<Character> selectedCharacterList = new List<Character>();

    private bool isShowingPanel = false; //현재 패널 띄워져있는지 플래그
    int maxShowCount;
    //const로 적용된 증강 최대갯수, 캐릭터 확률
    const int Character_Pick_Percent = 80;
    const int Total_Augment_Slot = 3;

    private TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>(); // SetResult로 성공/실패 여부 설정

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    private void OnEnable()
    {
        CharacterManager.Instance.OnCharacterSpawnEnd += OnCharacterSpawnCompleted;
    }
    private void Start()
    {
        //시작하면 1번 띄우자 패널을 
        //SetAndShowAugmentUI(1);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
        if (isShowingPanel) Time.timeScale = 1f;
        ClearSelectAugmentList();
        ClearSelectCharacterList();
    }

    public async void SetAndShowAugmentUI(int cnt)
    {
        await CharacterManager.Instance.InitalizeCharacterTask;
        maxShowCount += cnt;
        if (maxShowCount <= 0) return;
        if (isShowingPanel) return;
        Time.timeScale = 0f;
        ShowAugmentUI();
    }

    //private bool NeedCharacterSpawn(List<CharacterDataSO> remainCharacters)
    private bool NeedCharacterSpawn(List<Character> remainCharacters)
    {
        if (remainCharacters.Count == 0) return false;
        if (!CharacterManager.Instance.HaveCharacter()) return true;
        int characterPercent = Character_Pick_Percent; //80퍼센트 확률로 캐릭터 먼저 제공
        int value = Random.Range(1, 101);
        if (value > characterPercent) { return false; }

        return true;
    }

    private void ShowAugmentUI()
    {

        isShowingPanel = true;
        maxShowCount--;
        //UI를 보여주고 현재 플레이어중 1개를 선택해서 쭉쭉 내려간다음에 증강 1개를 선택해서 가져온다.
        bool needShowPanel = false;
        ClearSelectAugmentList();
        ClearSelectCharacterList();

        //미리 남은 플레이어 항목 리스트를 받아둠.
        //List<CharacterDataSO> remainCharacterList = CharacterManager.Instance.GetRemainCharacter();
        List<Character> remainCharacterList = CharacterManager.Instance.GetRemainCharacter();
        for (int i=0; i< Total_Augment_Slot; i++)
        {
            if(NeedCharacterSpawn(remainCharacterList))
            {
                //플레이어중 한명을 선택해서 패널에 띄우도록
                int randIndex = Random.Range(0, remainCharacterList.Count);

                Character selCharacter = remainCharacterList[randIndex];
                remainCharacterList.Remove(selCharacter);
                augmentPanel.AddData(selCharacter, i);
                selectedCharacterList.Add(selCharacter);
                selCharacter.OnStartCharacterSpawn += OnCharacterSelected;

                //CharacterDataSO selCharacterSO = remainCharacterList[randIndex];
                //remainCharacterList.Remove(selCharacterSO);
                //augmentPanel.AddData(selCharacterSO, i);
                //selectedCharacterList.Add(selCharacterSO);
                //selCharacterSO.CharacterPrefab.OnStartCharacterSpawn += OnCharacterSelected;
            }
            else
            {
                //증강 소환
                //증강이 있는 플레이어를 확인한다.
                haveAugmentCharacterList = CharacterManager.Instance.GetHaveAugmentCharacterList();
                if (haveAugmentCharacterList.Count == 0)
                {
                    if (i == 0)
                    {
                        //단 1번이라도 실행이 안되었었다면? 시간 다시 흘러가야함
                        Time.timeScale = 1f;
                        isShowingPanel = false;
                        maxShowCount = 0;
                    }
                    MyLog.LogWarning("더이상 증강이 남아있는 플레이어가 없습니다");
                    break;
                }
                int characterNo = Random.Range(0, haveAugmentCharacterList.Count);
                //CharacterDataSO characterSO = haveAugmentCharacterList[CharacterNo];
                Character character = haveAugmentCharacterList[characterNo];

                WeaponAugment data = character.GetAugment(); //정보를 하나 가져와보자.
                augmentPanel.AddData(data, character.CharacterInstanceData.characterData.CharacterPortraitSprite, i); //여기서 알아서 i번째 UI에 UI를 넣도록 함.

                //이벤트 등록
                selectedAugmentList.Add(data);
                data.OnSelected += OnSelectedAugment;
            }

                
            needShowPanel = true;
        }
        if(needShowPanel)
        {
            augmentPanel.Show();
        }
    }
    public void OnCharacterSelected(Character character)
    {
        //선택된 플레이어  소환. 그리고 WeaponAugment도 클리어시키자;
        MyLog.Log("이벤트 발동 - 플레이어");
        //플레이어는 소환되고나서  이벤트를 2단계로 나눠야함. 
        //1단계: 증강카드에 어떤 플레이어 소환되는지. 정보만 설정.
        //1-2: 특정 증강 카드가 선택됨(어떤 플레이어 소환해라) 
        //2단계: 해당 플레이어를 소환해달라고 CharacterManager에 요청. 
        //3단계: 플레이어 소환이 끝났다면? 이제 여기서 아래의 것들을 실행하면 됨

        WeaponAugmentProcess(null); //증강 초기화. 다 비워준다.
        CharacterProcess(character); //플레이어 처리 
        CheckPanelClose();
        //이거 끝나고 CharacterManager에서 Spanwn 끝나면 OnCharacterSpawnCompleted 이벤트를 호출해줘서 다시 패널 띄워야하는지 체크할거임
    }
    private void OnCharacterSpawnCompleted(Character character)
    {
        //플레이어 소환 끝나면 해야할 행동들
    }


    public void OnSelectedAugment(WeaponAugment augment)
    {
        //증강 선택되었으므로 해당 증강 적용해주고 플레이어 정보는 싹 지워주자
        MyLog.Log("이벤트 발동 - 증강");
        WeaponAugmentProcess(augment);  //무기 선택된거 1개 제외 모두 다시 반환하도록 한다.
        CharacterProcess(null); //플레이어 null 처리로 클리어
        CheckPanelClose(); //패널 처리
    }
    private void WeaponAugmentProcess(WeaponAugment augment)
    {
        HashSet<Weapon> weaponNeedUptate = new HashSet<Weapon>();
        foreach (var item in selectedAugmentList)
        {
            if (item != augment)
            {
                //선택 안한 증강은 다시 넣어주자 
                item.weapon.ReturnAugment(item.dataSO);
            }
            weaponNeedUptate.Add(item.weapon);
        }
        //업데이트를 return 시켜줬기때문에 다시 다 Update 해줘야함.
        foreach (var weapon in weaponNeedUptate)
        {
            weapon.UpdateWeaponData();
        }
        ClearSelectAugmentList();
    }
    private void CharacterProcess(Character character)
    {
        foreach (var curCharacter in selectedCharacterList)
        {

            if (curCharacter == character)
            {
                //플레이어 소환 요청
                CharacterManager.Instance.SpawnCharacter(curCharacter);
                break;
            }
        }
        ClearSelectCharacterList();
    }

    private void CheckPanelClose()
    {
        augmentPanel.Close();
        if (maxShowCount <= 0)
        {
            Time.timeScale = 1f;
            isShowingPanel = false;
            return;
        }
        else
        {
            //연속 레벨업을 체크해야하므로 추가로 계속 해준다.
            ShowAugmentUI(); 
        }
    }
    private void ClearSelectCharacterList()
    {
        foreach(var item in selectedCharacterList)
        {
            if(item != null)
            {
                item.OnStartCharacterSpawn -= OnCharacterSelected;
            }
        }
        selectedCharacterList.Clear();
    }
    private void ClearSelectAugmentList()
    {
        foreach (var item in selectedAugmentList)
        {
            if (item != null)
            {
                item.OnSelected -= OnSelectedAugment;
            }
        }
        selectedAugmentList.Clear();
    }
}
