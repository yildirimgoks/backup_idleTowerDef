using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Manager
{
    public class MageFactory
    {
        public static string[] FireNameList = { "Claire", "Ashe", "Nova", "Bayle", "Cendis", "Geysis", "Sizzle", "Scaldris" };
        public static string[] WaterNameList = { "Fross", "Azuris", "Monse", "Aquara", "Puddles", "Vapos", "Vapore", "Manos" };
        public static string[] EarthNameList = { "Therris", "Terros", "Mortus", "Slate", "Vexus", "Pulvi", "Funus", "Sod" };
        public static string[] AirNameList = { "Aeris", "Astra", "Exalos", "Watt", "Gliss", "Atmos", "Spiris", "Huricus" };
        public static string[][] NameList = { FireNameList, WaterNameList, EarthNameList, AirNameList };
        public static string[] LineList = {
            "Do a barrel roll, you fools!",
            "Winter is coming.",
            "Say what?",
            "Hellööööö!",
            "I am your father!",
            "Kanka ben de hiç çalışmadım boşver"
        };
        
        public Mage MagePrefab;

        public MageFactory(Mage magePrefab)
        {
            MagePrefab = magePrefab;
        }

        public static Element GetRandomElement()
        {
            int number = Random.Range(1, 5);
            switch(number)
            {
                case 1:
                    return Element.Fire;
                case 2:
                    return Element.Water;
                case 3:
                    return Element.Earth;
                case 4:
                    return Element.Air;
                default:
                    return GetRandomElement();
            }
        }

        public Mage GetMage(float posX, float posZ)
        {
            var elem = GetRandomElement();
            var mageData = new MageData(GetRandomName(elem), GetRandomLine(), elem);
            
            return Mage.Clone(MagePrefab, mageData, new Vector3(posX, 0, posZ), Quaternion.Euler(0, 90, 0));
        }

        public static string GetRandomName()
        {
            var currentList = NameList[Random.Range(0, NameList.Length)];
            return currentList[Random.Range(0, NameList.Length)];
        }

        public static string GetRandomName(Element element)
        {
            var currentList = NameList[(int) element-1];
            return currentList[Random.Range(0, currentList.Length)];
        }

        public static string GetRandomLine()
        {
            return LineList[Random.Range(0, LineList.Length)];
        }

        public Mage CreateMage(float posX, float posZ, MageData data)
        {
            return Mage.Clone(MagePrefab, data, new Vector3(posX, 8, posZ), Quaternion.Euler(0, 90, 0));
        }
    }
}