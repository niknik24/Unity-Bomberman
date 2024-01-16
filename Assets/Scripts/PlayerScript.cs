using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerScript : MonoBehaviour
{
    private TileNode[,] grid = null;
    [SerializeField] private GameObject bombModel;

    public Vector2Int current;
    public bool moved = false;
    public bool isDead = false;
    public UnityEvent onStep;

    private List<GameObject> bombs = new List<GameObject>();
    // Start is called before the first frame update
    public void StartOnGrid()
    {
        this.transform.position = grid[0,0].worldPosition; //Place player on map
        this.transform.position -= new Vector3(0, 5, 0);
        current = new Vector2Int(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }

        if (grid[current.x, current.y].explode) //Check if player got damaged by explosion
        {
            isDead = true;
            return;
        }

        List<Vector2Int> tiles = GetNeighbours(current);

        if (Input.GetKeyUp(KeyCode.Space)) //Place bomb
        {
            GameObject bomb = GameObject.Instantiate(bombModel, grid[current.x, current.y].worldPosition, Quaternion.identity);
            bomb.GetComponent<BombScript>().GetGrid(grid, current);
            bombs.Add(bomb);
        }

        if (Input.GetKeyUp(KeyCode.W)) //Move up
        {
            if (tiles[3].x != -1)
            {
                if (grid[tiles[3].x, tiles[3].y].walkable)
                {
                    this.transform.position = grid[tiles[3].x, tiles[3].y].worldPosition;
                    this.transform.position -= new Vector3(0, 5, 0);
                    current = tiles[3];
                }
            }
            this.transform.eulerAngles = new Vector3(-90.0f, 90.0f, -90.0f);
            moved = true;
            onStep.Invoke();
        }
        if (Input.GetKeyUp(KeyCode.S)) //Move down
        {
            if (tiles[2].x != -1)
            {
                if (grid[tiles[2].x, tiles[2].y].walkable)
                {
                    this.transform.position = grid[tiles[2].x, tiles[2].y].worldPosition;
                    this.transform.position -= new Vector3(0, 5, 0);
                    current = tiles[2];
                }
            }
            this.transform.eulerAngles =new Vector3(-90.0f, 90.0f, 90.0f);
            moved = true;
            onStep.Invoke();
        }
        if (Input.GetKeyUp(KeyCode.A)) //Move left
        {
            if (tiles[0].x != -1) {
                if (grid[tiles[0].x, tiles[0].y].walkable)
                {
                    this.transform.position = grid[tiles[0].x, tiles[0].y].worldPosition;
                    this.transform.position -= new Vector3(0, 5, 0);
                    current = tiles[0];
                }
            }
            this.transform.eulerAngles = new Vector3(-90.0f, 90.0f, 180.0f);
            moved = true;
            onStep.Invoke();
        }
        if (Input.GetKeyUp(KeyCode.D)) //Move right
        {

            if (grid[tiles[1].x, tiles[1].y].walkable && tiles[1].x != -1)
            {
                this.transform.position = grid[tiles[1].x, tiles[1].y].worldPosition;
                this.transform.position -= new Vector3(0, 5, 0);
                current = tiles[1];
            }
            this.transform.eulerAngles = new Vector3(-90.0f, 90.0f, 0.0f);
            moved = true;
            onStep.Invoke();
        }
    }

    private List<Vector2Int> GetNeighbours(Vector2Int current) //Getting neighbours of tile
    {
        List<Vector2Int> tiles = new List<Vector2Int>();
        for (int i = 0; i < 4; i++)
            tiles.Add(new Vector2Int(-1, 0));
        if (current.x != 0)
            tiles[0] = new Vector2Int(current.x - 1, current.y);
        if (current.x < grid.GetLength(0))
            tiles[1] = new Vector2Int(current.x + 1, current.y);
        if (current.y != 0)
            tiles[2] = new Vector2Int(current.x, current.y - 1);
        if (current.y < grid.GetLength(1)-1)
            tiles[3] = new Vector2Int(current.x, current.y + 1);
        return tiles;
    }

    public void CheckBombs() // Check bombs timer and explosion
    {
        int i = 0;
        while (i < bombs.Count)
        {
            bombs[i].GetComponent<BombScript>().OnMove();
            if (bombs[i].GetComponent<BombScript>().destroyed)
                bombs.RemoveAt(i);
            else
                i++;
        }
    }

    public void getGrid(TileNode[,] fieldGrid)
    {
        grid = fieldGrid;
    }
}
