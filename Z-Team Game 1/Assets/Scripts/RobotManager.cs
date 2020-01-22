using UnityEngine;

class RobotManager
{
	private delegate void DecrementRobotDelegate();
	private static event DecrementRobotDelegate decrementRobotEvent;

	private RobotSpawnZone[] spawnZones;
	private GameObject robotPrefab;

	//Robot spawning
	private const uint MAX_ROBOTS = 500;
	private const uint SPAWN_TIME_DIVISOR = 10; //adds one more zombie for time / N. If N=10, then 20 seconds would add 2+1 robots
	private uint currAmount;
	private uint toSpawn;
	private uint maxToSpawnPerFrame;

	//Timers
	private const float ADD_PER_FRAME_MAX = 60; //every N seconds allow more robots to spawn per frame
	private float addPerFrameTimer;
	private const float ADD_TIMER_MAX = 10; //add robots to spawn queue every N seconds
	private float addTimer;
	private const float SPAWN_TIMER_MAX = 1; //spawns robots (if there are any to spawn) every N seconds
	private float spawnTimer;
	private double totalTime;


	/// <summary>
	/// Create a manager to control robot spawning
	/// </summary>
	/// <param name="robotPrefab">The basic robot prefab to spawn</param>
	public RobotManager(GameObject robotPrefab, RobotSpawnZone[] spawnZones)
	{
		this.robotPrefab = robotPrefab;

		//Setup event
		decrementRobotEvent += () => currAmount--;

		//Find spawn zones
		this.spawnZones = spawnZones;
	}

	/// <summary>
	/// Start the manager
	/// </summary>
	public void Start()
	{
		currAmount = 0;
		toSpawn = 0;
		maxToSpawnPerFrame = 1;

		totalTime = 0;
		addPerFrameTimer = 0;
		addTimer = 0;
		spawnTimer = 0;
	}

	/// <summary>
	/// Update the manager (controls robot spawning)
	/// </summary>
	public void Update()
	{
		totalTime += Time.deltaTime;
		addPerFrameTimer += Time.deltaTime;
		addTimer += Time.deltaTime;
		spawnTimer += Time.deltaTime;

		//Every N seconds allow more robots to spawn per frame, up to 10 robots
		if (addPerFrameTimer > ADD_PER_FRAME_MAX && maxToSpawnPerFrame < 10)
		{
			addPerFrameTimer -= ADD_PER_FRAME_MAX;
			maxToSpawnPerFrame++;
		}

		//Add robots to spawn queue
		if (addTimer > ADD_TIMER_MAX)
		{
			addTimer -= ADD_TIMER_MAX;
			toSpawn += (uint)(totalTime / SPAWN_TIME_DIVISOR) + 1;
		}

		//Spawn robots
		if(spawnTimer > SPAWN_TIMER_MAX)
		{
			spawnTimer -= SPAWN_TIMER_MAX;
			ushort currentSpawned = 0;
			while (toSpawn > 0 && currentSpawned < maxToSpawnPerFrame && currAmount < MAX_ROBOTS)
			{
				currentSpawned++;
				toSpawn--;
				Spawn();
			}
		}
	}

	/// <summary>
	/// Spawn a zombie at a random position
	/// </summary>
#if UNITY_EDITOR
	public void Spawn()
#else
	private void Spawn()
#endif
	{
		Object.Instantiate(robotPrefab, spawnZones[Random.Range(0, spawnZones.Length)].GetRandomPointInZone(), Quaternion.identity);
		currAmount++;
	}

	/// <summary>
	/// Decrement the robot count by 1
	/// </summary>
	public static void DecrementRobotCount()
	{
		decrementRobotEvent?.Invoke();
	}
}
