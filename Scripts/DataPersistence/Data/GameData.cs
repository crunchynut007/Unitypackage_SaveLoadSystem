namespace DataPersistence.Data
{
    /// <summary>
    /// Persistent data used across all objects in the application. Persists data between sessions.
    /// Expand the implementation structure as required.
    /// </summary>
    public class GameData
    {
        public int counter;
        public float SomeOtherValue;

        // The values defined in this constructor will be the default values
        // the game starts with when there is no data to load. Or rather, if
        // this SO is newly created.
        public GameData() {
            counter = 0;
            SomeOtherValue = 15f;
        }
    }
}
