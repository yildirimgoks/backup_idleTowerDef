using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Slider))]
public class ToggleSlider : MonoBehaviour {
	public ToggleSliderEvent onTurnOn;
	public ToggleSliderEvent onTurnOff;
	private Slider slider;

	private void Start()
    {
        AssignSlider();
	}

    public void AssignSlider()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnValueChanged);
    }

	private void OnValueChanged(float value){
		if (value < 0.5f) {
		//Off
			onTurnOff.Invoke();
		} else {
		//On
			onTurnOn.Invoke();
		}
	}

	public void ChangeValue() {
		slider.value = 1 - slider.value;

	}

	[System.Serializable]
	public class ToggleSliderEvent:UnityEvent{}
}
