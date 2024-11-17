using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testZugether1.DTO;
using testZugether1.Models;

namespace testZugether1.Controllers
{
	public class MemberController : Controller
	{
		private readonly ZugetherContext _context;

		public MemberController(ZugetherContext context)
		{
			_context = context;
		}
		public IActionResult Notify()
		{
			return View();
		}

		public IActionResult EditPassword()
		{
			ViewBag.Email = "test123@gmail.com";
			return View();
		}

		public IActionResult EditInfo()
		{
			ViewBag.Name = "Allen";
			ViewBag.NickName = "Allen粉多錢";
			ViewBag.Birth = "1999-01-01";
			ViewBag.Gender = "男";
			ViewBag.Phone = "0912345678";
			ViewBag.Intro = "我是Allen";
			return View();
		}

		public IActionResult AddRoom()
		{
			ViewBag.isAdd = true;
			return View();
		}
		public IActionResult DeleteRoom()
		{
			ViewBag.isAdd = false;
			return View("AddRoom");
		}

		public IActionResult EditRoom()
		{
			return View();
		}
		//會員收藏
		public async Task<IActionResult> FavoriteRoom()
		{
			int? memberID = HttpContext.Session.GetInt32("FavoriteMemberID");
			IQueryable<Room> query = from x in _context.Favor_List
									 join y in _context.Favorites on x.favor_list_id equals y.favor_list_id
									 join z in _context.Room on y.room_id equals z.room_id
									 where x.member_id == memberID
									 select z;

			List<RoomViewModel> result = await query.Select(room => new RoomViewModel
			{
				Room = room,
				roomImages = (from x in _context.Room
							  where x.room_id == room.room_id
							  join y in _context.Photo on x.album_id equals y.album_id
							  select new RoomImages
							  {
								  room_photo = y.room_photo,
								  photo_type = y.photo_type
							  }).ToList()
			}).ToListAsync();

			// 判斷是否有收藏的房間
			if (result.Count <= 0)
			{
				ViewBag.Message = "目前尚無收藏項目";
				return View(new List<RoomViewModel>());
			}

			// 將查詢結果傳遞到 View 中顯示
			return View(result);
		}
		//送會員編號
		[HttpPost]
		public IActionResult FavoriteMemberID(short memberID)
		{
			HttpContext.Session.SetInt32("FavoriteMemberID", memberID);
			return Ok();
		}
		//ADD收藏
		[HttpPost]
		public async Task<IActionResult> FavoriteRoom(short roomID, short memberID)
		{
			if (roomID == 0)
			{
				return Json(new { state = false, message = "無效的房間ID" });
			}
			//Favor_List要先有對應的會員編號
			//建立Favorites物件
			//找出對應的favor_list_id
			short favorListID = await (from x in _context.Favor_List
									   where x.member_id == memberID
									   select x.favor_list_id
								).FirstOrDefaultAsync();
			Favorites favorite = new Favorites
			{
				room_id = roomID,
				favor_list_id = favorListID
			};
			try
			{
				await _context.Favorites.AddAsync(favorite);
				await _context.SaveChangesAsync();
				return Json(new { state = true, message = "收藏成功" });
			}
			catch (Exception ex)
			{
				return Json(new { state = false, message = "收藏失敗" + ex.InnerException?.Message });
			}
		}
		[HttpPost]
		public async Task<IActionResult> RemoveFavoriteRoom(short roomID, short memberID)
		{
			Favorites? favorite = await (from x in _context.Favor_List
										 where x.member_id == memberID
										 join y in _context.Favorites on x.favor_list_id equals y.favor_list_id
										 where y.room_id == roomID
										 select y).FirstOrDefaultAsync();
			try
			{
				_context.Favorites.Remove(favorite);
				await _context.SaveChangesAsync();
				return Json(new { state = true, message = "成功刪除收藏" });
			}
			catch (Exception ex)
			{
				return Json(new { state = false, message = "刪除失敗" + ex.InnerException?.Message });
			}
		}
		//Alert動畫
		public IActionResult Alert(string color, string alertText, bool show, int time)
		{
			PartialAlert model = new PartialAlert
			{
				Color = color,
				AlertText = alertText,
				Show = show,
				Time = time
			};
			return PartialView("_PartialAlert", model);
		}

	}
}
