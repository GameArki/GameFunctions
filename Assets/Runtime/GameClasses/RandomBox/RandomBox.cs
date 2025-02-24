using System;
using System.Collections.Generic;

namespace GameClasses {

    public class RandomBox {

        Random random;
        int seed;

        public RandomBox(int seed) {
            this.seed = seed;
            random = new Random(seed);
        }

        public T Roll<T>(Span<(T obj, float rate)> inputs) {

            int len = inputs.Length;

            float total = 0;
            for (int i = 0; i < len; i++) {
                total += inputs[i].rate;
            }

            float roll = (float)random.NextDouble() * total;
            for (int i = 0; i < len; i++) {
                roll -= inputs[i].rate;
                if (roll <= 0) {
                    return inputs[i].obj;
                }
            }

            return inputs[len - 1].obj;
        }

        public T Roll<T>(IList<(T obj, float rate)> inputs) {

            int len = inputs.Count;

            float total = 0;
            for (int i = 0; i < len; i++) {
                total += inputs[i].rate;
            }

            float roll = (float)random.NextDouble() * total;
            for (int i = 0; i < len; i++) {
                roll -= inputs[i].rate;
                if (roll <= 0) {
                    return inputs[i].obj;
                }
            }

            return inputs[len - 1].obj;
        }

    }

}
