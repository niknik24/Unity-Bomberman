using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{

    public Vector2Int current;
    private Vector2Int target;

    public bool dead = false;

    private TileNode[,] grid = null;
    private Vector2Int prev;

    // Start is called before the first frame update
    public void StartWithGrid(TileNode[,] fieldGrid) //Initialize enemy on grid
    {
        grid = fieldGrid;
        List<int> list = new List<int>();
        for (int i = 0; i < grid.GetLength(1); i++)
            if (i %2 == 0)
                list.Add(i);
        current = new Vector2Int(grid.GetLength(0) - 1, list[UnityEngine.Random.Range(0, list.Count)]) ;
        this.transform.position = grid[current.x, current.y].worldPosition;
        this.transform.position += new Vector3(0, 5, 0);
        this.transform.eulerAngles = new Vector3(90.0f, 0, 0);
        target = new Vector2Int(0, 0);
        //Move();
    }

    // Update is called once per frame
    void Update() //Check if tile has explosion
    {
       if (grid[current.x, current.y].enemyState == 2)
        {
            dead = true;
            grid[current.x, current.y].enemyState = 0;
        }
    }

    public void SetTarget(Vector2Int _target) //Set target of enemy
    {
        target = _target;
    }

    public void Move()
    {
        Vector2Int currentTile = target;
        List<Vector2Int> tiles = new List<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> trace = FindPath();
        if (trace == null)
            return;
        
        if (!trace.ContainsKey(currentTile)) // Check if enemy can reach target. If not then go as far left as possible
        {
            List<Vector2Int> keyX = new List<Vector2Int>(trace.Keys);
            Vector2Int min = new Vector2Int(0,0);
            int minx = 21;
            foreach (var key in keyX) {
                if (key.x < minx)
                {
                    min = key;
                    minx = key.x;
                }
            }
            currentTile = min;
            tiles.Add(currentTile);
        }
        
        while (currentTile.x != -1) {
            currentTile = trace[currentTile];
            tiles.Add(currentTile);
        }
        if (tiles.Count < 3 ) {
            return;
        }
        Vector2Int step = tiles[tiles.Count - 3];
        grid[prev.x, prev.y].hasEnemy = false;
        prev = current;
        current = step;
        this.transform.position = grid[step.x, step.y].worldPosition; // Make step to target
        this.transform.position += new Vector3(0, 5, 0);
        grid[current.x, current.y].enemyState = 1;
    }

    public void MoveBack() // Make step back if tile is already taken
    {
        current = prev;
        this.transform.position = grid[current.x, current.y].worldPosition;
        this.transform.position += new Vector3(0, 5, 0);
    }

    public Dictionary<Vector2Int, Vector2Int> FindPath() //Finding path to target
    {
        TileNode start = grid[current.x, current.y];
        Dictionary<Vector2Int, Vector2Int> trace = new Dictionary<Vector2Int, Vector2Int>(); // Recollection of paths
        Queue<Vector2Int> tiles = new Queue<Vector2Int>(); // All tiles

        tiles.Enqueue(current);
        trace.Add(current, new Vector2Int(-1, -1));
        

        while (tiles.Count > 0)
        {
            Vector2Int currentTile = tiles.Dequeue();

            if (currentTile == target)
                break;

            var neighbours = GetNeighbours(currentTile);
            foreach (var tile in neighbours)
                if (grid[tile.x, tile.y].walkable)
                {
                    if (trace.ContainsKey(tile))
                        continue;
                    trace.TryAdd(tile, currentTile);
                    tiles.Enqueue(tile);
                }
        }
        return trace;
    }

    private List<Vector2Int> GetNeighbours(Vector2Int current) //Getting neighbours of tile
    {
        List<Vector2Int> tiles = new List<Vector2Int>();
        if (current.x != 0)
            if (grid[current.x - 1, current.y].walkable)
                tiles.Add(new Vector2Int(current.x - 1, current.y));
        if (current.x < grid.GetLength(0)-1)
            if (grid[current.x + 1, current.y].walkable)
                tiles.Add(new Vector2Int(current.x + 1, current.y));
        if (current.y != 0)
            if (grid[current.x, current.y - 1].walkable)
            tiles.Add(new Vector2Int(current.x, current.y - 1));
        if (current.y < grid.GetLength(1)-1)
            if (grid[current.x, current.y + 1].walkable)
            tiles.Add(new Vector2Int(current.x, current.y + 1));
        return tiles;
    }
}
