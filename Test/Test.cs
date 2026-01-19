public unsafe struct ShouldExist<T>
{
  public int x;
  public int* y;
}


public struct ShouldNotExist_HasT<T>
{
  public int x;
  public T y;
}

public struct ShouldNotExist_HasGeneric
{
  public ShouldExist<int> x;
}

public struct ShouldExist_Nullable
{
  public int? x;
}

public struct ShouldNotExist_NullableHasGeneric
{
  public ShouldExist<int>? x;
}