using UnityEngine;
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
        public AudioClip[] SpellCastEffects;
        public AudioClip[] SpellCollisionEffects;
        public AudioClip[] SkillCastEffects;
        public AudioClip[] SkillCollisionEffects;

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

        public float GetElementSkillCooldown(Element element)
        {
            switch (element)
            {
                case Element.Fire:
                    return UpgradeManager.FireCooldown;
                case Element.Water:
                    return UpgradeManager.WaterCooldown;
                case Element.Earth:
                    return UpgradeManager.EarthCooldown;
                case Element.Air:
                    return UpgradeManager.AirCooldown;
                default:
                    return 5;
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
                    skillEffects.Add(SkillEffect.ChangeSpeed);
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
					skillEffects.Add(SkillEffect.ChangeDelay);
					break;
				case Element.Earth:
					break;
				case Element.Air:
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
                    return UpgradeManager.MageFireDamageInitial * UpgradeManager.BonusFireMultiplier;
                case Element.Water:
                    return UpgradeManager.MageWaterDamageInitial * UpgradeManager.BonusWaterMultiplier;
                case Element.Earth:
                    return UpgradeManager.MageEarthDamageInitial * UpgradeManager.BonusEarthMultiplier;
                case Element.Air:
                    return UpgradeManager.MageAirDamageInitial * UpgradeManager.BonusAirMultiplier;
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
                    return UpgradeManager.MageFireRangeInitial * UpgradeManager.BonusFireMultiplier;
                case Element.Water:
                    return UpgradeManager.MageWaterRangeInitial * UpgradeManager.BonusWaterMultiplier;
                case Element.Earth:
                    return UpgradeManager.MageEarthRangeInitial * UpgradeManager.BonusEarthMultiplier;
                case Element.Air:
                    return UpgradeManager.MageAirRangeInitial * UpgradeManager.BonusAirMultiplier;
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
                    return UpgradeManager.MageFireRateInitial * UpgradeManager.BonusFireMultiplier;
                case Element.Water:
                    return UpgradeManager.MageWaterRateInitial * UpgradeManager.BonusWaterMultiplier;
                case Element.Earth:
                    return UpgradeManager.MageEarthRateInitial * UpgradeManager.BonusEarthMultiplier;
                case Element.Air:
                    return UpgradeManager.MageAirRateInitial * UpgradeManager.BonusAirMultiplier;
                default:
                    return 1;
            }
        }

	    public float GetSkillPowerInitial(Element element)
	    {
            switch (element)
            {
                case Element.Fire:
                    return UpgradeManager.MageFireSkillPowerInitial * UpgradeManager.BonusFireMultiplier;
                case Element.Water:
                    return UpgradeManager.MageWaterSkillPowerInitial * UpgradeManager.BonusWaterMultiplier;
                case Element.Earth:
                    return UpgradeManager.MageEarthSkillPowerInitial * UpgradeManager.BonusEarthMultiplier;
                case Element.Air:
                    return UpgradeManager.MageAirSkillPowerInitial * UpgradeManager.BonusAirMultiplier;
                default:
                    return 1;
            }
        }

        public float GetSkillPowerMultiplier(Element element)
        {
            switch (element)
            {
                case Element.Fire:
                    return UpgradeManager.MageFireSkillPowerMultiplier;
                case Element.Water:
                    return UpgradeManager.MageWaterSkillPowerMultiplier;
                case Element.Earth:
                    return UpgradeManager.MageEarthSkillPowerMultiplier;
                case Element.Air:
                    return UpgradeManager.MageAirSkillPowerMultiplier;
                default:
                    return 1;
            }
        }

        public BigIntWithUnit GetSkillDamageInitial(Element element)
        {
            switch (element)
            {
                case Element.Fire:
                    return UpgradeManager.MageFireSkillDamageInitial * UpgradeManager.BonusFireMultiplier;
                case Element.Water:
                    return UpgradeManager.MageWaterSkillDamageInitial * UpgradeManager.BonusWaterMultiplier;
                case Element.Earth:
                    return UpgradeManager.MageEarthSkillDamageInitial * UpgradeManager.BonusEarthMultiplier;
                case Element.Air:
                    return UpgradeManager.MageAirSkillDamageInitial * UpgradeManager.BonusAirMultiplier;
                default:
                    return 0;
            }
        }

        public float GetPlayerBonusMultiplier(Element element)
        {
            switch (element)
            {
                case Element.Fire:
                    return UpgradeManager.BonusFireMultiplier;
                case Element.Water:
                    return UpgradeManager.BonusWaterMultiplier;
                case Element.Earth:
                    return UpgradeManager.BonusEarthMultiplier;
                case Element.Air:
                    return UpgradeManager.BonusAirMultiplier;
                default:
                    return 1;
            }
        }

        public float GetSkillDamageMultiplier(Element element)
        {
            switch (element)
            {
                case Element.Fire:
                    return UpgradeManager.MageFireSkillDamageMultiplier;
                case Element.Water:
                    return UpgradeManager.MageWaterSkillDamageMultiplier;
                case Element.Earth:
                    return UpgradeManager.MageEarthSkillDamageMultiplier;
                case Element.Air:
                    return UpgradeManager.MageAirSkillDamageMultiplier;
                default:
                    return 1;
            }
        }
    }
}