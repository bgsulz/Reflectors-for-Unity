# Reflectors for Unity
Ultra-powerful and blazing fast reflection utilities for Unity.

| Field access | Elapsed time (ticks/100,000 accesses) | Ratio vs. compiled |
| - | - | - |
| Compiled | 37,515 | 1x |
| Getter | 43,204 | 1.152x |
| Reflection | 317,665 | 8.468x |

***

### FieldReference
Dynamically reference a field, property, or parameter-less method on any Object.
_Great for consolidating scripts whose only difference is a type!_

```cs
[SerializeField] private PropertyReference<Color> colorProperty;

private void Start()
{
    colorProperty.Initialize();
    GetComponent<SpriteRenderer>().color = colorProperty.Value;
} 
```

![FieldReference Demo](https://user-images.githubusercontent.com/38191432/166614302-946f456a-b880-408d-8c10-3b3b4c195ac6.gif)

***

### ShowIf
Easy, dynamic custom Inspector. Only uses PropertyDrawers for total compatibility with existing custom editors.

```cs
public bool shouldUseRaycast;
[ShowIf("shouldUseRaycast")] public float raycastLength;
[HideIf("shouldUseRaycast")] public float detectionRadius;

public bool useCustomTitle;
[DisableIf("useCustomTitle")] public string defaultTitle;
[EnableIf("useCustomTitle")] public string overrideTitle;
```

***

### Getter
All previous APIs use this behind the scenes. You can build a Getter for your own purposes.

```cs
[SerializeField] private SpriteRenderer powerUpSprite;
[SerializeField] private SpriteRenderer debuffSprite;

private Getter<Color> _getter;

private void Start()
{
    _getter = Getter.Build<Color>(typeof(SpriteRenderer), "color");
    var powerUpColor = _getter.GetValue(powerUpSprite);
    var debuffColor = _getter.GetValue(debuffSprite);
}
```

#### How is this reflection so fast?
Only the `Initialize` method uses traditional reflection. Then, it compiles a LINQ `Expression` into a Getter `Func`. As shown in the benchmarks above, `Func` is suitably fast for use at runtime.

Complete benchmarks can be found [here.](Documentation/Benchmarks.md)

***

## How do I add this to Unity?
It's easy!

#### If you have Git...
1. Open the Unity Editor. On the top ribbon, click Window > Package Manager.
2. Click the + button in the upper left corner, and click "Add package from git url..."
3. Enter "https://github.com/bgsulz/Reflectors-for-Unity.git"
4. Enjoy!

#### If you don't have Git (or want to modify the code)...
1. Click the big green "Code" button and click Download ZIP.
2. Extract the contents of the .zip into your Unity project's Assets folder.
3. Enjoy!
