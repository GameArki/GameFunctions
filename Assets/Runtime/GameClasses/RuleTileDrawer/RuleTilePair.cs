using System;
using System.Collections.Generic;

#pragma warning disable 0660
#pragma warning disable 0661

namespace GameClasses.RuleTileDrawer.Internal {

    [Serializable]
    public struct RuleTilePair {
        public RuleTileRelationDescription relation;
        public ListWithIndex<RuleTileSO> tileSO;
    }

}