using System;

public class CharacterDataNew : IEquatable<CharacterDataNew>
{
    public int id {  get; set; }
    public string CName { get; set; }

    public bool Equals(CharacterDataNew other)
    {
        return id == other.id &&
            CName == other.CName;
    }

    public override string ToString()
    {
        return $"ID : {id}\nName : {CName}";
    }
}
