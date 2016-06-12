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
				result = Color.red;
				break;
			case Element.Water:
				result = Color.blue;
				break;
			case Element.Earth:
				result = Color.green;
				break;
			case Element.Wood:
				result = Color.yellow;
				break;
			default:
				result = Color.white;
				break;
			}
			return result;
		}
		

	}
}

