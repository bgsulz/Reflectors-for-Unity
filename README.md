# Reflectors for Unity

**Fast and powerful reflection utilities for Unity.**

| [Documentation](Documentation) | [Benchmarks](Documentation/Benchmarks.md) |
| - | - |

### How fast is it, really?

Hopefully, fast enough!

| Field access | Elapsed time (ticks/100,000 accesses) | Ratio vs. compiled |
| - | - | - |
| Compiled | 37,515 | 1x |
| Getter | 43,204 | 1.152x |
| Reflection | 317,665 | 8.468x |

### Quick Start Guide
This works by building a `Getter` or `Setter` object. These objects can be used to read or write to a specific property of a specific type.

1. Call the `Build` method to construct one, passing in a root type (`SpriteRenderer`, in this case) and a property name ("color".)
2. Pass in the type of the named property ("color") as a generic type parameter (`Color`.)
3. Use the `GetValue` or `SetValue` method on an object of the root type.

```cs
public SpriteRenderer root;

private Getter<Color> _getter;
private Setter<Color> _setter;

void Awake()
{
	_getter = Getter.Build<Color>(typeof(SpriteRenderer), "color");
	_setter = Setter.Build<Color>(typeof(SpriteRenderer), "color");
}

void Start()
{
	var retrievedColor = _getter.GetValue(root);
	_setter.SetValue(root, Color.yellow);
}
```

#### How does this work?
`Getter` and `Setter` are simply wrappers around code that compiled a LINQ `Expression` via the C# [expression tree API.](https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/expression-trees/expression-trees-building)

More benchmarks can be found [here.](Documentation/Benchmarks.md)

***

## Extra Applications

This repository includes a couple common applications for which `Getter`s and `Setter`s are useful. 

### PropertyReference
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

![PropertyReference Demo](https://user-images.githubusercontent.com/38191432/166614302-946f456a-b880-408d-8c10-3b3b4c195ac6.gif)

This is similar to building a `Getter`, but:
- It references a specific property on a specific object, not any object of the root type.
- It has a really nice custom property drawer.

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
