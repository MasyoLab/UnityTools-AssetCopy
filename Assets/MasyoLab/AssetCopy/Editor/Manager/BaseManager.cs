#if UNITY_EDITOR

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-AssetCopy
//
//=========================================================

namespace MasyoLab.Editor.AssetCopy {
    class BaseManager {
        protected IPipeline m_pipeline = null;

        public BaseManager(IPipeline pipeline) {
            m_pipeline = pipeline;
        }
    }
}
#endif
