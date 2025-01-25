using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameClasses.RuleTileDrawer.Internal {

    [Serializable]
    public class ListWithIndex<T> {

        [SerializeField] List<T> list;
        int index;

        public ListWithIndex() {
            list = new List<T>();
            index = 0;
        }

        public void Add(T item) {
            list.Add(item);
        }

        public T Next() {
            index++;
            index %= list.Count;
            return list[index];
        }

        public T FindIndex(Predicate<T> match) {
            return list.Find(match);
        }

    }

}