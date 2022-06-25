# Reflectors for Unity
Ultra-powerful and blazing fast reflection utilities for Unity.

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

***

### FieldReference
Dynamically reference a field, property, or parameter-less method on any Object.

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
Only the `Initialize` method uses reflection; it gathers `MethodInfo` objects and compiles an `Expression` that retrieves the selected property. 

In terms of elapsed time, accessing a field is roughly **2x slower** than compiled code, whereas accessing a property or method is **nearly compile-time fast.**

Benchmarks are soon to come.
