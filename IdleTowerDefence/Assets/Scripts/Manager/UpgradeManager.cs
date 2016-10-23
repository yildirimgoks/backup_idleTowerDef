namespace Assets.Scripts.Manager
{
    public class UpgradeManager
    {
        #region MinionConstants

        public static BigIntWithUnit MinionDeathRewardInitial = 10;
        public static float MinionDeathRewardMultiplier = 1.10f;
        
        public static BigIntWithUnit MinionLifeInitial = 100;
        public static float MinionLifeMultiplier = 1.12f;

        #endregion

        #region MageConstants
        public static float MageIdleGenerationMultiplier = 1.07f;
        public static BigIntWithUnit MageIdleGenerationInitial = 2;

        public static BigIntWithUnit MageIdleGenerationUpgradePriceInitial = 30;
        public static float MageIdleGenerationUpgradePriceMultiplier = 1.14f;

        public static BigIntWithUnit MageUpgradePriceInitial = 30;
        public static float MageUpgradePriceMultiplier = 1.14f;

        #region ElementConstants
        public static BigIntWithUnit PlayerDamageInitial = 10;
        public static float PlayerDamageMultiplier = 1.1f;

        public static BigIntWithUnit MageFireDamageInitial = 120;
        public static float MageFireDamageMultiplier = 1.11f;
        public static float MageFireRangeInitial = 12.0f;
        public static float MageFireRangeMultiplier = 1.05f;
        public static float MageFireRateInitial = 1.2f;
        public static float MageFireRateMultiplier = 1.04f;

        public static float MageFireSkillPowerInitial = 1.0f;
        public static float MageFireSkillPowerMultiplier = 1.0f;
        public static BigIntWithUnit MageFireSkillDamageInitial = 50;
        public static float MageFireSkillDamageMultiplier = 1.15f;

        public static float FireCooldown = 5.0f;
       
        public static BigIntWithUnit MageWaterDamageInitial = 60;
        public static float MageWaterDamageMultiplier = 1.04f;
        public static float MageWaterRangeInitial = 19.0f;
        public static float MageWaterRangeMultiplier = 1.11f;
        public static float MageWaterRateInitial = 1.7f;
        public static float MageWaterRateMultiplier = 1.08f;

        public static float MageWaterSkillPowerInitial = 1.1f;
        public static float MageWaterSkillPowerMultiplier = 1.05f;
        public static BigIntWithUnit MageWaterSkillDamageInitial = 0;
        public static float MageWaterSkillDamageMultiplier = 1.0f;

        public static float WaterCooldown = 5.0f;
       
        public static BigIntWithUnit MageEarthDamageInitial = 90;
        public static float MageEarthDamageMultiplier = 1.10f;
        public static float MageEarthRangeInitial = 14.0f;
        public static float MageEarthRangeMultiplier = 1.06f;
        public static float MageEarthRateInitial = 1.5f;
        public static float MageEarthRateMultiplier = 1.07f;

        public static float MageEarthSkillPowerInitial = 1.0f;
        public static float MageEarthSkillPowerMultiplier = 1.0f;
        public static BigIntWithUnit MageEarthSkillDamageInitial = 15;
        public static float MageEarthSkillDamageMultiplier = 1.15f;

        public static float EarthCooldown = 5.0f;

        public static BigIntWithUnit MageAirDamageInitial = 50;
        public static float MageAirDamageMultiplier = 1.05f;
        public static float MageAirRangeInitial = 24.0f;
        public static float MageAirRangeMultiplier = 1.15f;
        public static float MageAirRateInitial = 2.0f;
        public static float MageAirRateMultiplier = 1.09f;

        public static float MageAirSkillPowerInitial = 0.9f;
        public static float MageAirSkillPowerMultiplier = 0.95f;
        public static BigIntWithUnit MageAirSkillDamageInitial = 0;
        public static float MageAirSkillDamageMultiplier = 1.0f;

        public static float AirCooldown = 5.0f;

        #endregion

        #endregion

    }
}
