using System;
using UnityEngine.AddressableAssets;
using UnityEngine;

[Serializable]
public class AssetReferenceComponent<T> : AssetReferenceGameObject where T : Component
{
    //부모클래스가 guid 받는 생성자 요구해서 그대로 보내줌
    public AssetReferenceComponent(string guid) : base(guid) { }

    //인스펙터 드래그했을떄 Valid(유효)한 객체인지 체크하고 맞을때만 직렬화 넣을수있다
    public override bool ValidateAsset(UnityEngine.Object obj)
    {
        var go = obj as GameObject;
        return go != null && go.GetComponent<T>() != null;
    }
    //인스펙터 드래그 - string path 버전
    public override bool ValidateAsset(string path)
    {
#if UNITY_EDITOR
        var go = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
        return go != null && go.GetComponent<T>() != null;
#else
        return false;
#endif
    }
}

// 제네릭은 직렬화/드로어 문제가 생길 수 있어서 구체 클래스로 한 번 감싸기
[Serializable]
public class AssetReferenceCharacter : AssetReferenceComponent<Character>
{
    public AssetReferenceCharacter(string guid) : base(guid) { }
}
[Serializable]
public class AssetReferenceProjectile : AssetReferenceComponent<Projectile>
{
    public AssetReferenceProjectile(string guid) : base(guid){}
}