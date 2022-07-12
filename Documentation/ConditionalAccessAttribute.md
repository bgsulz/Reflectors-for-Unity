# ConditionalAccessAttribute

`ConditionalAccessAttribute` is an attribute that allows an annotated serialized property to show or hide when certain conditions are fulfilled.

### Usage

To use, simply annotate a serialized property with an attribute that inherits from `ConditionalAccessAttribute`. These attribute types are listed in the table below.

```cs
public bool useCustomTitle;
[DisableIf("useCustomTitle")] public string defaultTitle;
[EnableIf("useCustomTitle")] public string overrideTitle;

public float detectionRadius;
private bool ShouldUseRaycast => detectionRadius > 5;
[ShowIf(shouldIndent: true, "ShouldUseRaycast")] public bool fullAngleDetection;
[ShowIf(shouldIndent: true, "ShouldUseRaycast")] public float raycastLength;
[HideIf(shouldIndent: true, "ShouldUseRaycast", "fullAngleDetection")] public float detectionAngle;
```

### Constructors

Each class that inherits from `ConditionalAccessAttribute` has two constructors. They are:

```cs
[AttributeName(params string[] propertyNames)]
[AttributeName(bool shouldIndent, params string[] propertyNames)]
```

#### Parameters

`bool shouldIndent`
> If true, the editor will indent by one additional level for this property.

> In the constructor without this parameter, its value is `false`.

`string[] propertyNames`
> A list of `bool` property paths dictating how the annotated property should be rendered.

These property paths must be relative to the class (e.g. if the class that contains the annotated member has a `bool` member `toggle`, the path is `"toggle"`.) Fields, properties, and parameter-less methods are supported.

```cs
public class MyBehaviour : MonoBehaviour
{
	public int num;
	
	public bool toggle0;
	private bool Toggle1 => num > 5;
	protected bool Toggle2() => num > 10;
	
	[ShowIfAll("toggle0", "Toggle1", "Toggle2")] public float annotated;
}
```

Nested properties separated with `.` are supported (e.g. if the class that contains the annotated member has a `MyClass` member `myObj` that has a `bool` member `toggle`, the path is `"myObj.toggle"`.)

```cs
public class MyBehaviour : MonoBehaviour
{
	[Serializable]
	public class MyClass
	{
		public bool toggle;
	}
	
	public MyClass myObj;
	
	[EnableIf("myObj.toggle")] public float annotated;
}
```


### Classes

Several attribute types inherit from the abstract base class `ConditionalAccessAttribute`.

| Class Name | Functionality |
| - | - |
| `ShowIf` | If any provided property is true, the annotated property is shown. Otherwise, it is hidden. |
| `ShowIfAll` | If all provided properties are true, the annotated property is shown. Otherwise, it is hidden. |
| `HideIf` | If any provided property is true, the annotated property is hidden. Otherwise, it is shown. |
| `HideIfAll` | If all provided properties are true, the annotated property is hidden. Otherwise, it is shown. |
| `EnableIf` | If any provided property is true, the annotated property is enabled. Otherwise, it is disabled. |
| `EnableIfAll` | If all provided properties are true, the annotated property is enabled. Otherwise, it is disabled. |
| `DisableIf` | If any provided property is true, the annotated property is disabled. Otherwise, it is enabled. |
| `DisableIfAll` | If all provided properties are true, the annotated property is disabled. Otherwise, it is enabled. |