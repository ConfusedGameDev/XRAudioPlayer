namespace Synesthesure
{
    public static class GradientCacheWrapper
    {
        private static System.Action _ClearCache = null;
        public static void ClearCache()
        {
#if UNITY_EDITOR
            if (_ClearCache == null)
            {
                var gradientCacheType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditorInternal.GradientPreviewCache");
                if (gradientCacheType == null)
                    throw new System.Exception("Unity internal type GradientPreviewCache could not be found");
                var clearCacheMethod = gradientCacheType.GetMethod("ClearCache", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                if (clearCacheMethod == null)
                    throw new System.Exception("Unity internal type GradientPreviewCache doesn't have a method called ClearCache anymore");
                _ClearCache = (System.Action)System.Delegate.CreateDelegate(typeof(System.Action), clearCacheMethod);
            }
            _ClearCache();
#endif
        }
    }
}
