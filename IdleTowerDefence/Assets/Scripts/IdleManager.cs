using UnityEngine;
using System.Collections;

namespace Assets.Scripts {
public class IdleManager : MonoBehaviour {

	//Idle Income Calculation
	BigIntWithUnit _maxPotentialWaveDmg;
	BigIntWithUnit MageDPS;
	int MageAttackDuration;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		}

	//Idle Functionality Preparations

	public void OnApplicationQuit(){
		PlayerPrefs.SetString ("GameCloseTime", System.DateTime.Now.ToString());
	}

	public void CalculateIdleIncome(){
		PlayerPrefs.GetString ("GameCloseTime");
			_maxPotentialWaveDmg = MageAttackDuration * MageDPS;

			if (_maxPotentialWaveDmg> WaveManager.WaveLife && WaveManager.CurrentWave%4 != 0) {
			WaveManager.CurrentWave++;
			
		}
	}
}
}
