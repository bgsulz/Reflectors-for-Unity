# Getter\<TReturn\>

`Getter<TReturn>` is a wrapper class for `Func<TReturn>` that makes it easy to compile property accessors at runtime. An object of type `Getter<TReturn>` can get a specific property of type `TReturn` from any object of a certain type. 

### Usage

To use, simply call `Getter.Build<TReturn>`. This method is documented below.

```cs
public SpriteRenderer root;

void Start()
{
	var getter = Getter.Build<Color>(typeof(SpriteRenderer), "color");
}
```

Then, the `GetValue` method can be used to access the property on any object of the root type. (In this example, the root type is `SpriteRenderer`.)

```cs
void Update()
{
	GetComponent<SpriteRenderer>().color = getter.GetValue(root);
}
```

### Methods

```cs
TReturn GetValue(object root);
```

#### Returns
> The value of the `Getter`'s property for the passed-in root object.

This method is only present on `Getter<TRoot, TReturn>` objects.

#### Parameters
`TRoot root`
> The root object from which to get the property.

```cs
TReturn GetValue(TRoot root);
```

#### Returns
> The value of the `Getter`'s property for the passed-in root object.

#### Parameters
`object root`
> The root object from which to get the property.

# PropertyGetter\<TReturn\>

`PropertyGetter<TReturn>` is a convenience wrapper for `Getter<TReturn>` for compiling an accessor to a specific property of a specific object. An object of type `PropertyGetter<TReturn>` can get a specific property of type `TReturn` from a _specific_ object of a certain type.

### Usage

To use, simply call `Getter.BuildPropertyGetter<TReturn>`. This method is documented below. Then, the `Value` property can be used to access the property on the root object passed in during initialization.

```cs
public SpriteRenderer root;

void Start()
{
	var getter = Getter.BuildPropertyGetter<Color>(root, "color");
}

void Update()
{
	GetComponent<SpriteRenderer>().color = getter.Value;
}
```

### Properties

```cs
TReturn Value
```
> The value of the `PropertyGetter`'s property for the root object passed in during its initialization.

# Getter

`Getter` is a static helper class that makes it easy to create `Getter<TReturn>` objects.


### Static Methods

```cs
Getter<TReturn> Build<TProperty>(Type rootType, string propertyPath, BindingFlags bindingFlags, bool? useExpression);
Getter<TRoot, TReturn> Build<TRootType, TPropertyType>(string propertyPath, BindingFlags bindingFlags, bool? useExpression);
```

#### Returns

`Getter<TReturn>` / `Getter<TRoot, TReturn>`
> The generated getter.

#### Type Parameters

`TRoot`
> The type of the object that contains the property to get.

In other words, when building a `Getter` for the `Color color` property of an object of type `SpriteRenderer`, `TReturn` should be `Color`.

`TReturn`
> The type of the property to get.

In other words, when building a `Getter` for the `Color color` property of an object of type `SpriteRenderer`, `TRoot` should be `SpriteRenderer`.

#### Parameters

`Type rootType`
> The type of the object that contains the property to get.

See `TRoot` above for more details.

`string propertyPath`
> The property path of the property to get.

Nested properties separated with `.` are supported (e.g. for an object `myObj` that has a member `myField`, the path is `"myObj.myField"`.)

`BindingFlags bindingFlags`
> When initializing the `Getter`, these `BindingFlags` are used to find properties via reflection.

This parameter defaults to `BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public`.

`bool? useExpression`
> Optionally overrides if `Getter` is built as an `Expression` or not.

If true, the `Getter` is built as an `Expression`. If false, the `Getter` is built with reflection. If null, the `Getter` is built as an `Expression` only when supported by the platform. iOS, Android, and WebGL do not support compiling `Expression`s.

Note that if an unsupported platform builds the `Getter` as an `Expression`, it will use an interpreter in lieu of compiling the `Expression`. This is often even slower than reflection. It is usually best to leave this parameter as `null`, its default value.

```cs
bool TryBuild<TReturn>(Type rootType, string propertyPath, out Getter<TReturn> getter, BindingFlags bindingFlags, bool? useExpression);
bool TryBuild<TRoot, TReturn>(string propertyPath, out Getter<TRoot, TReturn> getter, BindingFlags bindingFlags, bool? useExpression);
```

These are wrapper methods for `Build<TReturn>` and `Build<TRoot, TReturn>`. Parameters shared between them are only described in brief. See above for more details.

#### Returns

`bool`
> True if the `Getter` was generated successfully; otherwise, false.

#### Type Parameters

`TRoot`
> The type of the object that contains the property to get.

`TReturn`
> The type of the property to get.

#### Parameters

`Type rootType`
> The type of the object that contains the property to get.

`out Getter<TReturn> getter` / `out Getter<TRoot, TReturn> getter`
> The generated getter. This should only be accessed if the method returns true.

`string propertyPath`
> The property path of the property to get.

`BindingFlags bindingFlags`
> When initializing the `Getter`, these `BindingFlags` are used to find properties via reflection.

`bool? useExpression`
> Optionally overrides if `Getter` is built as an `Expression` or not.
