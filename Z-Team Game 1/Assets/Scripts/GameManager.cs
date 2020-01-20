using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Starting, Playing, Paused, Ended }

public class GameManager : Singleton<GameManager>
{
	public static readonly float gameObjectYPosition = -0.92f;

    [SerializeField] GameObject robotPrefab;

    /// <summary>
    /// The current state of the game
    /// </summary>
    public GameState CurrentState { get; private set; }

    private RobotManager robotManager;

    // Start is called before the first frame update
    void Start()
    {
        robotManager = new RobotManager(robotPrefab);
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
}
