using Fusion;
using UnityEngine;

namespace Resources
{
    public class RPCTest : MonoBehaviour
    {
        [Rpc(HostMode = RpcHostMode.SourceIsHostPlayer, InvokeLocal = false)]
        public void RPC_C2S_Test()
        {
            Debug.Log("RPC_C2S_Test");
        }
    }
}
