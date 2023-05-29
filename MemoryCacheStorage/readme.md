# MemoryCacheStorage

Written by CypherPotato

This package provides the ability to store objects for a period of
time and retrieve them later. If the timeout time is exceeded, the
file is dereferenced and the GC does the work of collecting the
object if it is not referenced later.

There is no timer or recurrence. No objects are copied or serialized.
The class stores the reference to the object, and after the time it
runs in an asynchronous task, the pointer is released.

## Setting objects

```csharp
// initialize the new cache object
var cache = new MemoryCacheStorage();
cache.DefaultExpirity = TimeSpan.FromSeconds(30);
```

```csharp
// Caches the object below for the default time defined above (30 seconds)
// and gets an ID for the stored object.
int id = cache.Set(myCachedObject);

// Caches the object for 30 seconds, sets an tag on it and gets the ID
id = cache.Set(myCachedObject, "my-tag");

// Caches the object for 5 minutes (override the default), set a tag
// and returns the id
id = cache.Set(myCachedObject, TimeSpan.FromMinutes(5), "tag");

// Caches an object using multiple tags
id = cache.Set(myCachedObject, TimeSpan.FromHours(3), new string[] { "tag1", "tag2" });
```

## Retrieves cacheds objects

```csharp
// tries to retrieve an cached object by their ID
if (!cache.TryGetValue(id, out object? cachedValue)) {
	// couldn't find the cached object
} else {
	// found the cached object
}
```

```csharp
// tries to retrieve an cached object by one of their tags
if (!cache.TryGetValue("tag1", out object? cachedValue)) {
	// couldn't find the cached object by the tag
} else {
	// found the cached object
}
```

```csharp
// tries to get an cached object, and converts it on fly
if (cache.TryGetValue("tag3", out MyClassObject? cachedValue)) {
	// cachedValue was found
} else {
	// not found
}
```

```csharp
// retrieves multiple cached objects by tag
object?[] cachedObjects = cache.GetValuesByTag("tag1").ToArray();
```

```csharp
// retrieves multiple cached objects by predicate
object?[] cachedObjects = cache
	.GetValuesByTag(tags => tags.Contains("tag1") && tags.Contains("tag2"))
	.ToArray();
```

```csharp
// gets the count of cached objects
int cachedObjects = cache.Count;
```

## Invalidate objects

```csharp
// force immediate invalidation of object by one of their tags
int invalidatedObjects = cache.Invalidate("tag1");
```

```csharp
// force immediate invalidation of object by one of their ID
int invalidatedObjects = cache.Invalidate(3);
```

```csharp
// force immediate invalidation of all objects matching the tag
int invalidatedObjects = cache.Invalidate(tags => tags.Length > 3);
```

```csharp
// clears the cache
cache.Clear();
```