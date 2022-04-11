#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-AssetCopy
//
//=========================================================

namespace MasyoLab.Editor.AssetCopy {
    interface IPipeline {
        CopyPathManager CopyPathManager { get; }
        EditorWindow Root { get; }
        Rect WindowSize { get; }
    }
}
#endif