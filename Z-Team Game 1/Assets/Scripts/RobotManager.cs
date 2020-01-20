using UnityEngine;

class RobotManager
{
	private GameObject robotPrefab;
	private RobotSpawnZone[] spawnZones;

	private uint totalSpawned;
	private double timePlaying;

	public RobotManager(GameObject robotPrefab)
	{
		this.robotPrefab = robotPrefab;
		spawnZones = GameObject.FindObjectsOfType<RobotSpawnZone>();
	}

	/// <summary>
	/// Start the 
	/// </summary>
	public void Start()
	{
		timePlaying = 0;
		totalSpawned = 0;

		for(int i = 0; i < 1000; i++)
		{
			int rand = Random.Range(0, spawnZones.Length);
			Object.Instantiate(robotPrefab, spawnZones[rand].GetRandomPointInZone(), Quaternion.identity);
		}
	}

	public void Update()
	{
		timePlaying += Time.deltaTime;
	}
}
