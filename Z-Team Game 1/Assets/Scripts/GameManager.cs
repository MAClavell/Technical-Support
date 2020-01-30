using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Starting, Playing, Paused, Ended }

public class GameManager : Singleton<GameManager>
{
	public const float CONSTANT_Y_POS = -0.92f;

    [SerializeField] RobotSpawnZone[] robotSpawnZones;
    [SerializeField] GameObject robotPrefab;
    [SerializeField] GameObject towerPrefab;
    [SerializeField] GameObject zbuckPrefab;

    public Player Player { get; private set; }

    /// <summary>
    /// The current state of the game
    /// </summary>
    public GameState CurrentState { get; private set; }

    private List<GameObject> towers;
    private RobotManager robotManager;

    //Initialize vars
    private void Awake()
    {
        towers = new List<GameObject>();
        robotManager = new RobotManager(robotPrefab, robotSpawnZones);
    }

    // Start is called before the first frame update
    void Start()
    {
        NewGame();
        Player = GameObject.FindObjectOfType<Player>();
    }

    /// <summary>
    /// Reset data in the manager for a new game
    /// </summary>
    private void NewGame()
    {
        CurrentState = GameState.Starting;
        robotManager.Start();

        //Remove any towers
        foreach (var t in towers)
           Destroy(t);
        towers.Clear();
    }

    /// <summary>
    /// Begin the game
    /// </summary>
    private void BeginGame()
    {
        CurrentState = GameState.Playing;
    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentState)
        {
            case GameState.Starting:
                BeginGame();
                break;

            case GameState.Playing:
                robotManager.Update();

#if UNITY_EDITOR
                //Press 'R' to add zombies (debug only)
                if (Input.GetKeyDown(KeyCode.R))
                    robotManager.Spawn();

                if (Input.GetKeyDown(KeyCode.T))
                    SpawnZBucks(2, Vector3.zero, 1);
#endif
                break;

            case GameState.Paused:
                break;

            case GameState.Ended:
                break;

            default:
                Debug.LogError("Unknown game state reached. What did you do??");
                break;
        }
    }

    /// <summary>
    /// Spawns a tower into the world
    /// </summary>
    /// <param name="position">Position to spawn at</param>
    /// <param name="rotation">Rotation to spawn at</param>
    public void SpawnTower(Vector3 position, Quaternion rotation)
    {
        towers.Add(Instantiate(towerPrefab, position, rotation));
    }

    /// <summary>
    /// Spawn an amount of zbucks at a position
    /// </summary>
    /// <param name="amount">The amount to spawn</param>
    /// <param name="position">The center position</param>
    /// <param name="valuePerBuck">The value of each spawned zbuck</param>
    public void SpawnZBucks(ushort amount, Vector3 position, ushort valuePerBuck)
    {
        for(ushort i = 0; i < amount; i++)
        {
            //TODO: stop coins from going offscreen
            float angle = Random.Range(0, 360);
            Quaternion rotation = Quaternion.Euler(90, 0, angle);
            Vector3 target = position + (rotation * new Vector3(1, CONSTANT_Y_POS, 0));
            
            //Initialize the zbuck
            Instantiate(zbuckPrefab, position, rotation).GetComponent<ZBuck>().Init(target, valuePerBuck);
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Gizmos
    /// </summary>
    [ExecuteInEditMode]
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        foreach (var rsz in robotSpawnZones)
            Gizmos.DrawWireCube(new Vector3(rsz.position.x, CONSTANT_Y_POS, rsz.position.y), new Vector3(rsz.size.x * 2, 0, rsz.size.y * 2));
    }
#endif
}
