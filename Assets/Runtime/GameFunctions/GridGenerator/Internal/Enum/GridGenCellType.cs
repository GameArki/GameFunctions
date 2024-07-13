namespace GameFunctions.GridGeneratorInternal {

    public enum GridGenCellType {
        None,
        Land = 1 << 0,
        Sea = 1 << 1,
        Lake = 1 << 2,
        Forest = 1 << 3,

        Water = Sea | Lake,
    }

    public static class CellTypeExtension {

        internal static bool IsWater(this GridGenCellType type) {
            return (type & GridGenCellType.Water) == type;
        }
    }
}