namespace GameFunctions.GridGeneratorInternal {

    public enum CellType {
        None,
        Land = 1 << 0,
        Sea = 1 << 1,
        Lake = 1 << 2,
        Forest = 1 << 3,

        Water = Sea | Lake,
    }

    public static class CellTypeExtension {

        public static bool IsWater(this CellType type) {
            return (type & CellType.Water) != 0;
        }
    }
}