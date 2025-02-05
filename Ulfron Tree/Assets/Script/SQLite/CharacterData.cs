using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CharacterData : IEquatable<CharacterData>
{
    public int id {  get; set; }
    public string CName { get; set; }
    public string Partner { get; set; }
    public string Children { get; set; }

    public bool Equals(CharacterData other)
    {
        return id == other.id &&
            CName == other.CName &&
            Partner == other.Partner &&
            Children == other.Children;
    }
}
