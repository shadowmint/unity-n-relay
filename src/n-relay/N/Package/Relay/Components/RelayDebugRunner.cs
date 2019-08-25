using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace N.Package.Relay.Components
{
    public class RelayDebugRunner : MonoBehaviour
    {
        [Tooltip("The path to the folder containing a relay binary")]
        public string relayBinaryPath = "../../relay";

        [Tooltip("Should we start a relay instance if we can?")]
        public bool relayRunLocal = true;

#if UNITY_EDITOR
#if UNITY_EDITOR_OSX
        private Process _process;
#endif
#endif

        public void Start()
        {
            LaunchRelayServer();
            DontDestroyOnLoad(gameObject);
        }

        private void LaunchRelayServer()
        {
            if (!relayRunLocal) return;
#if UNITY_EDITOR
#if UNITY_EDITOR_OSX
            var assetsFolder = Application.dataPath;

            var pathParts = PathPartsFor(assetsFolder, relayBinaryPath);
            var relayFolder = Path.GetFullPath(Path.Combine(pathParts));
            if (!Directory.Exists(relayFolder))
            {
                Debug.LogWarning($"Missing relay folder: {relayFolder}");
                return;
            }

            Debug.Log($"Found assets path: {relayFolder}");

            var execPath = Path.Combine(relayFolder, "relay");
            if (!File.Exists(execPath))
            {
                Debug.LogWarning($"Missing relay binary: {execPath}");
                return;
            }

            Debug.Log($"Launching relay: {execPath}");

            var startInfo = new ProcessStartInfo {WorkingDirectory = relayFolder, FileName = execPath, CreateNoWindow = false};
            _process = new Process {StartInfo = startInfo};
            if (_process.Start()) return;
            Debug.LogWarning("Failed to start relay");
            Debug.Log(_process.StandardOutput.ReadToEnd());
#endif
#endif
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
#if UNITY_EDITOR_OSX
            if (_process != null)
            {
                Debug.Log("Halting relay process");
                _process.Kill();
            }
#endif
#endif
        }

        private string[] PathPartsFor(string rootPath, string relativePath)
        {
            var parts = new[] {rootPath}.ToList();
            if (relativePath.Contains("/"))
            {
                parts.AddRange(relativePath.Split('/'));
            }
            else if (relativePath.Contains("\\"))
            {
                parts.AddRange(relativePath.Split('\\'));
            }

            return parts.ToArray();
        }
    }
}