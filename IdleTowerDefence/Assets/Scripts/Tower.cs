using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Tower : MonoBehaviour
    {
        public bool Occupied;

		public Button TowerButton;

        // Use this for initialization
        private void Start()
        {
            Occupied = false;
			TowerButton.transform.position = new Vector3 (-13,7,20);	//yanlış!
		}

        // Update is called once per frame
        private void Update()
        {
			if (!Occupied) {
				TowerButton.interactable = false;
				TowerButton.image.transform.localScale = new Vector3 (0, 0, 0);
			} else {
				TowerButton.interactable = true;
				TowerButton.image.transform.localScale = new Vector3 (1, 1, 1);
				if (Input.mousePosition == Camera.main.WorldToScreenPoint (TowerButton.transform.position)) {
					TowerButton.image.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);	//olmadı???
				}
			}
        }
    }
}