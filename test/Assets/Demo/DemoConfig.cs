using N.Package.Relay;
using UnityEngine;

namespace Demo
{
    public class DemoConfig : MonoBehaviour
    {
        public string remoteUrl = "localhost:9977";
        public RelayClientOptions clientOptions;
        public RelayMasterOptions masterOptions;
    }
}