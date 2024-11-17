using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using System.Diagnostics;
using System.Security.Claims;
using testZugether1.Models;
namespace testZugether1.Controllers
{
	public class HomeController : Controller
	{
		private readonly ZugetherContext _context;

		public HomeController(ZugetherContext context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
			ViewBag.memberName = HttpContext.Session.GetString("memberName") ?? HttpContext.Session.GetString("googleName") ?? "";
			ViewBag.memberID = HttpContext.Session.GetString("memberID") ?? HttpContext.Session.GetString("googleMemberID") ?? "";
			ViewBag.memberEmail = HttpContext.Session.GetString("memberEmail") ?? HttpContext.Session.GetString("googleEmail") ?? "";
			if (!string.IsNullOrEmpty(ViewBag.memberName) && !string.IsNullOrEmpty(ViewBag.memberID) && !string.IsNullOrEmpty(ViewBag.memberEmail))
			{
				HttpContext.Session.SetString("isLogin", "true");
			}
			else
			{
				HttpContext.Session.SetString("isLogin", "false");
			}
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}
		public IActionResult About()
		{

			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
		public IActionResult Contact()
		{
			ViewBag.show = false;
			return View();
		}
		//利用表單
		[HttpPost]
		public IActionResult ContactSubmit()
		{
			ViewBag.show = true;
			return View("Contact");
		}
		//測試上傳圖片
		public IActionResult testImages()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Login(string email)
		{
			Member? query = await _context.Member.FirstOrDefaultAsync(x => x.email == email);
			if (query != null)
			{
				HttpContext.Session.SetString("memberID", query.member_id.ToString());
				HttpContext.Session.SetString("memberName", query.name.ToString());
				HttpContext.Session.SetString("memberEmail", query.email.ToString());
				//ViewBag.memberName = HttpContext.Session.GetString("memberName");
				//ViewBag.memberID = HttpContext.Session.GetString("memberID");
				//ViewBag.memberEmail = HttpContext.Session.GetString("memberEmail");
				return RedirectToAction("Index", "Home");
			}
			else
			{
				ViewBag.loginMessage = "帳號密碼錯誤";
				return View("Register");
			}

		}
		//上傳房間圖片
		[HttpPost]
		public async Task<IActionResult> UploadRoomImages(short roomID, List<IFormFile> ImageFiles)
		{
			if (ImageFiles != null && ImageFiles.Any())
			{

				foreach (var file in ImageFiles)
				{
					if (file.Length > 0)
					{
						//將 MemoryStream 限制在迴圈內，每個檔案都會使用一個新的memoryStream實例，避免資料混淆。
						using (var memoryStream = new MemoryStream())
						{
							await file.CopyToAsync(memoryStream);
							//取得二進制圖片
							byte[] imageBytes = memoryStream.ToArray();
							//取得圖片MIME類型
							string photoType = GetMimeTypeFromImage(imageBytes);
							//用房間找到對應相簿編號
							short? albumID = await _context.Room.Where(x => x.room_id == roomID).Select(x => x.album_id).FirstOrDefaultAsync();
							Photo newPhoto = new Photo
							{
								album_id = (short)albumID,
								room_photo = imageBytes,
								photo_type = photoType
							};
							_context.Photo.Add(newPhoto);
						}
					}
				}
				//foreach結束
				await _context.SaveChangesAsync();
			}
			return RedirectToAction("Index");
		}
		//抓出圖片格式存到資料庫
		public string GetMimeTypeFromImage(byte[] imageBytes)
		{
			IImageFormat format = Image.DetectFormat(imageBytes);  // 靜態方法調用
			if (format != null)
			{
				return format.DefaultMimeType;
			}
			return "unknown";  // 如果無法檢測到格式
		}
		//上傳會員圖片
		[HttpPost]
		public async Task<IActionResult> UploadMemberImages(short memberID, List<IFormFile> ImageFiles)
		{
			if (ImageFiles != null && ImageFiles.Any())
			{
				Member? member = await _context.Member.FirstOrDefaultAsync(x => x.member_id == memberID);

				if (member != null)
				{
					var file = ImageFiles.FirstOrDefault();
					if (file != null && file.Length > 0)
					{
						using (var memoryStream = new MemoryStream())
						{
							await file.CopyToAsync(memoryStream);
							// 取得二進制圖片
							byte[] imageBytes = memoryStream.ToArray();
							// 更新會員的 avatar
							member.avatar = imageBytes;
						}
						await _context.SaveChangesAsync(); // 保存更改
					}
				}
				else
				{
					// 處理會員不存在的情況
					return NotFound("會員不存在");
				}
			}
			return RedirectToAction("Index");
		}
		/* 註冊 */
		public IActionResult Register()
		{
			string? googleName = HttpContext.Session.GetString("googleName");
			string? googleEmail = HttpContext.Session.GetString("googleEmail");

			if (!string.IsNullOrEmpty(googleName) && !string.IsNullOrEmpty(googleEmail))
			{
				ViewBag.googleName = googleName;
				ViewBag.googleEmail = googleEmail;
				ViewBag.googleLogin = true;
			}
			ViewBag.post = false;
			return View();
		}
		//註冊會員
		[HttpPost]
		public async Task<IActionResult> Register(Member member)
		{
			Member? query = await (from x in _context.Member
								   where x.email == member.email
								   select x).FirstOrDefaultAsync();
			//如果有資料
			if (query != null)
			{
				return Json(new { state = false, message = "此帳號已經註冊過!!" });
			}
			try
			{
				switch (member.gender)
				{
					case "male":
						member.gender = "男";
						break;
					default:
						member.gender = "女";
						break;
				}
				//string 和引用型別的預設值為 null
				Member newMember = new Member
				{
					email = member.email,
					password = member.password,
					name = member.name,
					birthday = member.birthday,
					gender = member.gender,
					phone = member.phone,
					introduce = member.introduce
				};
				_context.Member.Add(newMember);
				await _context.SaveChangesAsync();

				return Json(new { state = true, message = "註冊成功，請重新登入!!" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					errorMessage = ex.Message,
					innerException = ex.InnerException?.Message,
					stackTrace = ex.StackTrace
				});
			}

		}
		[HttpPost]
		public IActionResult RegisterSubmit()
		{
			ViewBag.post = true;
			return View("Register");
		}
		public IActionResult GoogleLogin()
		{
			string? redirectUrl = Url.Action("GoogleResponse", "Home");
			return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, "Google");
		}
		public async Task<IActionResult> GoogleResponse()
		{
			// 獲取token
			var user = HttpContext.User;
			// 提取相關資訊
			string? googleName = user.FindFirst(c => c.Type == ClaimTypes.Name)?.Value ?? "no name";
			string? googleEmail = user.FindFirst(c => c.Type == ClaimTypes.Email)?.Value ?? "no email";
			Member? query = await _context.Member.FirstOrDefaultAsync(x => x.email == googleEmail.Trim());
			if (query != null)
			{
				HttpContext.Session.SetString("googleName", query.name);
				HttpContext.Session.SetString("googleEmail", query.email);
				HttpContext.Session.SetString("googleMemberID", query.member_id.ToString());
				return RedirectToAction("Index");
			}
			HttpContext.Session.SetString("googleName", googleName);
			HttpContext.Session.SetString("googleEmail", googleEmail);
			return RedirectToAction("Register", "Home");
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
