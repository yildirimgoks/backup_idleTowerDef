using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
	public enum Element{
		Fire = 1,
		Water = 2,
		Earth = 3,
		Air = 4
	}

	public enum Effect{
		None = 0,
		Burn = 1,
		Wet = 2,
		Daze = 3,
		High = 4
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
				case Element.Air:
					result = Color.gray;
				break;
				default:
					result = Color.black;
				break;
			}
			return result;
		}

		public Effect GetEffect(Element Element){
			Effect effect;
			switch (Element) {
				case Element.Fire:
					effect = Effect.Burn;
				break;
				case Element.Water:
					effect = Effect.Wet;
				break;
				case Element.Earth:
					effect = Effect.Daze;
				break;
				case Element.Air:
					effect = Effect.High;
				break;
				default:
					effect = Effect.None;
				break;
			}
			return effect;
		}

		public double GetDamageMultiplier(Element Element){
			double multiplier = 1;
			switch (Element) {
				case Element.Fire:
					multiplier = 1.1;
				break;
				case Element.Water:
					multiplier = 1;
				break;
				case Element.Earth:
					multiplier = 1.1;
				break;
				case Element.Air:
					multiplier = 0.9;
				break;
				default:
					multiplier = 1;
				break;
			}
			return multiplier;
		}

		// Returns Range Multiplier of the element mage
		public double GetRangeMultiplier(Element Element){
			double multiplier = 1;
			switch (Element) {
				case Element.Fire:
					multiplier = 1.1;
				break;
				case Element.Water:
					multiplier = 1;
				break;
				case Element.Earth:
					multiplier = 1;
				break;
				case Element.Air:
					multiplier = 1.1;
				break;
				default:
					multiplier = 1;
				break;
			}
			return multiplier;
		}

		// Returns Speed Multiplier of the element projectile
		public double GetSpeedMultiplier(Element Element){
			double multiplier = 1;
			switch (Element) {
				case Element.Fire:
					multiplier = 0.9;
				break;
				case Element.Water:
					multiplier = 1;
				break;
				case Element.Earth:
					multiplier = 0.9;
				break;
				case Element.Air:
					multiplier = 1;
				break;
				default:
					multiplier = 1;
				break;
			}
			return multiplier;
		}
		
		// Returns Cooldown Multiplier for casting the element
		public double GetDelayMultiplier(Element Element){
			double multiplier = 1;
			switch (Element) {
				case Element.Fire:
					multiplier = 0.9;
				break;
				case Element.Water:
					multiplier = 1;
				break;
				case Element.Earth:
					multiplier = 1;
				break;
				case Element.Air:
					multiplier = 1;
				break;
				default:
					multiplier = 1;
				break;
			}
			return multiplier;
		}

		public Element CombineElements(Element Element1, Element Element2){
			Element combinedElement;
			return combinedElement;
		}

	}
}

