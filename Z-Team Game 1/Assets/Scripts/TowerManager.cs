using System;
using System.Collections.Generic;
using UnityEngine;

class TowerManager
{
	List<Tower> towers;

	/// <summary>
	/// Create a manager to control tower spawning
	/// </summary>
	public TowerManager()
	{
		towers = new List<Tower>();
	}

	/// <summary>
	/// Destroy all towers in the scene
	/// </summary>
	public void Reset()
	{
		foreach (Tower t in towers)
			UnityEngine.Object.Destroy(t.gameObject);
		towers.Clear();
	}
}
