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
    [field: SerializeField] public AssetReferenceCharacter CharacterReferencePrefab { get; private set; }

    //개선 : 캐릭터의 WeaponSO에 대해서 가지고있을거임.
    [Header("캐릭터 무기 데이터SO")]
    [field: SerializeField] public WeaponDataSO BasicWeaponDataSO { get; private set; }
    [field: SerializeField] public WeaponDataSO SpecialWeaponDataSO { get; private set; }

    //캐릭터 가격 정보 
    [Header("캐릭터 구매 가격 정보")]
    [field: SerializeField] public Currency PurchasePrice { get; private set; }

    //인게임 정보
    [Header("인게임 정보")]
    [field: SerializeField] public CharacterStat CharacterBaseStat { get; private set; }
    [Header("사운드")]
    [field: SerializeField] public AudioClip AttackSound { get; private set; } //특수공격할때만 소리를 넣자

}
