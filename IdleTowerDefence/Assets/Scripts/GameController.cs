using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	// Public Variables, setted from Main Camera's components
	public Player player;
	public Minion minionPrefab;

	// Minion amount in a wave
	private int WAVE_LENGTH = 5;

	public static float MAX_Z;
	public static float MIN_Z;
	public static float MAX_X;
	public static float MIN_X;

	// Stores minions
	private List<Minion> wave = new List<Minion> ();

	// If a minion survives from towers, the bool is set to true
	private bool minionSurvived = false;

	// Use this for initialization
	void Start ()
	{
		GameObject plane = (GameObject.FindGameObjectsWithTag ("Plane") [0] as GameObject);
		float difz = plane.GetComponent<Collider> ().bounds.size.z/2;
		float difx = plane.GetComponent<Collider> ().bounds.size.z/2;
		GameController.MAX_Z = plane.transform.position.z + difz;
		GameController.MIN_Z = plane.transform.position.z - difz;
		GameController.MAX_X = plane.transform.position.x + difx;
		GameController.MIN_X = plane.transform.position.x - difx;

		SendWave (true);
	}

	// Update is called once per frame
	void Update ()
	{
		
	}

	// Minion calls this function, when it is destroyed
	public void MinionDied (Minion minion,int currencyGivenOnDeath)
	{
		if (wave.Contains (minion)) {
			player.IncreaseCurrency(currencyGivenOnDeath);
			wave.Remove (minion);
			if (wave.Count == 0) {
				Debug.Log ("All Minions are Killed");
				SendWave ( minionSurvived );
			}
		}
	}

	// Minion calls this function, when it survives from Tower or Player
	public void MinionSurvived (Minion survivor)
	{
		minionSurvived = true;
		MinionDied (survivor, 0);
		Destroy (survivor.gameObject);
	}


	// Creates a new wave from the beginning point
	void SendWave ( bool reset )
	{
		if (!reset)
			WAVE_LENGTH++;
		for (int i = 0; i < WAVE_LENGTH; i++) {
			Vector3 instantPos = new Vector3 (minionPrefab.transform.position.x, minionPrefab.transform.position.y, minionPrefab.transform.position.z - i);
			Minion clone = Instantiate (minionPrefab, instantPos, Quaternion.identity) as Minion;
			clone.tag = "Minion";
			wave.Add(clone);
		}
	}

	// Returns if the given gameObject is on Map
	public static bool onMap(GameObject gameObject)
	{
		Vector3 pos = gameObject.transform.position;
		if (pos.x < MAX_X && pos.x > MIN_X && pos.z > MIN_Z && pos.z < MAX_Z)
			return true;
		else
			return false;
	}

	// Returns if there are any Minion on Map
	public static bool anyMinionOnMap ()
	{
		foreach ( GameObject minion in GameObject.FindGameObjectsWithTag ("Minion")){
			if (onMap (minion))
				return true;
		}
		return false;
	}

}

