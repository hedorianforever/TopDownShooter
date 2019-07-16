using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;

public class LevelGenerator : MonoBehaviour
{
    enum gridSpace { empty, floor, wall };

    gridSpace[,] grid;
    //GameObject[,] gridObjs;
    List<GameObject> gridObjs = new List<GameObject>();
    int roomHeight, roomWidth;
    [SerializeField] Vector2 roomSizeWorldUnits = new Vector2(30, 30);
    float worldUnitsInOneGridCell = 1;

    struct walker
    {
        public Vector2 dir;
        public Vector2 pos;
    }

    //Making the walkers spawns more often and turn more often should generally make your rooms wider.
    List<walker> walkers;
    [SerializeField] float chanceWalkerChangeDir = 0.5f;
    [SerializeField] float chanceWalkerSpawn = 0.05f;
    [SerializeField] float chanceWalkerDestroy = 0.05f;
    [SerializeField] int maxWalkers = 10;
    [SerializeField] float percentToFill = 0.2f;

    [SerializeField] TileBase floorTile = default;
    [SerializeField] TileBase wallTile = default;
    [SerializeField] TileBase emptySpaceTile = default;

    [SerializeField] Tilemap groundTilemap = default;
    [SerializeField] Tilemap obstacleTilemap = default;
    [SerializeField] Tilemap spaceTilemap = default;
    

    [SerializeField] bool shouldRemoveSingleWalls = true;

    [Header("Enemies related variables")]
    [SerializeField] Transform playerTransform = default;

    //for percentage, add more of the same enemy ie 4 gunman prefabs and 1 summoner prefab for 80% chance to summon the gunman
    [SerializeField] Enemy[] enemies = default;

    [SerializeField] int maxNumberEnemies = 15;
    [SerializeField] int minNumberEnemies = 6;

    //chance to spawn enemy per tile
    [SerializeField] float chanceToSpawnEnemy = .05f;

    private float minEnemyDistanceFromPlayer = 12f;
    private int enemiesAlive = 0;


    private void Start()
    {
        Setup();
        CreateFloors();
        CreateWalls();
        if (shouldRemoveSingleWalls)
        {
            RemoveSingleWalls();
        }
        SpawnLevel();
    }

    private void Update()
    {
        Debug.Log("ENEMIES ALIVE: " + enemiesAlive);

        if (Input.GetKeyDown(KeyCode.R))
        {
            ClearGrid();
            Setup();
            CreateFloors();
            CreateWalls();
            if (shouldRemoveSingleWalls)
            {
                RemoveSingleWalls();
            }
            SpawnLevel();
        }
    }

    void ClearGrid()
    {
        foreach (GameObject go in gridObjs)
        {
            Destroy(go);
        }
        gridObjs.Clear();
    }

    void Setup()
    {
        //Clear the map (ensures we dont overlap)
        groundTilemap.ClearAllTiles();
        obstacleTilemap.ClearAllTiles();
        //find grid size
        roomHeight = Mathf.RoundToInt(roomSizeWorldUnits.x / worldUnitsInOneGridCell);
        roomWidth = Mathf.RoundToInt(roomSizeWorldUnits.y / worldUnitsInOneGridCell);
        //create grid
        grid = new gridSpace[roomWidth, roomHeight];
        //set grid's default state 
        for (int x = 0; x < roomWidth - 1; x++)
        {
            for (int y = 0; y < roomHeight - 1; y++)
            {
                //make every cell "empty"
                grid[x, y] = gridSpace.empty;
            }
        }

        //set first walker
        //init list
        walkers = new List<walker>();
        //create a walker
        walker newWalker = new walker();
        newWalker.dir = RandomDirection();
        //find center of grid
        Vector2 spawnPos = new Vector2(Mathf.RoundToInt(roomWidth / 2.0f),
                                       Mathf.RoundToInt(roomHeight / 2.0f));
        newWalker.pos = spawnPos;
        //add walker to list
        walkers.Add(newWalker);
    }

    void CreateFloors()
    {
        int iterations = 0; //loop will not run forever
        do
        {
            //create floor at every walker's position
            foreach (walker myWalker in walkers)
            {
                grid[(int)myWalker.pos.x, (int)myWalker.pos.y] = gridSpace.floor;
            }

            //chance to destroy walker
            int numberChecks = walkers.Count; //might modify count while in this loop
            for (int i = 0; i < numberChecks; i++)
            {
                //only if it's not the only one, and at a low chance
                if (Random.value < chanceWalkerDestroy && walkers.Count > 1)
                {
                    walkers.RemoveAt(i);
                    break; //only destroy one per iteration
                }
            }

            //chance: walker pick a new direction
            for (int i = 0; i < walkers.Count; i++)
            {
                if (Random.value < chanceWalkerChangeDir)
                {
                    walker thisWalker = walkers[i];
                    thisWalker.dir = RandomDirection();
                    walkers[i] = thisWalker;
                }
            }

            //chance: spawn new walker
            numberChecks = walkers.Count; //might modify while in this loop
            for (int i = 0; i < numberChecks; i++)
            {
                //only if # of walkers < max, and at a low chance
                if (Random.value < chanceWalkerSpawn && walkers.Count < maxWalkers)
                {
                    //create a walker at current walker's position
                    walker newWalker = new walker();
                    newWalker.dir = RandomDirection();
                    newWalker.pos = walkers[i].pos;
                    walkers.Add(newWalker);
                }
            }

            //move walkers
            for (int i = 0; i < walkers.Count; i++)
            {
                walker thisWalker = walkers[i];
                thisWalker.pos += thisWalker.dir;
                walkers[i] = thisWalker;
            }

            //avoid boarder of grid
            for (int i = 0; i < walkers.Count; i++)
            {
                walker thisWalker = walkers[i];
                //clamp x,y to leave a 1 space border: leave room for walls
                thisWalker.pos.x = Mathf.Clamp(thisWalker.pos.x, 1, roomWidth - 2);
                thisWalker.pos.y = Mathf.Clamp(thisWalker.pos.y, 1, roomHeight - 2);
                walkers[i] = thisWalker;
            }

            //check to exit loop
            if ((float)NumberOfFloors() / (float)grid.Length > percentToFill)
            {
                break;
            }
            iterations++;
        } while (iterations < 100000);
    }

    void CreateWalls()
    {
        //loop through every grid space
        for (int x = 0; x < roomWidth - 1; x++)
        {
            for (int y = 0; y < roomHeight - 1; y++)
            {
                //if there's a floor, check the spaces around it 
                if (grid[x, y] == gridSpace.floor)
                {
                    //if any surrounding spaces are empty, place a wall
                    if (grid[x, y + 1] == gridSpace.empty)
                    {
                        grid[x, y + 1] = gridSpace.wall;
                    }
                    if (grid[x, y - 1] == gridSpace.empty)
                    {
                        grid[x, y - 1] = gridSpace.wall;
                    }
                    if (grid[x + 1, y] == gridSpace.empty)
                    {
                        grid[x + 1, y] = gridSpace.wall;
                    }
                    if (grid[x - 1, y] == gridSpace.empty)
                    {
                        grid[x - 1, y] = gridSpace.wall;
                    }
                }
            }
        }
    }

    //could be a create obstacle where there is a single wall instead
    void RemoveSingleWalls()
    {
        //loop through every grid space
        for (int x = 0; x < roomWidth - 1; x++)
        {
            for (int y = 0; y < roomHeight - 1; y++)
            {
                //if there's a wall, check the spaces around it
                if (grid[x, y] == gridSpace.wall)
                {
                    //assume all spaces around the wall are floors
                    bool allFloors = true;
                    //check each side to see if they are all floors 
                    for (int checkX = -1; checkX <= 1; checkX++)
                    {
                        for (int checkY = -1; checkY <= 1; checkY++)
                        {
                            if (x + checkX < 0 || x + checkX > roomWidth - 1 ||
                                y + checkY < 0 || y + checkY > roomHeight - 1)
                            {
                                //skip checks that are out of range
                                continue;
                            }
                            if ((checkX != 0 && checkY != 0) || (checkX == 0 && checkY == 0))
                            {
                                //skip corners and center
                                continue;
                            }
                            if (grid[x + checkX, y + checkY] != gridSpace.floor)
                            {
                                allFloors = false;
                            }
                        }
                    }
                    //if is single wall
                    if (allFloors)
                    {
                        grid[x, y] = gridSpace.floor;
                    }
                }
            }
        }
    }

    Vector2 RandomDirection()
    {
        //pick random int between 1 and 3
        int choice = Mathf.FloorToInt(Random.value * 3.99f);
        //use that int to choose direction
        switch (choice)
        {
            case 0: return Vector2.down;
            case 1: return Vector2.left;
            case 2: return Vector2.up;
            case 3: return Vector2.right;
            default: return Vector2.zero;
        }
    }

    int NumberOfFloors()
    {
        int count = 0;
        foreach (gridSpace space in grid)
        {
            if (space == gridSpace.floor)
            {
                count++;
            }
        }
        return count;
    }

    void SpawnLevel()
    {
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                switch (grid[x, y])
                {
                    case gridSpace.empty:
                        //Spawn(x, y, emptySpaceTile, spaceTilemap);

                        //for now, I want empty space to be walls
                        Spawn(x, y, wallTile, obstacleTilemap);
                        break;
                    case gridSpace.floor:
                        Spawn(x, y, floorTile, groundTilemap);
                        //SpawnEnemy(x, y);
                        break;
                    case gridSpace.wall:
                        //also spawning the floorTile below the wallTile in case the walltile has transparent background
                        Spawn(x, y, floorTile, groundTilemap);
                        Spawn(x, y, wallTile, obstacleTilemap);
                        break;
                }
            }
        }
        //for some reason it's not working if called immediately; the tilemap probably takes a while to recalculate everything
        Invoke("ScanLevel", .1f);
        Invoke("SpawnEnemies", .3f);

    }

    void SpawnEnemies()
    {
        Debug.Log("SPAWNENEMIES() CALLED WITH ENEMIES ALIVE == " + enemiesAlive);

        for (int x = 0; x < roomWidth - 1; x++)
        {
            for (int y = 0; y < roomHeight - 1; y++)
            {
                if (grid[x, y] == gridSpace.floor)
                {
                    SpawnEnemy(x, y);
                }
            }
        }

        if (enemiesAlive < minNumberEnemies)
        {
            SpawnEnemies();
        } else
        {
            //originally false for testing if spawned enemies are inside walls
            obstacleTilemap.GetComponent<TilemapCollider2D>().usedByComposite = true;
        }
    }

    void SpawnEnemy(int x, int y)
    {
        Vector2 offset = roomSizeWorldUnits / 2.0f;
        Vector2 spawnPos = new Vector2(x, y) * worldUnitsInOneGridCell - offset;

        float distanceToPlayer = Vector2.Distance(spawnPos, playerTransform.position);
        
        if (Random.value < chanceToSpawnEnemy && enemiesAlive < maxNumberEnemies && distanceToPlayer > minEnemyDistanceFromPlayer)
        {
            Enemy enemyToSpawn = enemies[Random.Range(0, enemies.Length)];
            Enemy spawnedEnemy = Instantiate(enemyToSpawn, spawnPos, Quaternion.identity);
            enemiesAlive++;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnedEnemy.transform.position, 1f);
            foreach (Collider2D col in colliders)
            {
                if (col.tag == "Obstacle")
                {
                    Destroy(spawnedEnemy.gameObject);
                    enemiesAlive--;
                }
            }
        }
    }

    void Spawn(float x, float y, TileBase tile, Tilemap tilemap)
    {
        Vector2 offset = roomSizeWorldUnits / 2.0f;
        //if x and y range from 0 to 30; world units from -15 to 15, for example
        Vector2 spawnPos = new Vector2(x, y) * worldUnitsInOneGridCell - offset; 

        tilemap.SetTile(new Vector3Int((int)spawnPos.x, (int)spawnPos.y, 0), tile);
    }

    void ScanLevel()
    {
        if (AstarPath.active != null)
        {
            AstarPath.active.Scan();
        }
        else
        {
            Debug.LogError("No active AstarPath found in the scene");
        }
    }

}
