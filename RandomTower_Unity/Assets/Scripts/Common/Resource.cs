using System;

public class Resource
{
    private int _value;
    public event Action<int> ValueChanged;

    public Resource(int value)
    {
        _value = value;
    }

    public int Get()
    {
        return _value;
    }

    public void Earn(int amount)
    {
        _value += amount;
        ValueChanged?.Invoke(_value);
    }

    public bool Spend(int amount)
    {
        if(_value - amount >= 0)
        {
            _value -= amount;
            ValueChanged?.Invoke(_value);
            return true;
        }

        return false;
    }
}
