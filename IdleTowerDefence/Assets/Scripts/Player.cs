using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public Rigidbody playerSpellPrefab;
	public Minion minionPrefab;
	public GameObject floor;
    private int Currency = 0;

	// Minion amount in a wave
	private int WAVE_LENGTH = 30;

	private static float MAX_Z;
	private static float MIN_Z;
	private static float MAX_X;
	private static float MIN_X;

	// Stores minions
	private List<Minion> wave = new List<Minion> ();

	// If a minion survives from towers, the bool is set to true
	// It is used for reseting the wave.
	private bool minionSurvived = false;

	// Use this for initialization
	void Start () {
		// Calculating area of the Plane
		GameObject plane = (GameObject.FindGameObjectsWithTag ("Floor") [0] as GameObject);
		float difz = plane.GetComponent<Collider> ().bounds.size.z/2;
		float difx = plane.GetComponent<Collider> ().bounds.size.x/2;
		MAX_Z = plane.transform.position.z + difz;
		MIN_Z = plane.transform.position.z - difz;
		MAX_X = plane.transform.position.x + difx;
		MIN_X = plane.transform.position.x - difx;

		SendWave (true);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			
			/**
			Vector3 planeInPoint = floor.transform.position;
			Plane floorPlane = new Plane (floor.GetComponent<MeshFilter> ().mesh.normals [0], planeInPoint);
			Ray floorRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			float rayDistance;
			if (floorPlane.Raycast(floorRay, out rayDistance)) {
				Vector3 spellSpawnPoint = floorRay.GetPoint(rayDistance);
				spellSpawnPoint.y = spellSpawnPoint.y + 5;
				Instantiate (playerSpellPrefab, spellSpawnPoint, Quaternion.identity);
			} 
			**/
		}
	}
		

	// Minion calls this function, when it is destroyed
	public void MinionDied (Minion minion, int currencyGivenOnDeath)
	{
		if (wave.Contains (minion)) {
			IncreaseCurrency(currencyGivenOnDeath);
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
	// If reset is true, the amount of minions in a wave doesn't change.
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

    public void IncreaseCurrency(int amount)
    {
        Currency += amount;
    }

    public void DecreaseCurrency(int amount)
    {
        Currency -= amount;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), "Currency: " + Currency);
    }
}
