#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-AssetCopy
//
//=========================================================

namespace MasyoLab.Editor.AssetCopy {

    [System.Serializable]
    class CopyPathData {

        [SerializeField]
        private string s = string.Empty;
        public string SourceDirectory {
            set {
                s = value;
            }
            get {
                return s;
            }
        }

        [SerializeField]
        private string f = string.Empty;
        public string FolderGUID {
            set {
                f = value;
            }
            get {
                return f;
            }
        }

        public string DestinationDirectory {
            get {
                return Application.dataPath.TrimEnd("Asset".ToCharArray()) + AssetDatabase.GUIDToAssetPath(FolderGUID);
            }
        }

        private Object _folderAsset { get; set; } = null;
        public Object FolderAsset {
            set {
                _folderAsset = value;
            }
            get {
                if (_folderAsset == null) {
                    _folderAsset = GetFolderAsset();
                }
                return _folderAsset;
            }
        }
        public bool IsUnity {
            get {
                return SourceDirectory.Contains(Application.dataPath);
            }
        }
        public bool IsRemove { get; set; } = false;
        public bool IsCopy { get; set; } = false;

        private Object GetFolderAsset() {
            // GUIDでパスを取得
            var assetPath = AssetDatabase.GUIDToAssetPath(FolderGUID);
            if (assetPath == string.Empty) {
                return null;
            }

            // アセットを取得
            var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            if (asset == null) {
                return null;
            }

            return asset;
        }
    }

    [System.Serializable]
    class CopyPathDataJson {

        [SerializeField]
        private List<CopyPathData> d;

        public static string ToJson(IReadOnlyList<CopyPathData> copyPaths) {
            return JsonUtility.ToJson(new CopyPathDataJson {
                d = new List<CopyPathData>(copyPaths)
            });
        }

        public static List<CopyPathData> FromJson(string jsonData) {
            return JsonUtility.FromJson<CopyPathDataJson>(jsonData).d;
        }
    }
}
#endif
