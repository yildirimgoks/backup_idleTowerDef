using UnityEngine;
using System.Collections;

public class Minion : MonoBehaviour {

    public Player Player;
    public TextMesh healthIndicator;

    public int CurrencyGivenOnDeath = 5;
    public int Life = 100;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        healthIndicator.text = "" + Life;
        if(Life <= 0)
        {
            Destroy(gameObject);
        }
	}

    void OnDestroy() {
        Player.IncreaseCurrency(CurrencyGivenOnDeath);
    }
}
