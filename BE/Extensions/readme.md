# Extension trong CSharp

Đây là 1 cách tuyệt vời để mở rộng class mà không cần kế thừa class đó
Eg:

## Với Generic<T>: any

```cs
public static partial class Extensions
{
  public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
  {
    foreach (var item in source)
    {
    action(item);
    }
  }
}
```

điều này khiến ``IEnumerable<int>`` có thể sử dụng ForEach mà không cần tạo 1 class kế thừa IEnumerable

## Với Generic<struct> [int,string,bool,float,v.v.] hoặc class

```cs
public static partial class Extensions
{
  public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) where T : struct,class
  {
    foreach (var item in source)
    {
    action(item);
    }
  }
}
```

## với class, struct nhất định

```cs
public static partial class Extensions
{
  public static void ForEach<int>(this IEnumerable<int> source, Action<int> action)
  {
    foreach (var item in source)
    {
    action(item);
    }
  }
}
```
