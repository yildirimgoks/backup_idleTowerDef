using UnityEngine;
using System.Collections;

public class Minion : MonoBehaviour {

    public TextMesh healthIndicator;

    private int CurrencyGivenOnDeath = 5;
    public int Life = 100;

	private float speed = 0.01f;

	private GameController controller = Camera.main.gameObject.GetComponent<GameController> ();

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
		 Walk ();
		if (transform.position.z > GameController.MAX_Z) {
			controller.MinionSurvived (this);
		}
	}

	// To UYGAR from HAYDAR: Denerken kullandim bu şekilde, istediğin şekilde değiştirirsin
	void Walk (){
		Vector3 desiredPosition = new Vector3 (this.transform.position.x, this.transform.position.y, this.transform.position.z + 100);
		transform.Translate (desiredPosition * speed * Time.deltaTime);
	}

    void OnDestroy() {
		if (controller == null)
			return;
		controller.MinionDied (this, CurrencyGivenOnDeath);
    }
}
