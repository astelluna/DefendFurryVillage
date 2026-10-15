using UnityEngine;

public class CharacterLevelDataDB
{
    //DB에서 불러온 캐릭터 레벨 데이터
    public string id;
    public int level;
    public int exp;
    public CharacterLevelDataDB(string characterId, int characterLevel, int characterExp)
    {
        id = characterId;
        level = characterLevel;
        exp = characterExp;
    }
}