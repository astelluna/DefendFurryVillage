using System.Collections.Generic;
using UnityEngine;

public class AugmentPanel : MonoBehaviour
{
    [SerializeField] List<AugmentUI> augmentList = new List<AugmentUI>();

    public void AddData(WeaponAugment weaponAugment,Sprite characterSprite, int index)
    {
        if(index >= augmentList.Count)
        {
            MyLog.LogError("증강 UI index가 너무 큽니다");
            return;
        }
        augmentList[index].SetData(weaponAugment, characterSprite);
        augmentList[index].gameObject.SetActive(true);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        //이제 여기서 증강의 갯수에 따라서 n개만 표시를 해야한다.

    }
    public void Close()
    {
        foreach(var item in augmentList)
        {
            item.gameObject.SetActive(false);
        }
        this.gameObject.SetActive(false);

    }
}
