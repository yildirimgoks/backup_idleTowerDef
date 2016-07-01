using UnityEngine;
using Assets.Scripts.Model;

namespace Assets.Scripts
{
    public class MageFactory
    {
        public static string[] NameList = { "Gandalf the Magenta", "Dumblebee", "Hayri", "Merlin", "İzzet", "Longbottom" };
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
            var mageData = new MageData(GetRandomName(), GetRandomLine(), GetRandomElement());
            
            return Mage.Clone(MagePrefab, mageData, new Vector3(posX, 12.2f, posZ), Quaternion.Euler(0, 90, 0));
        }

        public static string GetRandomName()
        {
            return NameList[Random.Range(0, NameList.Length)];
        }

        public static string GetRandomLine()
        {
            return LineList[Random.Range(0, LineList.Length)];
        }
    }
}