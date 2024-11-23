using System;
using System.Collections.Generic;

namespace testZugether1.Models;

public partial class Photo
{
    public int photo_id { get; set; }

    public byte[] room_photo { get; set; } = null!;

    public short album_id { get; set; }

    public string photo_type { get; set; } = null!;

    public virtual Album album { get; set; } = null!;
}
