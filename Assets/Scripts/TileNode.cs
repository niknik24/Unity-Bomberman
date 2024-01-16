using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileNode //: MonoBehaviour
{
    public bool canDestroy;
    public bool walkable;
    public Vector3 worldPosition;
    private GameObject prefab;
    public GameObject body;
    public bool explode = false;
    public bool hasEnemy = false;
    public int enemyState = 0;  // 1 = noEnemy, 2 = hasEnemy, 3 = deadEnemy


    public TileNode(GameObject _prefab, bool _walkable, bool _canDestroy, Vector3 position, GameObject Parent)
    {
        prefab = _prefab;
        walkable = _walkable;
        worldPosition = position;
        canDestroy = _canDestroy;
        body = GameObject.Instantiate(prefab, worldPosition, Quaternion.identity);
        body.transform.SetParent(Parent.transform, true);
    }

}
