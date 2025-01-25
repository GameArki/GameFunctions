using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameClasses.RuleTileDrawer {

    [CreateAssetMenu(fileName = "So_GFRuleTile_", menuName = "GameFunctions/GFRuleTileSO", order = 1)]
    public class RuleTileSO : TileBase {

        // 不处理 Rule
        // 仅用于存储数据
        public short typeID;
        public Sprite spr_default;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            tileData.sprite = spr_default;
        }

        public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
            tilemap.RefreshTile(position);
        }

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go) {
            return base.StartUp(position, tilemap, go);
        }

    }

}
