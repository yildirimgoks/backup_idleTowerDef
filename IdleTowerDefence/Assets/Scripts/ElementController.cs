﻿using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Manager;
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
		public Sprite[] ElementIcons;
        public AudioClip[] ElementSoundEffects;
        public AudioClip[] SpellCollisionEffects;
		
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
			Debug.Log(element);
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

        public AudioClip GetSoundEffects(Element element) {
                switch (element) {
                case Element.Fire:
                    return ElementSoundEffects[1];
                case Element.Water:
                    return ElementSoundEffects[2];
                case Element.Earth:
                    return ElementSoundEffects[3];
                case Element.Air:
                    return ElementSoundEffects[4];
                default:
                    return null;
            }
        }

        public AudioClip GetSpellCollisionEffects(Element element) {

            switch (element) {
                case Element.Fire:
                    return SpellCollisionEffects[1];
                case Element.Water:
                    return SpellCollisionEffects[2];
                case Element.Earth:
                    return SpellCollisionEffects[3];
                case Element.Air:
                    return SpellCollisionEffects[4];
                default:
                    return null;
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

		public Sprite GetIcon(Element element) {
			switch (element) {
			case Element.Fire:
				return ElementIcons [0];
			case Element.Water:
				return ElementIcons [1];
			case Element.Earth:
				return ElementIcons[2];
			case Element.Air:
				return ElementIcons[3];
			default:
				return ElementIcons[0];
			}
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
                    return UpgradeManager.MageFireDamageMultiplier;
				case Element.Water:
                    return UpgradeManager.MageWaterDamageMultiplier;
				case Element.Earth:
                    return UpgradeManager.MageEarthDamageMultiplier;
				case Element.Air:
                    return UpgradeManager.MageAirDamageMultiplier;
				default:
                    return 1;
			}
		}

        public BigIntWithUnit GetDamageInitial(Element element)
        {
            switch (element)
            {
                case Element.Fire:
                    return UpgradeManager.MageFireDamageInitial;
                case Element.Water:
                    return UpgradeManager.MageWaterDamageInitial;
                case Element.Earth:
                    return UpgradeManager.MageEarthDamageInitial;
                case Element.Air:
                    return UpgradeManager.MageAirDamageInitial;
                default:
                    return 1;
            }
        }

        // Returns Range Multiplier of the element mage
        public float GetRangeMultiplier(Element element) {
			switch (element) {
				case Element.Fire:
                    return UpgradeManager.MageFireRangeMultiplier;
				case Element.Water:
                    return UpgradeManager.MageWaterRangeMultiplier;
				case Element.Earth:
                    return UpgradeManager.MageEarthRangeMultiplier;
				case Element.Air:
                    return UpgradeManager.MageAirRangeMultiplier;
				default:
                    return 1;
			}
		}

        public float GetRangeInitial(Element element)
        {
            switch (element)
            {
                case Element.Fire:
                    return UpgradeManager.MageFireRangeInitial;
                case Element.Water:
                    return UpgradeManager.MageWaterRangeInitial;
                case Element.Earth:
                    return UpgradeManager.MageEarthRangeInitial;
                case Element.Air:
                    return UpgradeManager.MageAirRangeInitial;
                default:
                    return 1;
            }
        }

        // Returns Cooldown Multiplier for casting the element
        public float GetDelayMultiplier(Element element) {
			switch (element) {
				case Element.Fire:
                    return UpgradeManager.MageFireRateMultiplier;
				case Element.Water:
                    return UpgradeManager.MageWaterRateMultiplier;
				case Element.Earth:
                    return UpgradeManager.MageEarthRateMultiplier;
				case Element.Air:
                    return UpgradeManager.MageAirRateMultiplier;
				default:
                    return 1;
			}
		}

        public float GetDelayInitial(Element element)
        {
            switch (element)
            {
                case Element.Fire:
                    return UpgradeManager.MageFireRateInitial;
                case Element.Water:
                    return UpgradeManager.MageWaterRateInitial;
                case Element.Earth:
                    return UpgradeManager.MageEarthRateInitial;
                case Element.Air:
                    return UpgradeManager.MageAirRateInitial;
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