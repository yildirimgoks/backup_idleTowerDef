using UnityEngine;

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
			get { return instance ?? (instance = new ElementController()); }
		}

		public Texture[] textures;
		
		public Color GetColor(Element element) {
			switch (element) {
				case Element.Fire:
					return Color.red;
				case Element.Water:
                    return Color.blue;
				case Element.Earth:
                    return Color.green;
				case Element.Air:
                    return Color.gray;
				default:
                    return Color.black;
            }
		}

		public Texture GetTower(Element element) {
			switch (element) {
                case Element.Fire:
                    return textures [1];
                case Element.Water:
                    return textures [2];
                case Element.Earth:
                    return textures[3];
                case Element.Air:
                    return textures[4];
                default:
                    return textures[0];
			}
		}

        //ToDo: Make rest of the functions also shorter as GetTower/GetColor
		public Effect GetEffect(Element element) {
			Effect effect;
			switch (element) {
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

		public double GetDamageMultiplier(Element element) {
			double multiplier = 1;
			switch (element) {
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
		public double GetRangeMultiplier(Element element) {
			double multiplier = 1;
			switch (element) {
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
		public double GetSpeedMultiplier(Element element) {
			double multiplier = 1;
			switch (element) {
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
		public double GetDelayMultiplier(Element element) {
			double multiplier = 1;
			switch (element) {
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

		public Element CombineElements(Element element1, Element element2) {
			var combinedElement = Element.Fire;
			return combinedElement;
		}

	}
}

