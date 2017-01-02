## What is HyperState?
HyperState is a library that provides high-performance access to objects that may exist in multiple states, including native, serialized, and persisted states.
## Project Goals
* Provide seamless access to objects in local memory (native), shared memory (serialized to cache), and persistent (data store) states
* Extensible architecture to allow flexible implementation of
 * Object serialization
 * Shared memory cache provider
 * Persistent data store provider
* Avoid unnecessary serialization and deserialization of objects between application layers, examples
 * Web GET < cache
 * Web POST/PUT > cache > persist serialized (e.g. JSON NoSQL)
 
