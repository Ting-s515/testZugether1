using System;
using System.Collections.Generic;

namespace testZugether1.Models;

public partial class Message_Board
{
    public short message_board_id { get; set; }

    public short? message_count { get; set; }

    public short room_id { get; set; }

    public virtual ICollection<Message> Message { get; set; } = new List<Message>();

    public virtual Room room { get; set; } = null!;
}
