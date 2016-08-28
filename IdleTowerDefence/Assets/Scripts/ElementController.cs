using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Model;

namespace Assets.Scripts
{
	public enum Element{
		Fire = 1,
		Water = 2,
		Earth = 3,
		Air = 4
	}

	public enum SpellEffect{
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
		public Texture[] ShrineTextures;
		public Texture[] MageTextures;
        public Spell[] SpellParticles;
        public SkillProjectile[] SkillParticles;
		
		public Color GetColor(Element element) {
			switch (element) {
				case Element.Fire:
					return Color.red;
				case Element.Water:
					return new Color(0.239f,0.722f,0.635f,1);
				case Element.Earth:
				return new Color(0.439f,0.235f,0.039f,1);
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

        public SkillProjectile GetSkillProjectile(Element element)
        {
            switch (element)
            {
                case Element.Fire:
                    return SkillParticles[0];
                case Element.Water:
                    return SkillParticles[1];
                case Element.Earth:
                    return SkillParticles[2];
                case Element.Air:
                    return SkillParticles[3];
                default:
                    return SkillParticles[0];
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

		public Texture GetShrine(Element element) {
			switch (element) {
			case Element.Fire:
				return ShrineTextures [1];
			case Element.Water:
				return ShrineTextures [2];
			case Element.Earth:
				return ShrineTextures[3];
			case Element.Air:
				return ShrineTextures[4];
			default:
				return ShrineTextures[0];
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
        
		public SpellEffect GetSpellEffect(Element element) {
			switch (element) {
				case Element.Fire:
					return SpellEffect.Burn;
				case Element.Water:
                    return SpellEffect.Wet;
				case Element.Earth:
                    return SpellEffect.Daze;
				case Element.Air:
                    return SpellEffect.High;
				default:
                    return SpellEffect.None;
			}
		}

		public SkillType GetSkillType(Element element){
			switch (element) {
				case Element.Fire:
					return SkillType.AreaTop;
				case Element.Water:
                    return SkillType.AllTowers;
				case Element.Earth:
                    return SkillType.PathFollower;
				case Element.Air:
                    return SkillType.PathFollower;
				default:
                    return SkillType.AreaTop;
			}
		}

		public List<SkillEffect> GetSkillEffectsToMinions(Element element){
			var skillEffects = new List<SkillEffect>();
			switch (element) {
				case Element.Fire:
					skillEffects.Add(SkillEffect.Damage);
					break;
				case Element.Water:
					break;
				case Element.Earth:
                    skillEffects.Add(SkillEffect.Damage);
					break;
				case Element.Air:
                    skillEffects.Add(SkillEffect.DecreaseSpeed);
					break;
				default:
					skillEffects.Add(SkillEffect.Damage);
                    break;
			}
			return skillEffects;
		}

		public List<SkillEffect> GetSkillEffectsToTowers(Element element){
			var skillEffects = new List<SkillEffect>();
			switch (element) {
				case Element.Fire:
					break;
				case Element.Water:
					skillEffects.Add(SkillEffect.DecreaseDelay);
					break;
				case Element.Earth:
					break;
				case Element.Air:
					break;
				default:
					break;
                    
			}
			return skillEffects;
		}

		public float GetDamageMultiplier(Element element) {
			switch (element) {
				case Element.Fire:
                    return 1.17f;
				case Element.Water:
                    return 1.01f;
				case Element.Earth:
                    return 1.1f;
				case Element.Air:
                    return 1.03f;
				default:
                    return 1;
			}
		}

		// Returns Range Multiplier of the element mage
		public float GetRangeMultiplier(Element element) {
			switch (element) {
				case Element.Fire:
                    return 1.1f;
				case Element.Water:
                    return 1.2f;
				case Element.Earth:
                    return 1.01f;
				case Element.Air:
                    return 1.15f;
				default:
                    return 1;
			}
		}
		
		// Returns Cooldown Multiplier for casting the element
		public float GetDelayMultiplier(Element element) {
			switch (element) {
				case Element.Fire:
                    return 0.97f;
				case Element.Water:
                    return 1.03f;
				case Element.Earth:
                    return 1.123f;
				case Element.Air:
                    return 1.053f;
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