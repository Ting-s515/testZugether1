using System;
using System.Collections.Generic;

namespace testZugether1.Models;

public partial class Album
{
    public short album_id { get; set; }

    public virtual ICollection<Photo> Photo { get; set; } = new List<Photo>();
}
