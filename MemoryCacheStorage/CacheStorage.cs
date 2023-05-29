namespace CypherPotato.CacheStorage
{
    /// <summary>
    /// Provides a thread-safe memory cache that stores objects for a certain period of time.
    /// </summary>
    public class MemoryCacheStorage
    {
        private List<CacheStorageItem> _collection = new List<CacheStorageItem>();
        private long incrementingId = 0;

        /// <summary>
        /// Creates an new instance of <see cref="MemoryCacheStorage"/>.
        /// </summary>
        public MemoryCacheStorage()
        {
        }

        /// <summary>
        /// Gets or sets the default expirity time for newly cached objects.
        /// </summary>
        public TimeSpan DefaultExpirity { get; set; } = TimeSpan.FromMinutes(10);

        /// <summary>
        /// Gets the current cached items count.
        /// </summary>
        public int Count
        {
            get
            {
                lock (_collection)
                {
                    return _collection.Count;
                }
            }
        }

        /// <summary>
        /// Clears the cache memory and restarts the ID to zero.
        /// </summary>
        public void Clear()
        {
            lock (_collection)
            {
                Interlocked.Exchange(ref incrementingId, 0);
                _collection.Clear();
            }
        }

        /// <summary>
        /// Attempts to gets a cached value through its ID, and if successful, a true boolean value is returned.
        /// </summary>
        /// <param name="id">The ID of the cached object.</param>
        /// <param name="value">The cached object, returned by reference.</param>
        /// <returns>A Boolean indicating whether the object was found or not.</returns>
        public bool TryGetValue(long id, out object? value)
        {
            lock (_collection)
            {
                foreach (var item in _collection)
                {
                    if (item.Identifier == id)
                    {
                        value = item.Value;
                        return true;
                    }
                }
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Attempts to gets a cached value through its ID, and if successful, a true boolean value is returned.
        /// </summary>
        /// <param name="id">The ID of the cached object.</param>
        /// <param name="value">The cached object, returned by reference.</param>
        /// <returns>A Boolean indicating whether the object was found or not.</returns>
        public bool TryGetValue<T>(long id, out T? value)
        {
            lock (_collection)
            {
                foreach (var item in _collection)
                {
                    if (item.Identifier == id)
                    {
                        value = (T?)item.Value;
                        return true;
                    }
                }
            }
            value = default;
            return false;
        }

        /// <summary>
        /// Attempts to gets a cached value through their tag part, and if successful, a true boolean value is returned.
        /// </summary>
        /// <param name="tag">One of the tags that must be present on the object.</param>
        /// <param name="value">The cached object, returned by reference.</param>
        /// <returns>A Boolean indicating whether the object was found or not.</returns>
        public bool TryGetValue(string tag, out object? value)
        {
            lock (_collection)
            {
                foreach (var item in _collection)
                {
                    if (item.Tags?.Contains(tag) == true)
                    {
                        value = item.Value;
                        return true;
                    }
                }
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Attempts to gets a cached value through their tag part, and if successful, a true boolean value is returned.
        /// </summary>
        /// <param name="tag">One of the tags that must be present on the object.</param>
        /// <param name="value">The cached object, returned by reference.</param>
        /// <returns>A Boolean indicating whether the object was found or not.</returns>
        public bool TryGetValue<T>(string tag, out T? value)
        {
            lock (_collection)
            {
                foreach (var item in _collection)
                {
                    if (item.Tags?.Contains(tag) == true)
                    {
                        value = (T?)item.Value;
                        return true;
                    }
                }
            }
            value = default;
            return false;
        }

        /// <summary>
        /// Returns an array of all cached objects that share the referenced tag.
        /// </summary>
        /// <param name="tag">The tag present on all stored objects that will return in this query.</param>
        /// <returns>An enumerable of objects with the values found in those tags.</returns>
        public IEnumerable<object?> GetValuesByTag(string tag)
        {
            lock (_collection)
            {
                foreach (var item in _collection)
                {
                    if (item.Tags?.Contains(tag) == true)
                    {
                        yield return item.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Returns an array of all cached objects that matches the specified predicate function.
        /// </summary>
        /// <param name="predicate">The function that returns an boolean indicating if the tags is matched or not.</param>
        /// <returns>An enumerable of objects with the values matched by the predicate.</returns>
        public IEnumerable<object?> Where(Func<string[], bool> predicate)
        {
            lock (_collection)
            {
                foreach (var item in _collection)
                {
                    if (item.Tags == null) continue;
                    if (predicate(item.Tags) == true)
                    {
                        yield return item.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to anticipate the invalidation of an cached object by its ID and returns a boolean indicating the success of the operation.
        /// </summary>
        /// <param name="id">The ID of the cached object.</param>
        /// <returns>An integer indicating how much objects was invalidated.</returns>
        public int Invalidate(long id)
        {
            lock (_collection)
            {
                foreach (var item in _collection)
                {
                    if (item.Identifier == id)
                    {
                        item.Invalidate();
                        return 1;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// Attempts to invalidate all cached objects that contains the specified tag.
        /// </summary>
        /// <param name="tag">One of the tags that must be present on the cached objects.</param>
        /// <returns>An integer indicating how much objects was invalidated.</returns>
        public int Invalidate(string tag)
        {
            int removed = 0;
            lock (_collection)
            {
                foreach (var item in _collection)
                {
                    if (item.Tags?.Contains(tag) == true)
                    {
                        item.Invalidate();
                        removed++;
                    }
                }
            }
            return removed;
        }

        /// <summary>
        /// Attempts to invalidate all cached objects that matches the specified expression.
        /// </summary>
        /// <param name="removePredicate">The predicate that determines if the object should be removed or not.</param>
        /// <returns>An integer indicating how much objects was invalidated.</returns>
        public int Invalidate(Func<string[], bool> removePredicate)
        {
            int removed = 0;
            lock (_collection)
            {
                foreach (var item in _collection)
                {
                    if (item.Tags == null) continue;
                    if (removePredicate(item.Tags) == true)
                    {
                        item.Invalidate();
                        removed++;
                    }
                }
            }
            return removed;
        }

        /// <summary>
        /// Stores a value in cache.
        /// </summary>
        /// <param name="value">The value which will be cached.</param>
        /// <param name="expirity">The time for the object to be invalidated in the cache.</param>
        /// <param name="tags">Tags to be able to fetch that object later.</param>
        /// <returns>Returns the ID of the cached object.</returns>
        public long Set(object? value, TimeSpan expirity, string[]? tags)
        {
            // verify if the incrementing Id is larger than long.maxvalue
            Interlocked.CompareExchange(ref incrementingId, 0, long.MaxValue - 1);

            CacheStorageItem i = new CacheStorageItem(ref value, this, expirity)
            {
                Identifier = Interlocked.Increment(ref incrementingId),
                Tags = tags
            };
            return i.Identifier;
        }

        /// <summary>
        /// Stores a value in cache.
        /// </summary>
        /// <param name="value">The value which will be cached.</param>
        /// <param name="expirity">The time for the object to be invalidated in the cache.</param>
        /// <param name="tag">Tag to be able to fetch that object later.</param>
        /// <returns>Returns the ID of the cached object.</returns>
        public long Set(object? value, TimeSpan expirity, string tag)
        {
            return Set(value, expirity, new string[] { tag });
        }

        /// <summary>
        /// Stores a value in cache using the default expirity time.
        /// </summary>
        /// <param name="value">The value which will be cached.</param>
        /// <param name="tag">Tag to be able to fetch that object later.</param>
        /// <returns>Returns the ID of the cached object.</returns>
        public long Set(object? value, string tag)
        {
            return Set(value, DefaultExpirity, new string[] { tag });
        }

        /// <summary>
        /// Stores a value in cache using the default expirity time and no tag.
        /// </summary>
        /// <param name="value">The value which will be cached.</param>
        /// <returns>Returns the ID of the cached object.</returns>
        public long Set(object? value)
        {
            return Set(value, DefaultExpirity, Array.Empty<string>());
        }

        protected void InvokeRemove(CacheStorageItem item)
        {
            lock (_collection)
            {
                _collection.Remove(item);
            }
        }

        protected void InvokeAdd(CacheStorageItem item)
        {
            lock (_collection)
            {
                _collection.Add(item);
            }
        }

        protected class CacheStorageItem
        {
            public object? Value { get; private set; }
            public long Identifier { get; set; }
            public string[]? Tags { get; set; }
            public MemoryCacheStorage Parent { get; set; }
            public TimeSpan Expires { get; private set; }

            bool isInvalidated = false;

            private async void Collect()
            {
                await Task.Delay(Expires);
                Invalidate();
            }

            internal void Invalidate()
            {
                if (isInvalidated) return;
                Value = null;
                isInvalidated = true;
                this.Parent.InvokeRemove(this);
            }

            public CacheStorageItem(ref object? value, MemoryCacheStorage parent, TimeSpan expirity)
            {
                this.Value = value;
                this.Expires = expirity;
                this.Parent = parent;
                this.Parent.InvokeAdd(this);
                Collect();
            }

            public override bool Equals(object? obj)
            {
                CacheStorageItem? other = obj as CacheStorageItem;
                if (other == null) return false;
                if (other.Identifier == this.Identifier) return true;
                return false;
            }

            public override int GetHashCode()
            {
                return (int)this.Identifier;
            }
        }
    }
}