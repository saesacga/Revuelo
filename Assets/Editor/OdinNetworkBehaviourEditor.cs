#if UNITY_EDITOR
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Unity.Netcode;

[CustomEditor(typeof(NetworkBehaviour), true)]
public class OdinNetworkBehaviourEditor : OdinEditor { }
#endif