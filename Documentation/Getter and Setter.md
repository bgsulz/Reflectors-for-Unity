# Getter\<T\> and Setter\<T\>

These classes make it easy to compile property accessors at runtime. 

A `Getter<T>`/`Setter<T>` can get/set a specific property of type `T` from any object of a certain type.

### Usage

To use, simply call the `Build` method and pass in the root type.

```cs
public SpriteRenderer root;

private Getter<Color> _getter;
private Setter<Color> _setter;

void Awake()
{
	_getter = Getter.Build<Color>(typeof(SpriteRenderer), "color");
	_setter = Setter.Build<Color>(typeof(SpriteRenderer), "color");
}
```

Then, the `GetValue` and `SetValue` methods can be used to access the property on any object of the root type. 

(In this example, the root type is `SpriteRenderer`.)

```cs
void Start()
{
	var retrievedColor = _getter.GetValue(root);
	_setter.SetValue(root, Color.yellow);
}
```