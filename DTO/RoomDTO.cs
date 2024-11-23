using testZugether1.Models;

namespace testZugether1.DTO
{
	public class RoomDTO
	{
		public short room_id { get; set; }

		public string room_title { get; set; } = null!;

		public string address { get; set; } = null!;

		public short rent { get; set; }

		public short? management_fee { get; set; }

		public byte floor { get; set; }

		public byte room_size { get; set; }

		public string room_type { get; set; } = null!;

		public string? bed_type { get; set; }

		public DateOnly? post_date { get; set; }

		public byte roommate_num { get; set; }

		public string? roommate_description { get; set; }

		public short? album_id { get; set; }

		public short? device_list_id { get; set; }

		public short? member_id { get; set; }

		public short? landlord_id { get; set; }

		public short? message_board_id { get; set; }

		public virtual ICollection<Favorites> Favorites { get; set; } = new List<Favorites>();

		public virtual Album? album { get; set; }

		public virtual Device_List? device_list { get; set; }

		public virtual Landlord? landlord { get; set; }

		public virtual Member? member { get; set; }
	}
}
