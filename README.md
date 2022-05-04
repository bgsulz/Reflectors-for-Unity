# Reflectors for Unity
Ultra-powerful and blazing fast reflection utilities for Unity.

### FieldReference
Dynamically reference a field, property, or parameter-less method on any Object.

```cs
using Extra.Editor.Properties;
using UnityEngine;

public class ColorMatcher : MonoBehaviour
{
    [SerializeField] private FieldReference<Color> colorProperty;

    private SpriteRenderer _renderer;

    private void Start()
    {
        colorProperty.Initialize();
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        _renderer.color = colorProperty.Value;
    }
}
```

![FieldReference Demo](https://user-images.githubusercontent.com/38191432/166614302-946f456a-b880-408d-8c10-3b3b4c195ac6.gif)

###### But isn't this slow?
Nope. The `Initialize` method compiles an Expression that retrieves the selected property. In terms of elapsed time, accessing a field is roughly **2x slower** than compiled code, whereas accessing a property or method is **nearly compile-time fast.**

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
