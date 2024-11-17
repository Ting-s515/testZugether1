using System;
using System.Collections.Generic;

namespace testZugether1.Models;

public partial class Landlord
{
    public short landlord_id { get; set; }

    public string? name { get; set; }

    public string? phone { get; set; }

    public string? gender { get; set; }

    public virtual ICollection<Room> Room { get; set; } = new List<Room>();
}
