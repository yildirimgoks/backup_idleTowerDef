using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
	public enum Element{
		Fire = 1,
		Water = 2,
		Earth = 3,
		Wood = 4
	}

	// Element işlemlerini yapmak için kullanırız diye düşündüm :)
	public class ElementController
	{
		private static ElementController instance;

		private ElementController(){}

		public static ElementController Instance
		{
			get {
				if ( instance == null ){
					instance = new ElementController();
				}
				return instance;
			}
		}
		
		public Color GetColor(Element Element){
			Color result;
			switch (Element) {
			case Element.Fire:
				return Color.red;
				break;
			case Element.Water:
				return Color.blue;
				break;
			case Element.Earth:
				return Color.green;
				break;
			case Element.Wood:
				return Color.yellow;
				break;
			default:
				return Color.white;
				break;
			}
		}
		

	}
}

