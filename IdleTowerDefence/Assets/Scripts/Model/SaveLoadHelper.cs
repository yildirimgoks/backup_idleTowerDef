using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class SaveLoadHelper {

        private static string SavegameFileName = "saveGame.xml";

        public static string GetSaveGameFilePath()
        {
            var path = Application.persistentDataPath + "/" + SavegameFileName;
            return path;
        }

        public static void SaveGame(PlayerData data)
        {
            // Creates an instance of the XmlSerializer class;
            // specifies the type of object to serialize.
            var serializer = new DataContractSerializer(typeof(PlayerData));
            var writer = new FileStream(GetSaveGameFilePath(), FileMode.Create);
        
            // Serializes the purchase order, and closes the TextWriter.
            serializer.WriteObject(writer, data);
            writer.Close();
        }

        public static PlayerData LoadGame()
        {
            try
            {
                var fileStream = new FileStream(GetSaveGameFilePath(), FileMode.Open);
                var reader = XmlDictionaryReader.CreateTextReader(fileStream, new XmlDictionaryReaderQuotas());
                var serializer = new DataContractSerializer(typeof(PlayerData));

                // Deserialize the data and read it from the instance.
                var playerData = (PlayerData)serializer.ReadObject(reader, true);
                reader.Close();
                fileStream.Close();

                return playerData;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
