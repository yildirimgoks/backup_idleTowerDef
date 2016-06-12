using UnityEngine;

namespace Assets.Scripts
{
    public class Tower : MonoBehaviour
    {
        public bool Occupied;
		public bool MenuOpen;
		public Mage InsideMage;

		[System.Serializable]
		public class Action{
			public Sprite sprite;
		}

		public Action[] options;	//For different options on Tower Menu

        // Use this for initialization
        private void Start()
        {
            Occupied = false;
			MenuOpen = false;
        }

        // Update is called once per frame
        private void Update()
        {
        }

		void OnMouseDown(){
			if (!MenuOpen) {
				TowerMenuSpawner.INSTANCE.SpawnMenu (this);
			} else {
				MenuOpen = false;
			}
		}
    }
}