## Components

This is a very simple and naive implementation of the ECS. The interface will be the same (or very similar), but the implementation of the component pools and how memory is allocated should be updated when we get to a more stable state.
Some ideas
* Dynamic allocation for component pools, only allocate more memory when needed (allocate a block(PAGE_SIZE) of virtual memory, and expand when needed)
* Pack some components together, archetype style. And use a Stride to jump to next index (this is supported today, but not really implemented as that)
