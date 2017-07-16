using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class DailyBonusWindow : MonoBehaviour
    {
        public UIManager UIManager;

        public GameObject BonusMenu;
        public GameObject Closer;
        public GameObject BonusPrefab;
        public ScrollRect BonusScroll;
        public RectTransform Viewport;

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                SetScrollToDay(2);
            }
        }

        public void SetScrollToDay(int dayNumber)
        {
            var bonusWidth = BonusPrefab.GetComponent<LayoutElement>().preferredWidth;
            var spacing = GetComponent<HorizontalLayoutGroup>().spacing;
            var totalWidth = 7 * bonusWidth + 8 * spacing;
            var diff = totalWidth - Viewport.rect.width;
            var fromLeft = dayNumber * spacing + (dayNumber - 1) * bonusWidth;
            BonusScroll.horizontalNormalizedPosition = fromLeft / diff;
            //Debug.Log (fromLeft/diff);
            //Debug.Log (BonusScroll.horizontalNormalizedPosition );
        }

        public void UnlockDay(int daynumber)
        {
            var dayWindow = transform.Find(daynumber.ToString());
            dayWindow.Find("Lock Image").gameObject.SetActive(false);
        }

        public void UnlockUntilDay(int daynumber)
        {
            for (var day = 1; day <= daynumber; day++)
            {
                UnlockDay(day);
            }
        }

        public void SetDayBonus(int daynumber, UnityAction bonusaction, string bonusdescription)
        {
            var dayWindow = transform.Find(daynumber.ToString());
            dayWindow.GetComponent<Button>().interactable = true;
            dayWindow.GetComponent<Button>().onClick.AddListener(bonusaction);
            dayWindow.GetComponent<Button>().onClick.AddListener(delegate
            {
                UIManager.OpenCloseMenu(BonusMenu, false);
                dayWindow.GetComponent<Button>().interactable = false;
            });
            dayWindow.Find("Prize Name").GetComponent<Text>().text = bonusdescription;
            //rewardTexts [daynumber - 1] = bonusdescription;
        }

        public void SetDaysUntilDay(int daynumber, UnityAction bonusaction, string bonusdescription)
        {
            for (var day = 1; day < daynumber; day++)
            {
                var dayWindow = transform.Find(day.ToString());
                dayWindow.Find("Prize Name").GetComponent<Text>().text = "You already claimed this reward.";//rewardTexts[day-1];
                dayWindow.GetComponent<Button>().interactable = false;
            }
            SetDayBonus(daynumber, bonusaction, bonusdescription);
        }

        public void LockAllDays()
        {
            for (var day = 1; day <= 7; day++)
            {
                var dayWindow = transform.Find(day.ToString());
                dayWindow.Find("Lock Image").gameObject.SetActive(true);
                dayWindow.GetComponent<Button>().interactable = false;
                dayWindow.GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }

        public void OpenBonusMenu()
        {
            UIManager.OpenCloseMenu(BonusMenu, true);
        }
    }
}