using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectPanel : MonoBehaviour
{
    [SerializeField] StatIconAll statIconAll;
    [SerializeField] SkillIconAll skillIconAll;

    //가지고있는 캐릭터가 표시되는 스코롤뷰
    [SerializeField] CharacterSelectScrollView characterSelectScrollView;
    //선택한 캐릭터를 표시하는 UI매니저
    [SerializeField] CharacterSelectedUIManager characterSelectedUIManager;

    private void Start()
    {
        Invoke(nameof(TestDelayFunc), 10f);
    }

    private void TestDelayFunc()
    {
        characterSelectScrollView.CharacterShow(this);
    }

    public void SelectCharacterAndShowInfo(CharacterSelectButton selectButton, CharacterDataSO characterDataSO)
    {
        //캐릭터 정보 표시
        statIconAll.SetIcon(characterDataSO);
        skillIconAll.SetIcon(characterDataSO);

        //선택한 리스트에 하나씩 추가를 해준다.
        if(characterSelectedUIManager.SetData(characterDataSO, out bool selected))
        {
            //변했으면 패널에 변경점이 있음
            selectButton.UpdateCheckImage(selected);
        }

    }
}
