namespace GameClasses.BehaviourTree {

    public enum BHTreeContainerType {
        None,               // Is a leaf node(action)
        Sequence,           // Run children in order until all done
        SelectorSeq,        // Run children in order until one done
        SelectorRandom,     // Run children in random order until one done
        ParallelOr,         // Run all children in parallel until one done
        ParallelAnd,        // Run all children in parallel until all done
    }

}