using System.Collections.Generic;
using UnityEngine;

public class CaracterSelectButtonSpawner : MonoBehaviour
{
    public static CaracterSelectButtonSpawner Instance { get; private set; }
    [SerializeField] CharacterSelectButton characterSelectButton;
    [SerializeField] int poolSize = 15;

    ObjPool<CharacterSelectButton> selectButtonPool;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        selectButtonPool = new ObjPool<CharacterSelectButton>(characterSelectButton, poolSize, gameObject.transform);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public CharacterSelectButton GetPrefabByPool(Transform parentTransform)
    {
        CharacterSelectButton characterSelectButtonPrefab = selectButtonPool.GetObject();
        characterSelectButtonPrefab.transform.SetParent(parentTransform, false);
        //characterSelectButtonPrefab.transform.position = trf.position;
        return characterSelectButtonPrefab;
    }
    //오브젝트 반환 (반환이벤트없으면 public으로 써도됨)
    public void ReturnPrefabToPool(CharacterSelectButton selectButton)
    {
        selectButton.transform.SetParent(this.transform, false);
        selectButtonPool.ReturnObject(selectButton);
    }



}

