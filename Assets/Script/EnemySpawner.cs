using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject obj;

    private void Start()
    {
        StartCoroutine(spawnCor());
    }

    IEnumerator spawnCor()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            for (int i = 0; i < 50; i++)
            {
                float value = Random.Range(0f, 10f);
                Vector3 pos = obj.transform.position;
                pos.x = value;
                obj.transform.position = pos;
                Instantiate(obj, transform);
            }
        }

    }
}
