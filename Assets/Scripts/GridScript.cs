using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GridScript : MonoBehaviour
{
    [SerializeField] private GameObject tileModel;
    [SerializeField] private GameObject ironModel;
    [SerializeField] private GameObject woodModel;

    [SerializeField] private Transform landscape;
    [SerializeField] private GameObject field;
    [SerializeField] private int gridDelta = 20;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemy;

    private TileNode[,] grid = null;
    private List<GameObject> enemies = new List<GameObject>();

    private float enemyTimer = 5;

    public UnityEvent onPlayerDead;
    public UnityEvent onVictory;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 size = landscape.GetComponent<Renderer>().bounds.size;
        int sizeX = (int)(size.x / gridDelta);
        int sizeZ = (int)(size.z / gridDelta);
        grid = new TileNode[sizeX, sizeZ];
        for (int x = 0; x < sizeX; ++x)
            for (int z = 0; z < sizeZ; ++z)
            {
                Vector3 position = new Vector3(x * gridDelta, 0, z * gridDelta);
                position.y = 5;
                if (x % 2 != 0 && z % 2 != 0) 
                    grid[x, z] = new TileNode(ironModel, false, false, position, field); //Spawn iron boxes
                else
                {
                    if (UnityEngine.Random.Range(0, 5) ==4 && x!=0 && x< grid.GetLength(0)-2)
                    {
                        grid[x, z] = new TileNode(woodModel, false, true, position, field); //Spawn wooden boxes
                    }
                    else
                        grid[x, z] = new TileNode(tileModel, true, true, position, field); //Spawn empy tiles
                }
            }
        player.GetComponent<PlayerScript>().getGrid(grid);
        player.GetComponent<PlayerScript>().StartOnGrid();
        player.GetComponent<PlayerScript>().enabled = false; //Place Player and wait for start

        GameObject startEnemy1 = GameObject.Instantiate(enemy, new Vector3(0f,0f,0f), Quaternion.identity);
        startEnemy1.GetComponent<EnemyScript>().StartWithGrid(grid);
        GameObject startEnemy2 = GameObject.Instantiate(enemy, new Vector3(0f, 0f, 0f), Quaternion.identity);
        startEnemy2.GetComponent<EnemyScript>().StartWithGrid(grid); //Spawn Enemies

        while (startEnemy1.GetComponent<EnemyScript>().current == startEnemy2.GetComponent<EnemyScript>().current)
            startEnemy2.GetComponent<EnemyScript>().StartWithGrid(grid); //Respawn Enemy if they are in same tile

        enemies.Add(startEnemy1);
        enemies.Add(startEnemy2);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        player.GetComponent<PlayerScript>().enabled = true;
        Time.timeScale = 1;
    }

    public void Restart() //Respawn whole map
    {
        foreach (var tile in grid)
        {
            Destroy(tile.body);
        }
        grid = null;
        foreach (var enemy in enemies)
        {
            Destroy(enemy);
        }
        enemies = new List<GameObject>();
        player.GetComponent<PlayerScript>().isDead = false;
        Start();
        Resume();
    }

    public void TimeOut() // If time ran out
    {
        player.GetComponent<PlayerScript>().isDead = true;
        Time.timeScale = 0;
        onPlayerDead.Invoke();
    }


    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<PlayerScript>().isDead) //Check if player is dead
        {
            Time.timeScale = 0;
            onPlayerDead.Invoke();
            return;
        }

        if (player.GetComponent<PlayerScript>().current.x == grid.GetLength(0) - 1) //Check if player exited
        {
            onVictory.Invoke();
            return;
        }

        int j = 0;
        while (j < enemies.Count)
        {
            if (enemies[j].GetComponent<EnemyScript>().dead) //Check if enemies are dead and delete them
            {
                enemies[j].transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = Color.red;
                Destroy(enemies[j],1f);
                enemies.RemoveAt(j);
            }
            else
                j++;
        }

        if (player.GetComponent<PlayerScript>().moved) //Check if player has moved
        {

            foreach (var enemy in enemies) //Moving enemies
            {
                if (player.GetComponent<PlayerScript>().current == enemy.GetComponent<EnemyScript>().current)
                {
                    player.GetComponent<PlayerScript>().isDead = true;
                    onPlayerDead.Invoke();
                    return;
                }

                enemy.GetComponent<EnemyScript>().SetTarget(player.GetComponent<PlayerScript>().current);
                enemy.GetComponent<EnemyScript>().Move();

                if (player.GetComponent<PlayerScript>().current == enemy.GetComponent<EnemyScript>().current)
                {
                    player.GetComponent<PlayerScript>().isDead = true;
                    onPlayerDead.Invoke();
                    return;
                }
            }
            for (int i = 1; i < enemies.Count; ++i) //Move enemy if they are on same tile
            {
                if (enemies[i].GetComponent<EnemyScript>().current == enemies[i - 1].GetComponent<EnemyScript>().current)
                    enemies[i].GetComponent<EnemyScript>().MoveBack();

            }


            player.GetComponent<PlayerScript>().moved = false;
            foreach (var tile in grid)
                tile.explode = false;
            player.GetComponent<PlayerScript>().CheckBombs();
        }

        

        if (enemyTimer > 0)
            enemyTimer-= Time.deltaTime;
        else
        {   //Spawn new enemy every 5 seconds
            GameObject timeEnemy = GameObject.Instantiate(enemy, new Vector3(0f, 0f, 0f), Quaternion.identity);
            timeEnemy.GetComponent<EnemyScript>().StartWithGrid(grid);
            enemies.Add(timeEnemy);
            enemyTimer = 5;
        }

    }
}
