using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] GameObject playerPortal = default;

    //private List<Enemy> enemies = new List<Enemy>();
    private int enemyCount = 0;
    private Player player;

    public void IncreaseEnemyCount()
    {
        enemyCount++;
    }

    public void SetEnemyCount(int n)
    {
        enemyCount = n;
    }

    public void DecreaseEnemyCount(Vector3 lastEnemyPosition)
    {
        enemyCount--;
        if (enemyCount == 0) {
            Instantiate(playerPortal, lastEnemyPosition, Quaternion.identity);
        }
    }
}
