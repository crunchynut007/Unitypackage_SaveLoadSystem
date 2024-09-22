using System.Reflection;
using DataPersistence.Data;
using UnityEngine;

namespace DataPersistence
{
    /// <summary>
    /// A fire-and-forget listener for invoking all <see cref="IDataPersistence.SaveData"/> and <see cref="IDataPersistence.LoadData"/>
    /// methods of all scripts on the gameObject (and its children) it is a component of. Those scripts must inherit <see cref="IDataPersistence"/> interface
    /// for their Save/Load implementations to be triggered.
    /// It is used in conjunction with a <see cref="DataPersistenceManager"/>.
    /// </summary>
    public class DataPersistenceListener : MonoBehaviour, IDataPersistence
    {
        private IDataPersistence[] m_scripts;
        private bool m_deferSubscription = false;
        private const string LoadDataMethod = "LoadData";
        private const string SaveDataMethod = "SaveData";

        private void OnEnable() {
            
            // Subscribe to Methods. Check if DataPersistenceManager exists, else defer subscription to Start()
            if (DataPersistenceManager.Instance) {
                DataPersistenceManager.Instance.OnLoadData += LoadData;
                DataPersistenceManager.Instance.OnSaveData += SaveData;
            }
            else {
                m_deferSubscription = true;
            }

            // Get all instances of IDataPersistence inherited scripts on this gameObject and its children
            m_scripts = gameObject.GetComponentsInChildren<IDataPersistence>();
        }
        
        private void OnDisable() {
            // Unsubscribe events
            DataPersistenceManager.Instance.OnLoadData -= LoadData;
            DataPersistenceManager.Instance.OnSaveData -= SaveData;
        }

        private void Start() {
            // Perform deferred subscription
            if (m_deferSubscription) {
                DataPersistenceManager.Instance.OnLoadData += LoadData;
                DataPersistenceManager.Instance.OnSaveData += SaveData;
                m_deferSubscription = false;
            }
        }


        /// <summary>
        /// Invoke LoadData on all instances of IDataPersistence inherited scripts
        /// </summary>
        /// <param name="data">Persistent data from <see cref="DataPersistenceManager"/></param>
        public void LoadData(GameData data) {
            HandleMethodInvocation(LoadDataMethod, data);
        }

        /// <summary>
        /// Invoke SaveData on all instances of IDataPersistence inherited scripts
        /// </summary>
        /// <param name="data">Persistent data from <see cref="DataPersistenceManager"/></param>
        public void SaveData(ref GameData data) {
            HandleMethodInvocation(SaveDataMethod, data);
        }

        /// <summary>
        /// Helper for invoking a target method (with parameters) on each script
        /// </summary>
        /// <param name="methodName">string name of method</param>
        /// <param name="data">Persistent data from <see cref="DataPersistenceManager"/></param>
        private void HandleMethodInvocation(string methodName, object data) {
            foreach (IDataPersistence script in m_scripts)
            {
                // Invoke target method of each script (looks for private and public).
                MethodInfo method = script.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                // Skip self invocation, send GameData as object parameter for method invocation
                if (method != null && !ReferenceEquals(script, this)) {
                    object[] parameters = { data };
                    method.Invoke(script, parameters);
                }
            }
        }
    }
}