using System;
using System.IO;
using System.Linq;
using DataPersistence.Data;
using UnityEngine;

namespace DataPersistence.Utilities
{
    /// <summary>
    /// Implementation for saving and loading from a File. Read/Writes JSON data.
    /// </summary>
    public class FileDataHandler
    {
        private readonly string m_dataDirPath = string.Concat(Application.persistentDataPath, "/Saves/");
        private string m_dataFileName = "defaultSave.game";

        /// <summary>
        /// Loads data from a JSON file to a data object, serializes it, then returns as an object
        /// </summary>
        /// <returns><see cref="GameData"/> object or null</returns>
        public GameData Load() {
            string fullPath = Path.Combine(m_dataDirPath, GetFileName());
            GameData loadedData = null;
            if (File.Exists(fullPath)) {
                try {
                    // Load the serialized data from the file
                    // using() block ensure the file is closed after read/write operation
                    string dataToLoad = "";
                    using (FileStream stream = new FileStream(fullPath, FileMode.Open)) { // FileMode.Open to open as readonly
                        using (StreamReader reader = new StreamReader(stream)) {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }
                    
                    // Deserialize json back into GameData
                    loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                }
                catch (Exception e) {
                    Debug.Log($"Error when trying to load data from file: {fullPath}\n{e}");
                    throw;
                }
            }
            return loadedData;
        }

        /// <summary>
        /// Save data to a file as JSON. A save directory is created if it does not exist.
        /// </summary>
        /// <param name="data"><see cref="GameData"/> object to be serialized to JSON then saved to the file</param>
        /// <exception cref="InvalidOperationException">Unable to read/write directory path</exception>
        public void Save(GameData data) {
            string fullPath = Path.Combine(m_dataDirPath, GetFileName());
            try {
                // create directory if not exists
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? throw new InvalidOperationException());
                
                // serialize the GameData object into Json
                string dataToStore = JsonUtility.ToJson(data, true);
                
                // write the serialized json data to a file
                // using() block ensure the file is closed after read/write operation
                using (FileStream stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write)) { // FileMode.Create to overwrite a file
                    using (StreamWriter writer = new StreamWriter(stream)) {
                        writer.Write(dataToStore);
                    } 
                }
            }
            catch (Exception e) {
                Debug.Log($"Error when trying to save data to file: {fullPath}\n{e}");
                throw;
            }
        }
        
        /// <summary>
        /// Utility to change the file name for saving and loading
        /// </summary>
        /// <param name="newFileName">string</param>
        public void SetFileName(string newFileName) {
            if (!newFileName.Contains(".")) newFileName = string.Concat(newFileName, ".game");
            m_dataFileName = newFileName;
        }
        
        /// <summary>
        /// Utility to get the file name for saving and loading
        /// </summary>
        /// <returns>string</returns>
        public string GetFileName() {
            if (!m_dataFileName.Contains(".")) return string.Concat(m_dataFileName, ".game");
            return m_dataFileName != "" ? m_dataFileName : "";
        }
    }
}