public struct Test1<T>
{
  public int x;
}


public struct Test2<T>
{
  public int x;
  public T y;
}

public struct Test2
{
  public Test1<int> x;
}