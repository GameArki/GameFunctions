```
// 1.
var baker = new CellBaker(width, height);

// 2.
baker.Fill(typeID);

// 3. Loop
int startIndex = baker.Select(selectOption);
baker.Gen(startIndex, genOption);
```