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
        public Spell[] SpellParticles;
		
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

        public Spell GetParticle(Element element)
        {
            switch (element)
            {
                case Element.Fire:
                    return SpellParticles[0];
                case Element.Water:
                    return SpellParticles[1];
                case Element.Earth:
                    return SpellParticles[2];
                case Element.Air:
                    return SpellParticles[3];
                default:
                    return SpellParticles[0];
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

		public Texture GetMage(Element element) {
			switch (element) {
			case Element.Fire:
				return MageTextures [0];
			case Element.Water:
				return MageTextures [1];
			case Element.Earth:
				return MageTextures[2];
			case Element.Air:
				return MageTextures[3];
			default:
				return MageTextures[3];
			}
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