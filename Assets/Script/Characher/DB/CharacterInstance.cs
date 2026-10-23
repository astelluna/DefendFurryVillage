using System;
using UnityEngine;

//캐릭터에 필요한거. 캐릭터Prefab을 관리하는 인게임전용, 캐릭터 매니저, DB를 위해-  캐릭터 인스턴스, 캐릭터 레벨DB

public class CharacterInstance
{
    //어떤 캐릭터가 어떤 레벨과 경험치를 가지고있고, 실제 SO데이터는 뭔지 넣어둔다.
    public CharacterDataSO characterData { get; private set; }
    public CharacterDataDb characterDataDb { get; private set; }
    public CharacterEquipment characterEquipment { get; private set; }

    private CharacterStat totalCharacterStat = new CharacterStat();

    //특정한 메서드 하나 부여해서, 언제든지 해당 클래스를 호출할수있게 해버리자 
    public CharacterStat GetCharacterTotalStat()
    {
        CharacterStat characterBaseStat = characterData.CharacterBaseStat;
        CharacterStat equipmentStat = characterEquipment.GetEquipmentStat();
        totalCharacterStat.SetFromSum(characterBaseStat, equipmentStat);
        return totalCharacterStat;
    }


    public CharacterInstance(CharacterDataSO soData, CharacterDataDb levelData)
    {
        if (soData == null) throw new ArgumentNullException(nameof(soData));
        if (levelData == null) throw new ArgumentNullException(nameof(levelData));

        if (soData.CharacterId != levelData.id) throw new ArgumentException($"DB와 SO의 ID가 다릅니다. SO:{soData.CharacterId}, DB:{levelData.id}");
        characterData = soData;
        characterDataDb = levelData;



        characterEquipment = new CharacterEquipment();
        characterEquipment.UpdateCharacterEquipment_DB(characterDataDb);

    }
}
