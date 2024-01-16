using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

public class BombScript : MonoBehaviour
{
    public bool destroyed = false;

    [SerializeField] private GameObject explosion;
    private TileNode[,] grid = null;
    private Vector2Int pos;

    public UnityEvent onExplode;
    // Start is called before the first frame update

    public void GetGrid(TileNode[,] fieldGrid, Vector2Int position)
    {
        grid = fieldGrid;
        pos = position;
    }

    public void OnMove() // Make one tick with every step
    {
        GameObject obj = this.transform.GetChild(2).gameObject;
        TMPro.TextMeshProUGUI text = obj.GetComponent<TMPro.TextMeshProUGUI>();
        string s = text.text;
        int x = Int16.Parse(s);
        x--;
        if (x < 1)
            Destroy();
        else
            text.text = x.ToString();
    }

    private void Destroy()
    {
        destroyed = true;
        Explode();
        this.gameObject.GetComponent<MeshRenderer>().enabled = false; // Make body invisble to make audio still play after Destroy
        for (int i = 0; i < transform.childCount-1; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(false);
        }
        Destroy(this.gameObject,2f);
    }

    void Explode()
    {
        onExplode.Invoke(); //To create sound

        for (int i = 0; i < 3; i++)
        {
            if (pos.x + i >= grid.GetLength(0)) // Check for out of bounds
                break;
            
            if (!grid[pos.x + i, pos.y].walkable) // Destory wooden box and no reaction to iron box
            {

                if (grid[pos.x + i, pos.y].canDestroy)
                {
                    grid[pos.x + i, pos.y].walkable = true;
                    Destroy(grid[pos.x + i, pos.y].body);
                }
                break;
            }
            if (grid[pos.x + i, pos.y].enemyState == 1) //Check if enemy was on tile
                grid[pos.x + i, pos.y].enemyState = 2;
            grid[pos.x + i, pos.y].explode = true; // Flag for player to know if they are damaged

            GameObject explode = GameObject.Instantiate(explosion, grid[pos.x+i, pos.y].worldPosition, Quaternion.identity); // Create a visual effect of explosion
            ParticleSystem exp = explosion.GetComponent<ParticleSystem>();
            exp.Play();
            Destroy(explode, exp.main.duration);
        }
        for (int i = 0; i < 3; i++)
        {
            if (pos.x - i < 0)
                break;
           
            if (!grid[pos.x - i, pos.y].walkable)
            {

                if (grid[pos.x - i, pos.y].canDestroy)
                {
                    grid[pos.x - i, pos.y].walkable = true;
                    Destroy(grid[pos.x - i, pos.y].body);
                }
                break;
            }
            if (grid[pos.x - i, pos.y].enemyState == 1)
                grid[pos.x - i, pos.y].enemyState = 2;
            grid[pos.x - i, pos.y].explode = true;

            GameObject explode = GameObject.Instantiate(explosion, grid[pos.x - i, pos.y].worldPosition, Quaternion.identity);
            ParticleSystem exp = explosion.GetComponent<ParticleSystem>();
            exp.Play();
            Destroy(explode, exp.main.duration);
        }
        for (int i = 0; i < 3; i++)
        {
            if (pos.y - i < 0)
                break;
            
            if (!grid[pos.x, pos.y - i].walkable)
            {

                if (grid[pos.x, pos.y - i].canDestroy)
                {
                    grid[pos.x, pos.y - i].walkable = true;
                    Destroy(grid[pos.x, pos.y - i].body);
                }
                break;
            }
            if (grid[pos.x, pos.y - i].enemyState == 1)
                grid[pos.x, pos.y - i].enemyState = 2;
            grid[pos.x, pos.y - i].explode = true;

            GameObject explode = GameObject.Instantiate(explosion, grid[pos.x, pos.y - i].worldPosition, Quaternion.identity);
            ParticleSystem exp = explosion.GetComponent<ParticleSystem>();
            exp.Play();
            Destroy(explode, exp.main.duration);
        }
        for (int i = 0; i < 3; i++)
        {
            if (pos.y + i > grid.GetLength(1)-1)
                break;

            if (!grid[pos.x, pos.y + i].walkable)
            {

                if (grid[pos.x, pos.y + i].canDestroy)
                {
                    grid[pos.x, pos.y + i].walkable = true;
                    Destroy(grid[pos.x, pos.y + i].body);
                }
                break;
            }

            if (grid[pos.x, pos.y + i].enemyState == 1)
                grid[pos.x, pos.y + i].enemyState = 2;
            grid[pos.x, pos.y + i].explode = true;

            GameObject explode = GameObject.Instantiate(explosion, grid[pos.x, pos.y + i].worldPosition, Quaternion.identity);
            ParticleSystem exp = explosion.GetComponent<ParticleSystem>();
            exp.Play();
            Destroy(explode, exp.main.duration);
        }
    }
}
