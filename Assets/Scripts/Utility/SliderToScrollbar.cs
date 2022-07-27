using UnityEngine.UI;
using UnityEngine;

public class SliderToScrollbar : MonoBehaviour {
	public Slider slider;
	public Scrollbar target;

	public void SetValue() {
		target.value = slider.value;
	}
}
