using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Player Player;

    public Text CurrText;
    public Text WaveText;
    public Text WaveLifeText;
    public Text MageText;
    public Text IncomeText;
    public Text PlayerUpgrade;
    public Button Wave1;
    public Button Wave2;
    public Button Wave3;
    public Button Wave4;
    public Button Wave5;
    public Slider WaveLifeBar;

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
        WaveLifeBar.value = 1 / Player.WaveManager.TotalWaveLife.Divide(Player.WaveManager.WaveLife);
        MageText.text = "Damage: " + Player.Data.CumulativeDps();
        IncomeText.text = "Income: ";
        var currentWaveBlock = Player.WaveManager.Data.CurrentWave / 5 * 5;
        Wave1.GetComponentInChildren<Text>().text = "" + (currentWaveBlock + 1);
        Wave2.GetComponentInChildren<Text>().text = "" + (currentWaveBlock + 2);
        Wave3.GetComponentInChildren<Text>().text = "" + (currentWaveBlock + 3);
        Wave4.GetComponentInChildren<Text>().text = "" + (currentWaveBlock + 4);
        Wave5.GetComponentInChildren<Text>().text = "" + (currentWaveBlock + 5);
        ColorBlock cb;
        switch (Player.WaveManager.Data.CurrentWave % 5)
        {
            case 1: cb = Wave2.colors; cb.disabledColor = Color.yellow; Wave2.colors = cb; break;
            case 2: cb = Wave3.colors; cb.disabledColor = Color.yellow; Wave3.colors = cb; break;
            case 3: cb = Wave4.colors; cb.disabledColor = Color.yellow; Wave4.colors = cb; break;
            case 4: cb = Wave5.colors; cb.disabledColor = Color.yellow; Wave5.colors = cb; break;
        }
        if (Player.WaveManager.Data.CurrentWave % 5 == 0)
        {
            cb = Wave1.colors;
            cb.disabledColor = Color.yellow;
            Wave1.colors = cb;
            cb = Wave2.colors;
            cb.disabledColor = Color.white;
            Wave2.colors = cb;
            cb = Wave3.colors;
            cb.disabledColor = Color.white;
            Wave3.colors = cb;
            cb = Wave4.colors;
            cb.disabledColor = Color.white;
            Wave4.colors = cb;
            cb = Wave5.colors;
            cb.disabledColor = Color.white;
            Wave5.colors = cb;
        }
    }

    //Can be used for any menu
    public void OpenCloseMenu(GameObject menu, bool open)
    {
        var anim = menu.GetComponent<Animator>();
        anim.SetBool("isDisplayed", !anim.GetBool("isDisplayed") && open);
    }
}
