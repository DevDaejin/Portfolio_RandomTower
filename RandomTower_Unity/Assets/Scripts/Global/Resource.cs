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
        Invoke();
    }

    public bool Spend(int amount)
    {
        if (_value - amount >= 0)
        {
            _value -= amount;
            Invoke();
            return true;
        }

        return false;
    }

    public void Invoke()
    {
        ValueChanged?.Invoke(_value);
    }
}
