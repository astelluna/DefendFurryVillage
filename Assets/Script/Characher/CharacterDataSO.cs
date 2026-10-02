using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Character/CharacterDataSO")]
public class CharacterDataSO : ScriptableObject
{
    //표시되는 정보 
    [Header("캐릭터 정보")]
    [field: SerializeField] public string CharacterId { get; private set; }
    [field: SerializeField] public string CharacterName { get; private set; }
    [Header("캐릭터 이미지")]
    [field: SerializeField] public Sprite CharacterBodySprite { get; private set; }
    [field: SerializeField] public Sprite CharacterPortraitSprite { get; private set; }
    [Header("캐릭터 프리팹")]
    //[field: SerializeField] public Character CharacterPrefab;
    //[field: SerializeField] public AssetReferenceT<Character> CharacterRefecrencePrefab { get; private set; }
    [field: SerializeField] public AssetReferenceCharacter CharacterReferencePrefab { get; private set; }
    //public Character CharacterDataPrefab { get; private set; }

    //개선 : 캐릭터의 WeaponSO에 대해서 가지고있을거임.
    [Header("캐릭터 무기 데이터SO")]
    [field: SerializeField] public WeaponDataSO BasicWeaponDataSO { get; private set; }
    [field: SerializeField] public WeaponDataSO SpecialWeaponDataSO { get; private set; }


    //인게임 정보
    [Header("아래부터 인게임 정보")]
    [field: SerializeField] public int attack { get; private set; } = 10; //플레이어 기본 공격력
    [field: Header("크리확률 0~100까지 설정")]
    [field: SerializeField] public int criticalPercent { get; private set; } = 0; //크리 확률 0~100
    [field: Header("크리 데미지 배율 예시) 2.0 이렇게 설정(2배)")]
    [field: SerializeField] public float criticalDamageRate { get; private set; } = 2.0f; //크리 데미지 배율 2.0 -> (2배 데미지)
    [field:Header("방어관통 0~100까지 설정 (float 가능)")]
    [field: SerializeField] public float armorPenetration { get; private set; } = 0; //방어 관통률 0~100; 35.5% -> DamageManager에서 0.355로 바꿔서 계산할거임

}
