using System;
using System.Collections.Generic;

namespace GameClasses {

    public class ReadOnlyList<T> {

        List<T> list;

        public ReadOnlyList(List<T> list) {
            this.list = list;
        }

        public T this[int index] {
            get {
                return list[index];
            }
        }

        public int Count {
            get {
                return list.Count;
            }
        }

        public bool Contains(T item) {
            return list.Contains(item);
        }

        public void ForEach(Action<T> action) {
            list.ForEach(action);
        }

        public int FindIndex(Predicate<T> match) {
            return list.FindIndex(match);
        }

        public T Find(Predicate<T> match) {
            return list.Find(match);
        }

    }

}