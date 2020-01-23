using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Starting, Playing, Paused, Ended }

public class GameManager : Singleton<GameManager>
{
	public static readonly float CONSTANT_Y_POS = -0.92f;
	public static readonly ushort MAX_TOWERS = 100;

    [SerializeField] GameObject robotPrefab;
    [SerializeField] RobotSpawnZone[] robotSpawnZones;

    public Player Player { get; private set; }

    /// <summary>
    /// The current state of the game
    /// </summary>
    public GameState CurrentState { get; private set; }

    private List<Tower> towers;
    private RobotManager robotManager;

    //Initialize vars
    private void Awake()
    {
        towers = new List<Tower>();
        Player = GameObject.FindObjectOfType<Player>();
        robotManager = new RobotManager(robotPrefab, robotSpawnZones);
    }

    // Start is called before the first frame update
    void Start()
    {
        NewGame();
    }

    /// <summary>
    /// Reset data in the manager for a new game
    /// </summary>
    private void NewGame()
    {
        CurrentState = GameState.Starting;
        robotManager.Start();

        //Remove any towers
        foreach (Tower t in towers)
           Destroy(t.gameObject);
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
