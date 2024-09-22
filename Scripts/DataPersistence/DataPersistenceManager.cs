using System.Collections.Generic;
using DataPersistence.Data;
using DataPersistence.Utilities;
using UnityEngine;

namespace DataPersistence
{
    /// <summary>
    /// MonoBehaviour Singleton that manages saving and loading system. Sends save and load events to listeners. Place on
    /// empty gameObject.
    /// Requires: <br></br>
    /// - <see cref="GameData"/> object <br></br>
    /// - <see cref="IDataPersistence"/> interface <br></br>
    /// - <see cref="DataPersistenceListener"/> script <br></br>
    /// - <see cref="FileDataHandler"/> utility
    /// </summary>
    public class DataPersistenceManager : MonoBehaviour
    {
        public static DataPersistenceManager Instance { get; private set; }
        
        [Header("Configuration")] 
        public bool skipLoadFromFileOnStart = false;
        public bool loadFromMemoryNotFile = false;
        public bool skipSaveToFile = false;
        public bool skipSaveOnQuit = false;

        // Delegates with the data you want to pass. Listener script will handle all event subscriptions
        public delegate void LoadDataEventHandler(GameData gameData);
        public delegate void SaveDataEventHandler(ref GameData gameData);
        public event LoadDataEventHandler OnLoadData;
        public event SaveDataEventHandler OnSaveData;
        
        // Persistent data referenced in all save/load system-wide
        public GameData gameData;

        // Implementation for handling data IO to/from a file
        private FileDataHandler m_fileDataHandler;

        private void Awake() {
            // singleton check
            if (Instance != null) Debug.Log("Found more than one Data Persistence Manager in the scene");
            Instance = this;
        }

        private void Start() {
            // assign dataHandler with default OS-agnostic Unity save location. Add a "Save" directory
            m_fileDataHandler = new FileDataHandler();
            
            // Load on Start
            if (!skipLoadFromFileOnStart) LoadGame();
        }

        /// <summary>
        /// Creates a new <see cref="GameObject"/> object with default parameters.
        /// </summary>
        public void NewGame() {
            gameData = new GameData();
        }

        /// <summary>
        /// Invokes OnLoadData event. Used to trigger Loading data
        /// </summary>
        public void LoadGame() {
            // Load any saved data from a file using the data handler
            // if LoadFromLocal is ticked, skip file loading and continue with current data
            if (!loadFromMemoryNotFile) gameData = m_fileDataHandler?.Load();
            
            // if no data can be loaded, initialize to a new game
            if (gameData == null) {
                Debug.Log("No data was found, Initialising data to defaults");
                NewGame();
            }
            
            // Invoke a LoadData event for all listeners
            OnLoadData?.Invoke(gameData);
        }

        /// <summary>
        /// Invokes OnSaveData event. Used to trigger saving data
        /// </summary>
        public void SaveGame() {
            // Invoke a SaveData event for all listeners
            OnSaveData?.Invoke(ref gameData);
            
            // Save GameData to file using the data handler
            if (!skipSaveToFile) m_fileDataHandler?.Save(gameData);
        }

        /// <summary>
        /// Configurable save on quit
        /// </summary>
        private void OnApplicationQuit() {
            if (!skipSaveOnQuit) SaveGame();
        }

        /// <summary>
        /// Set save/load file name
        /// </summary>
        /// <param name="newFileName"></param>
        public void SetFileName(string newFileName) {
            m_fileDataHandler?.SetFileName(newFileName);
        }
        
        /// <summary>
        /// Gets save/load file name
        /// </summary>
        /// <returns></returns>
        public string GetFileName() {
            return m_fileDataHandler?.GetFileName() ?? "";
        }
    }
}