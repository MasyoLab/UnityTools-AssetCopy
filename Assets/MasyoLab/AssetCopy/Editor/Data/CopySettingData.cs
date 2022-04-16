#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-AssetCopy
//
//=========================================================

namespace MasyoLab.Editor.AssetCopy {

    [System.Serializable]
    class CopySettingData {
        [SerializeField]
        private string ext = string.Empty;
        public string Extension {
            set {
                ext = value;
            }
            get {
                return ext;
            }
        }

        public CopySettingData() {

        }
        public CopySettingData(string extension) {
            ext = extension;
        }
    }

    [System.Serializable]
    class CopySettingDataJson {
        [SerializeField]
        private List<CopySettingData> cs = new List<CopySettingData>();

        public static string ToJson(IReadOnlyList<CopySettingData> copyPaths) {
            return JsonUtility.ToJson(new CopySettingDataJson {
                cs = new List<CopySettingData>(copyPaths)
            });
        }

        public static List<CopySettingData> FromJson(string jsonData) {
            var settingDataJson = JsonUtility.FromJson<CopySettingDataJson>(jsonData);
            if (settingDataJson == null) {
                var settingDatas = new List<CopySettingData>();
                settingDatas.Add(new CopySettingData(".png"));
                settingDatas.Add(new CopySettingData(".jpg"));
                return settingDatas;
            }
            return settingDataJson.cs;
        }
    }
}
#endif
