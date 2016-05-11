using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public Rigidbody playerSpellPrefab;
    private int Currency = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			Vector3 mousePos = Input.mousePosition;
			// Mouse Position'u boyle ayarlayinca, aslinda gercek haritadaki noktaya donusturmuyor.
			// Kameradan 8 birim asagida bir plane dusunup, oradaki x,y,z degerlerine bakiyor.
			// Haliyle bizim gercekten tabanda olan x,y ve z degerlerini vermiyor.
			// Bundan dolayi da cogu zaman en yakindaki minionu en ondeki zannediyor.
			// Debuglamak sar
			mousePos.z = 8f;

			// Bu sekilde ayarlarinca dogru miniona vuruyor ama
			// PlayerSpell direk yerden cikiyor, haliyle yukaridan bir spell atmissin gibi gozukmuyor.
			//mousePos.z = Camera.main.transform.position.y-1;
			Vector3 instantPos = Camera.main.ScreenToWorldPoint (mousePos);

			Instantiate (playerSpellPrefab, instantPos, Quaternion.identity);
		}
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
