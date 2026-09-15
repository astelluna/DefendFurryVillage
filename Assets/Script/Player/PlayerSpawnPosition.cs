using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPosition : MonoBehaviour
{
    [SerializeField] private List<Transform> transformList = new List<Transform>();

    public Transform GetTransform(int index)
    {
        if (index >= transformList.Count) return null;
        return transformList[index];
    }

}
