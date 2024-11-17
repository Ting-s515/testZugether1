using System;
using System.Collections.Generic;

namespace testZugether1.Models;

public partial class Favorites
{
    public short favorites_id { get; set; }

    public short favor_list_id { get; set; }

    public short room_id { get; set; }

    public virtual Favor_List favor_list { get; set; } = null!;

    public virtual Room room { get; set; } = null!;
}
