using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GameClasses.RuleTileDrawer;

namespace GameFunctions.Sample {

    public class Sample_RuleTimeDrawer : MonoBehaviour {

        [SerializeField] RuleTileBakerSO bakerSO;
        [SerializeField] Tilemap tilemap;

        void Start() {
            bakerSO.Init();
        }

        void Update() {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = tilemap.WorldToCell(mousePos);
            if (Input.GetMouseButtonDown(0)) {
                bakerSO.FillOneCell(tilemap, new Vector2Int(cellPos.x, cellPos.y), true);
            }
        }

    }

}
