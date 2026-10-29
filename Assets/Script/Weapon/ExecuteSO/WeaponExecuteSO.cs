using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponExecuteSO : ScriptableObject
{
    //총알 발사 로직은 대부분 같고, 여러번 발사할때 어떤 기준으로 적을 선택하는지와 총알 발사 정보를 어떤식으로 설정하는지만 다르다
    [SerializeField] EffectScript shootEffect; //무기 발사할때 이펙트가 필요하면
    public IEnumerator ShootWeapon(Weapon weapon,AudioClip shootSound, AudioClip destroySound)
    {
        while (true)
        {
            int attackCount = weapon.runtimeData.plusAttackCount;
            List<Transform> enemyTargetList = TargetManager.Instance.GetEnemyTarget(weapon.WeaponDataSO.baseRange, attackCount);
            //무기 쿨타임이 지났고, 타겟을 확인헀다면 발사
            if (Time.time >= weapon.nextAttackTime && enemyTargetList != null && enemyTargetList.Count > 0)
            {
                //스페셜 공격중 레이저를 쓴다고 생각해보자. -> GrowingLaser.cs 씀
                Transform fixedTarget = enemyTargetList[0];
                for (int i = 0; i < attackCount; i++)
                {
                    Transform trf = GetTarget(enemyTargetList, fixedTarget, i);
                    if (trf == null)
                    {
                        trf = weapon.FindExistTransform(enemyTargetList);
                        if (trf == null)
                        {
                            yield return weapon.continuousFireDelay;
                            continue;
                        }
                    }
                    ShootAt(trf, weapon, destroySound);
                    weapon.SetWeaponCoolTime();
                    weapon.InvokeWeaponShootEvent();
                    if(shootEffect != null)
                    {
                        EffectManager.Instance.Playfbx(shootEffect,weapon.transform); //이펙트 추가
                    }
                    if(shootSound != null)
                    {
                        SoundManager.Instance.PlaySFX(shootSound);
                    }
                    yield return weapon.continuousFireDelay;
                }
            }
            yield return null;
        }
    }
    protected virtual Transform GetTarget(List<Transform> enemyTransformList, Transform fixedTarget, int index)
    {
        int idx = index % enemyTransformList.Count;
        return enemyTransformList[idx];
    }

    protected abstract void ShootAt(Transform enemyTarget, Weapon weapon, AudioClip destroySound);

}