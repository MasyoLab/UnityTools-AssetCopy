#if UNITY_EDITOR

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-AssetCopy
//
//=========================================================

namespace MasyoLab.Editor.AssetCopy {

    static class StringExtensions {
        public static string RemoveAtLast(this string self, string value) {
            return self.Remove(self.LastIndexOf(value), value.Length);
        }
    }
}
#endif
