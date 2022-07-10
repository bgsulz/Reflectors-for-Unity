# Reflectors for Unity
Ultra-powerful and blazing fast reflection utilities for Unity.

***

### FieldReference
Dynamically reference a field, property, or parameter-less method on any Object.
_Great for consolidating scripts whose only difference is a type!_

```cs
[SerializeField] private FieldReference<Color> colorProperty;

private void Start()
{
    colorProperty.Initialize();
    GetComponent<SpriteRenderer>().color = colorProperty.Value;
} 
```

![FieldReference Demo](https://user-images.githubusercontent.com/38191432/166614302-946f456a-b880-408d-8c10-3b3b4c195ac6.gif)

***

### Show-/Hide-/Enable-/Disable-If Attribute
Easy, dynamic custom Inspector with exclusively PropertyDrawers -- compatible with your existing custom editors.

```cs
[SerializeField] private bool shouldUseRaycast;
[SerializeField, ShowIf("shouldUseRaycast")] private float raycastLength;
[SerializeField, HideIf("shouldUseRaycast")] private float detectionRadius;
```

***

### Getter
All previous APIs use this behind the scenes. Build a Getter for your own purposes.

```cs
private const string PropertyPath = "color";

[SerializeField] private SpriteRenderer powerUpSprite;

private Getter<Color> _getter;

private void Start()
{
    _getter = Getter.Build<Color>(powerUpSprite, PropertyPath);
    var powerUpColor = _getter.Value;
}
```

#### But isn't this reflection? That's too slow for use at runtime, right?
Only the `Initialize` method uses reflection. Then, it compiles a LINQ `Expression` into a `Func<TRoot, TReturn>`, where `TRoot` is the root type and `TReturn` is the retrieved type.

Running this `Func` retrieves the value with nearly compile-time speed. It is suitable for use at runtime. Accessing fields from the Unity's C++ layer (say, `transform.position`) is sometimes even faster than compiled code.

| Access type | Elapsed time (ticks/100,000 accesses) |
| - | - |
| Compiled, Field | 37514.5 |
| Generic Getter, Field | 42212 |
| Specific Getter, Field | 43203.8 |
| Property Getter, Field | 46247 |
| Reflected Getter, Field | 317665 |
| Compiled, Property | 42863 |
| Generic Getter, Property | 49247.1 |
| Specific Getter, Property | 51104.5 |
| Property Getter, Property | 53829.1 |
| Reflected Getter, Property | 156328.8 |
| Compiled, Method | 40749.9 |
| Generic Getter, Method | 50659.3 |
| Specific Getter, Method | 53297 |
| Property Getter, Method | 54570.3 |
| Reflected Getter, Method | 895844 |

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
