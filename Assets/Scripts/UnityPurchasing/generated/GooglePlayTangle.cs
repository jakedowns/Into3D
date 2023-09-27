// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("pH3vQgqn5AJ19iFjh/Nk00eE1earYzAJnkqT3hqhLJGVd+Pp2BdVw4Heuyw3b+0pR9aowNLCai7nBjyyhzW2lYe6sb6dMf8xQLq2trayt7RdoZyyawiOozCZYTglLnB4z1B30OYfDPjAYHbpPgEDEqEE081aNPmhgTzAdI2rJfKGoITV2hhyC4fVLlq5qhFyDxUkv9Z1mr8RiiZUC3bqsxj2zSFr+6brC9ifNFO3xf6ifBt9oJ4/O/cS4WlViBLyNGeq7N20mqD9k7K46ushmnlswpi0HMmya9nZhay825ctxsV9RHM25XIf2TYM31cwNba4t4c1tr21Nba2twYu92PTz6bCYESD6B5RBCrFAQGkhWHeahFPAhpX1NXzJbPTRrW0tre2");
        private static int[] order = new int[] { 12,10,13,12,6,12,9,9,8,9,11,12,12,13,14 };
        private static int key = 183;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
