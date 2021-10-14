using System;

namespace ValidataAPI.Utils.Generators
{
    public class GuidUtils
    {
        private static bool _isFrozen;
        private static Guid? _guidSet;

        private GuidUtils()
        {
            
        }
        public static void Freeze(Guid guid)
        {
            _isFrozen = true;
            _guidSet = guid;
        }
        
        public static void UnFreeze()
        {
            _isFrozen = false;
            _guidSet = null;
        }
        
        public static Guid New()
        {
            if (_isFrozen)
            {
                return _guidSet ?? Guid.NewGuid();
            }
            return Guid.NewGuid();
        }
    }
}