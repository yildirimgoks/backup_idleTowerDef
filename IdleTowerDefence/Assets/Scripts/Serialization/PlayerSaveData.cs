using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Serialization
{
    [Serializable]
    public class PlayerSaveData
    {
        private BigIntWithUnit _spellDamage;
        private int _spellSpeed;
        private BigIntWithUnit _currency;
        private IEnumerable<MageSaveData> _mageList;

        public PlayerSaveData(Player obj)
        {
            _spellDamage = obj.SpellDamage;
            _spellSpeed = obj.SpellSpeed;
            _currency = obj.GetCurrency();
            _mageList = obj.GetMages().Select(elem => new MageSaveData(elem));
        }
    }
}