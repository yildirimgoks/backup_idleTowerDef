using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class QkButton : MonoBehaviour
    {
        private AudioManager _audioManager;
        private Button _thisButton;
        
        public Image Icon;

        void Awake()
        {
            _audioManager = Camera.main.GetComponent<AudioManager>();
            _thisButton = gameObject.GetComponent<Button>();
            _thisButton.onClick.AddListener(_audioManager.PlayButtonClickSound);
        }

        void Start()
        {
            if (gameObject.GetComponent<CoolDown>())
            {
                gameObject.GetComponent<CoolDown>().enabled = false;
            }
        }

        public Button GetButton()
        {
            return _thisButton;
        }

        public void Init(float theta, MageAssignableBuilding.Action action)
        {
            var x = Mathf.Sin(theta + Mathf.PI / 2);
            var y = Mathf.Cos(theta + Mathf.PI / 2);
            transform.localPosition = new Vector3(x, y, 0f) * 125f;

            SetIcon(action.sprite);

            foreach (ActionWithEvent actionWithEvent in action.actions)
            {
                if (actionWithEvent == null) continue;
                EventTrigger trigger = GetComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = actionWithEvent.triggerType;
                entry.callback.AddListener(actionWithEvent.function);
                trigger.triggers.Add(entry);
            }
        }

        public void SetIcon(Sprite icon)
        {
            Icon.sprite = icon;
        }
    }
}
