# PropertyReference\<T\>

`PropertyReference<T>` is a serializable class that allows for dynamically referencing and accessing a specific field, property, or parameter-less method by its path.

### Usage

To use, simply create a serialized property of type `PropertyReference<T>` and call its `Initialize()` method.


```cs
public PropertyReference<Color> colorProperty;

void Start()
{
    colorProperty.Initialize();
}
```

In the inspector, drag in an object that inherits from `UnityEngine.Object` and select a property path.

Then, the selected property can be accessed with the `Value` property.

```cs
void Update()
{
	GetComponent<SpriteRenderer>().color = colorProperty.Value;
}
```

### Methods

`void Initialize(bool? useExpression = null)`
> Initializes the PropertyReference, building a `Getter` for the property selected in the Inspector.

This method _must_ be called before accessing the `Value` property. It is suggested to call it in `Awake`, `Start`, or `OnEnable`.

#### Parameters

`bool? useExpression`
> Optionally overrides how the `Getter` is built.

If true, the `Getter` is built as an `Expression`. If false, the `Getter` is built with reflection. If null, the `Getter` is built as an `Expression` only when supported by the platform. iOS, Android, and WebGL do not support compiling `Expression`s.

Note that if an unsupported platform builds the `Getter` as an `Expression`, it will use an interpreter in lieu of compiling the `Expression`. This is often even slower than reflection. It is usually best to leave this parameter as `null`, its default value.

### Properties

`T Value`
> Returns the value of the property path selected in the Inspector.

# SearchDepth

This attribute can be optionally added to a `PropertyReference<T>` to specify how many "layers" of nested properties should be scanned. If this attribute is absent, the property drawer scans 1 level deep. A value of 0 indicates that only the root object will be searched.

### Constructors

```cs
SearchDepthAttribute(int searchDepth)
```

#### Parameters

`int searchDepth`
> How many "layers" of nested properties the property drawer should search.

If left empty, the default value of this parameter is 1. This is to enhance performance, as nested properties may increase in quantity exponentially per layer. This attribute should _only_ be added in order to select a specific deeply nested property.