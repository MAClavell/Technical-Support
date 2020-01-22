using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Starting, Playing, Paused, Ended }

public class GameManager : Singleton<GameManager>
{
	public static readonly float gameObjectYPosition = -0.92f;

    [SerializeField] GameObject robotPrefab;

    [SerializeField] RobotSpawnZone[] robotSpawnZones;

    /// <summary>
    /// The current state of the game
    /// </summary>
    public GameState CurrentState { get; private set; }

    private RobotManager robotManager;

    // Start is called before the first frame update
    void Start()
    {
        robotManager = new RobotManager(robotPrefab, robotSpawnZones);
        NewGame();
    }

    /// <summary>
    /// Reset data in the manager for a new game
    /// </summary>
    private void NewGame()
    {
        CurrentState = GameState.Starting;
        robotManager.Start();
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
        foreach (var rsz in robotSpawnZones)
            Gizmos.DrawWireCube(new Vector3(rsz.position.x, gameObjectYPosition, rsz.position.y), new Vector3(rsz.size.x * 2, 0, rsz.size.y * 2));
    }
#endif
}
