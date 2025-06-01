using System;
using System.Collections.Generic;

public class Pool<T> {

    Stack<T> stack;
    Func<T> createFunc;
    int size;

    public Pool(int size, Func<T> createFunc) {
        this.size = size;
        this.createFunc = createFunc;
        stack = new Stack<T>(size);
        for (int i = 0; i < size; i++) {
            var obj = createFunc();
            stack.Push(obj);
        }
    }

    public T Get() {
        if (stack.Count == 0) {
            return createFunc();
        }
        return stack.Pop();
    }

    public void Return(T t) {
        stack.Push(t);
    }

}