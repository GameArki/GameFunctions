using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFunctions {

    struct Room {
        public int id;
        public HashSet<Vector2Int> spaces;
    }

    public class Sample_GFConfineSpace : MonoBehaviour {

        HashSet<Vector2Int> barrierSet;
        Vector2Int[] result;
        Dictionary<int, Room> roomDict;

        const int LIMIT_COUNT = 24 * 6;

        void Start() {
            barrierSet = new HashSet<Vector2Int>();
            result = new Vector2Int[LIMIT_COUNT];
            roomDict = new Dictionary<int, Room>();
        }

        void Update() {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int mousePosInt = new Vector2Int(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y));
            if (Input.GetMouseButton(0)) {
                barrierSet.Add(mousePosInt);

                Span<Vector2Int> brothers = stackalloc Vector2Int[4];
                brothers[0] = mousePosInt + new Vector2Int(0, 1);
                brothers[1] = mousePosInt + new Vector2Int(1, 0);
                brothers[2] = mousePosInt + new Vector2Int(0, -1);
                brothers[3] = mousePosInt + new Vector2Int(-1, 0);

                for (int i = 0; i < 4; i++) {
                    Vector2Int cur = brothers[i];
                    if (ExistRoom(cur)) {
                        continue;
                    }
                    int spaceCount = GFConfineSpaceV2.Process(
                        cur,
                        LIMIT_COUNT,
                        IsWalkable,
                        result
                    );

                    // TODO: Cell must has a room id, to avoid duplicate room
                    if (spaceCount > 0) {
                        Room room = new Room();
                        room.id = roomDict.Count;
                        room.spaces = new HashSet<Vector2Int>(spaceCount);
                        for (int j = 0; j < spaceCount; j++) {
                            room.spaces.Add(result[j]);
                        }
                        roomDict.Add(room.id, room);
                    }
                }
            }
        }

        bool ExistRoom(Vector2Int pos) {
            foreach (Room room in roomDict.Values) {
                if (room.spaces.Contains(pos)) {
                    return true;
                }
            }
            return false;
        }

        bool IsWalkable(Vector2Int pos) {
            return !barrierSet.Contains(pos);
        }

        void OnDrawGizmos() {

            // Draw Mouse
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int mousePosInt = new Vector2Int(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y));
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(new Vector3(mousePosInt.x, mousePosInt.y), Vector3.one);

            // Draw rooms
            if (roomDict != null) {
                foreach (Room room in roomDict.Values) {
                    Color roomColor = Color.green;
                    roomColor.b += room.id * 0.01f;
                    Gizmos.color = roomColor;
                    foreach (Vector2Int pos in room.spaces) {
                        Gizmos.DrawCube(new Vector3(pos.x, pos.y), Vector3.one);
                    }
                }
            }

            // Draw barrier
            if (barrierSet != null) {
                Gizmos.color = Color.red;
                foreach (Vector2Int pos in barrierSet) {
                    Gizmos.DrawCube(new Vector3(pos.x, pos.y), Vector3.one);
                }
            }
        }

    }

}
