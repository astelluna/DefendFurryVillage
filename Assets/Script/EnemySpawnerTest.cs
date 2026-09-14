using System.Collections;
using UnityEngine;

public class EnemySpawnerTest : MonoBehaviour
{
    [SerializeField] GameObject obj;
    [SerializeField] float spawnTime = 5;
    WaitForSeconds delay;

    private void Start()
    {
        delay = new WaitForSeconds(spawnTime);
        StartCoroutine(spawnCor());
        
    }

    IEnumerator spawnCor()
    {
        while (true)
        {
            yield return delay;
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
