using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Player Player;

    public Text CurrText;
    public Text WaveLifeText;
    public Text MageText;
    public Text IncomeText;
	public Text PrevWave;
	public Text CurrWave;
	public Text NextWave;
	public Image NextWaveImage;
	public Sprite EmptyWaveButton;
	public Sprite FullWaveButton;
	public Slider WaveLifeBar;

    public FloatingText popupText;
    public GameObject nonintUI;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        //1M Currency Cheat
        if (Input.GetKeyDown(KeyCode.M))
        {
            Player.Data.IncreaseCurrency(1000000);
        }

        // Kill wave cheat
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (var minion in Player.WaveManager.GetMinionList())
            {
                minion.Data.Kill();
            }
        }

	    UpdateLabels();
    }

    private void UpdateLabels()
    {
        CurrText.text = "Gold: " + Player.Data.GetCurrency();
		WaveLifeText.text = "Wave Life: " + Player.WaveManager.WaveLife;
		if (Player.WaveManager.WaveLife != 0) {
			WaveLifeBar.value = 1 / Player.WaveManager.TotalWaveLife.Divide (Player.WaveManager.WaveLife);
		} else {
			WaveLifeBar.value = 0;
		}
		MageText.text = "Damage: " + Player.Data.CumulativeDps();
		IncomeText.text = "Income: " + Player.Data.CumulativeIdleEarning();

		if (Player.WaveManager.Data.CurrentWave == 0) {
			PrevWave.text = "";
		} else {
			PrevWave.text = Player.WaveManager.Data.CurrentWave.ToString ();
		}
		CurrWave.text = (Player.WaveManager.Data.CurrentWave+1).ToString();
		if (Player.WaveManager.Data.CurrentWave == Player.WaveManager.Data.GetMaxReachedWave ()) {
			NextWave.text = "";
			NextWaveImage.sprite = EmptyWaveButton;
		} else {
			NextWave.text = (Player.WaveManager.Data.CurrentWave + 2).ToString ();
			NextWaveImage.sprite = FullWaveButton;
		}

    }

    //Can be used for any menu
    public void OpenCloseMenu(GameObject menu, bool open)
    {
        var anim = menu.GetComponent<Animator>();
        anim.SetBool("isDisplayed", !anim.GetBool("isDisplayed") && open);
    }

    //For handling popup damage text
    public void CreateFloatingText(string text, Transform location)
    {
        FloatingText instance = Instantiate(popupText);
        Vector3 pos = location.position + new Vector3(0f, 12f, 0f);
        instance.transform.SetParent(nonintUI.transform, false);
        instance.transform.position = pos;
        instance.SetText(text);
    }
}
