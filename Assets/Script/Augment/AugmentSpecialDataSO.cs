using UnityEngine;
public interface IOnHit
{
    public void OnHit(WeaponRuntimeData runtimeData, Enemy enemy);
}
public abstract class AugmentSpecialDataSO : ScriptableObject
{
    //public string dataId;

}