using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform enemy1;
    public Transform enemy2;
    [Space]
    public float initialCount;
    public float spawnInterval;
    [Space] 
    public Transform spot1;
    public Transform spot2;
    
    private enum enemyType
    {
        type1, type2
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(SpawnCycle());
    }

    private IEnumerator SpawnCycle()
    {
        for (int i = 0; i < initialCount; i++)
        {
            Spawn(enemyType.type1);
            Spawn(enemyType.type2);
        }
        
        if (spawnInterval <= 0)
            yield break;
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            Spawn(enemyType.type1);
            Spawn(enemyType.type2);
        }
    }

    private void Spawn(enemyType type)
    {
        var obj = type == enemyType.type1 ? enemy1 : enemy2;
        var pos = type == enemyType.type1 ? spot1 : spot2;
        var s = Instantiate(obj);
        var offset = Random.insideUnitCircle * 5;
        s.position = pos.position + new Vector3(offset.x, 0, offset.y);
    }
}
