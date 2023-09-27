using System;
using UnityEngine;

namespace BackgroundDownload.Utils
{
    /// <summary>
    /// Allows retrieving a value of T and caching it, in case calculating or obtaining it is expensive.
    /// </summary>
    public class Cached<T>
    {
        private const int DEFAULT_FRAMES_BETWEEN_UPDATES = 30;

        private long frameWhenLastUpdated;
        private readonly int framesBetweenUpdates = DEFAULT_FRAMES_BETWEEN_UPDATES;

        private readonly Func<T> fetchValue;

        private T cachedValue;

        public Cached(Func<T> fetchValue)
        {
            if (fetchValue == null)
            {
                throw new ArgumentNullException("fetchValue");
            }

            this.fetchValue = fetchValue;
        }

        /// <summary>
        /// Retrieves the value cached by this instance (might be an expensive operation).
        /// </summary>
        /// <returns></returns>
        protected virtual T GetValue()
        {
            return fetchValue();
        }

        public T Value
        {
            get
            {
                // calculate frame diff
                var currentFrame = Time.frameCount;

                // check if we should fetch the value again
                if (currentFrame - frameWhenLastUpdated >= framesBetweenUpdates)
                {
                    cachedValue = GetValue();
                    frameWhenLastUpdated = currentFrame;
                }

                return cachedValue;
            }
        }

        public static implicit operator T(Cached<T> cached)
        {
            return cached.Value;
        }
    }
}