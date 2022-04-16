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

    struct CONST {
        public const string DEVELOPER = "MasyoLab";
        public const string VERSION = "v1.3.5";
        public const string EDITOR_NAME = "AssetCopyWindow";
        public const string EDITOR_WINDOW_NAME = "AssetCopy";
        public const string MENU_ITEM = "Tools/" + EDITOR_WINDOW_NAME;
        public const string UNITY_EXT = ".unity";
        public const string FILE_EXTENSION = "assetcopy";
        public const string COPY_FILE_NAME = "assetcopy";
        public const string COPY_SETTING_FILE_NAME = "copy_setting";
        public const string SAVE = "Save";
        public const string LOAD = "Load";
        public const string SELECT = "Select";
        public const string ASSETS = "Assets";
        public const string LIBRARY = "Library";
        public static string FOLDER_NAME => $"{DEVELOPER}@{EDITOR_WINDOW_NAME}";

        public const int GUI_ITEM_SIZE_HEIGHT = 25;

        /// <summary>
        /// アイコン：https://github.com/halak/unity-editor-icons
        /// </summary>

        /// <summary>
        /// リフレッシュ
        /// </summary>
        public const string REFRESH = "Refresh@2x";
    }

    enum WindowEnum {
        Max,
    }
}
#endif
