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
        //ToDo: Add elemental constants
        public static BigIntWithUnit MageDamageInitial = 50;
        public static float MageDamageMultiplier = 1.10f;
        public static float MageFireRateInitial = 1.0f;
        public static float MageFireRateMultiplier = 1.05f;
        #endregion

        #endregion

    }
}
