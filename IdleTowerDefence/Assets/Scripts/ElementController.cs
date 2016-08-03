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

		public Texture[] TowerTextures;
		public Texture[] MageTextures;
		
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
                    return TowerTextures [1];
                case Element.Water:
				return TowerTextures [2];
                case Element.Earth:
				return TowerTextures[3];
                case Element.Air:
				return TowerTextures[4];
                default:
				return TowerTextures[0];
			}
		}

		public Texture[] GetMage(Element element) {
			var tex = new Texture[2];
			switch (element) {
			case Element.Fire:
				tex [0] = MageTextures [0];
				tex [1] = MageTextures [1];
				break;
			case Element.Water:
				tex [0] = MageTextures [2];
				tex [1] = MageTextures [3];
				break;
			case Element.Earth:
				tex [0] = MageTextures [4];
				tex [1] = MageTextures [5];
				break;
			case Element.Air:
				tex [0] = MageTextures [6];
				tex [1] = MageTextures [7];
				break;
			default:
				tex [0] = MageTextures [6];
				tex [1] = MageTextures [7];
				break;
			}
			return tex;
		}
        
		public Effect GetEffect(Element element) {
			switch (element) {
				case Element.Fire:
					return Effect.Burn;
				case Element.Water:
                    return Effect.Wet;
				case Element.Earth:
                    return Effect.Daze;
				case Element.Air:
                    return Effect.High;
				default:
                    return Effect.None;
			}
		}

		public double GetDamageMultiplier(Element element) {
			switch (element) {
				case Element.Fire:
                    return 1.1;
				case Element.Water:
                    return 1;
				case Element.Earth:
                    return 1.1;
				case Element.Air:
                    return 0.9;
				default:
                    return 1;
			}
		}

		// Returns Range Multiplier of the element mage
		public double GetRangeMultiplier(Element element) {
			switch (element) {
				case Element.Fire:
                    return 1.1;
				case Element.Water:
                    return 1;
				case Element.Earth:
                    return 1;
				case Element.Air:
                    return 1.1;
				default:
                    return 1;
			}
		}

		// Returns Speed Multiplier of the element projectile
		public double GetSpeedMultiplier(Element element) {
			switch (element) {
				case Element.Fire:
                    return 0.9;
				case Element.Water:
                    return 1;
				case Element.Earth:
                    return 0.9;
				case Element.Air:
                    return 1;
				default:
                    return 1;
			}
		}
		
		// Returns Cooldown Multiplier for casting the element
		public double GetDelayMultiplier(Element element) {
			switch (element) {
				case Element.Fire:
                    return 0.9;
				case Element.Water:
                    return 1;
				case Element.Earth:
                    return 1;
				case Element.Air:
                    return 1;
				default:
                    return 1;
			}
		}

		public Element CombineElements(Element element1, Element element2) {
			var combinedElement = Element.Fire;
			return combinedElement;
		}
	}
}