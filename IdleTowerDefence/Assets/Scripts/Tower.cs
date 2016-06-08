using UnityEngine;

namespace Assets.Scripts
{
    public class Tower : MonoBehaviour
    {
        public bool Occupied;
		public bool menuOpen;
		public Mage insideMage;

		[System.Serializable]
		public class Action{
			public Sprite sprite;
		}

		public Action[] options;	//For different options on Tower Menu

        // Use this for initialization
        private void Start()
        {
            Occupied = false;
			menuOpen = false;
        }

        // Update is called once per frame
        private void Update()
        {
        }

		void OnMouseDown(){
			if (!menuOpen) {
				TowerMenuSpawner.ins.SpawnMenu (this);
			} else {
				menuOpen = false;
			}
		}
    }
}