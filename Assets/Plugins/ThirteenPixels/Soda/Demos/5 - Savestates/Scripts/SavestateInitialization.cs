
namespace ThirteenPixels.SodaDemos
{
    using UnityEngine;
    using Soda;

    /// <summary>
    /// An example class showcasing an automatic Savestate loading and initialization of a Savestate reader/writer.
    /// This class uses the Json reader/writer class to save the Savestate data into Unity's persistent data path.
    /// 
    /// For actual projects, it is recommended to use [RuntimeInitializeOnLoadMethod] as shown below instead of calling the method in Start().
    /// This makes the initialization run without having to add a script like this to specific scenes.
    /// The attribute is commented out in this demo so it doesn't show up in other demos or other parts of your project.
    /// </summary>
    public class SavestateInitialization : MonoBehaviour
    {
        private const string filename = "Soda Demo Savestate.json";
        private static string savestateFilePath;

        [SerializeField]
        private Savestate saveStateToLoadOnStart = default;

        // [RuntimeInitializeOnLoadMethod]
        private static void InitializeSavestateReaderWriter()
        {
            savestateFilePath = Application.persistentDataPath + "/" + filename;
            Debug.Log("Demo is saving into " + savestateFilePath);

            var readerWriter = new SavestateReaderWriterJson(savestateFilePath);
            SavestateSettings.defaultReader = readerWriter;
            SavestateSettings.defaultWriter = readerWriter;
        }

        private void Start()
        {
            // Comment this out to use the [RuntimeInitializeOnLoadMethod] attribute instead.
            InitializeSavestateReaderWriter();

            if (saveStateToLoadOnStart)
            {
                if (System.IO.File.Exists(savestateFilePath))
                {
                    saveStateToLoadOnStart.Load();
                }
                else
                {
                    Debug.LogWarning("No Savestate file found to load.");
                }
            }
        }
    }
}
