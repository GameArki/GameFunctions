namespace GameClasses.BehaviourTree {

    public enum BHTreeActionNodeExecuteType {
        NotEntered,     // Need To Judge PreCondition
        EnterFailed,    // PreCondition Failed
        Running,
        Done,
    }

}