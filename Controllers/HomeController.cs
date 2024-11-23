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
		//�Q�Ϊ��
		[HttpPost]
		public IActionResult ContactSubmit()
		{
			ViewBag.show = true;
			return View("Contact");
		}
		//���դW�ǹϤ�
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
				ViewBag.loginMessage = "�b���K�X���~";
				return View("Register");
			}

		}
		//�W�ǩж��Ϥ�
		[HttpPost]
		public async Task<IActionResult> UploadRoomImages(short roomID, List<IFormFile> ImageFiles)
		{
			if (ImageFiles != null && ImageFiles.Any())
			{

				foreach (var file in ImageFiles)
				{
					if (file.Length > 0)
					{
						//�N MemoryStream ����b�j�餺�A�C���ɮ׳��|�ϥΤ@�ӷs��memoryStream��ҡA�קK��ƲV�c�C
						using (var memoryStream = new MemoryStream())
						{
							await file.CopyToAsync(memoryStream);
							//���o�G�i��Ϥ�
							byte[] imageBytes = memoryStream.ToArray();
							//���o�Ϥ�MIME����
							string photoType = GetMimeTypeFromImage(imageBytes);
							//�Ωж���������ï�s��
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
				//foreach����
				await _context.SaveChangesAsync();
			}
			return RedirectToAction("Index");
		}
		//��X�Ϥ��榡�s���Ʈw
		public string GetMimeTypeFromImage(byte[] imageBytes)
		{
			IImageFormat format = Image.DetectFormat(imageBytes);  // �R�A��k�ե�
			if (format != null)
			{
				return format.DefaultMimeType;
			}
			return "unknown";  // �p�G�L�k�˴���榡
		}
		//�W�Ƿ|���Ϥ�
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
							// ���o�G�i��Ϥ�
							byte[] imageBytes = memoryStream.ToArray();
							// ��s�|���� avatar
							member.avatar = imageBytes;
						}
						await _context.SaveChangesAsync(); // �O�s���
					}
				}
				else
				{
					// �B�z�|�����s�b�����p
					return NotFound("�|�����s�b");
				}
			}
			return RedirectToAction("Index");
		}
		/* ���U */
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
		//���U�|��
		[HttpPost]
		public async Task<IActionResult> Register(Member member)
		{
			Member? query = await (from x in _context.Member
								   where x.email == member.email
								   select x).FirstOrDefaultAsync();
			//�p�G�����
			if (query != null)
			{
				return Json(new { state = false, message = "���b���w�g���U�L!!" });
			}
			try
			{
				switch (member.gender)
				{
					case "male":
						member.gender = "�k";
						break;
					default:
						member.gender = "�k";
						break;
				}
				//string �M�ޥΫ��O���w�]�Ȭ� null
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

				return Json(new { state = true, message = "���U���\�A�Э��s�n�J!!" });
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
			// ���token
			var user = HttpContext.User;
			// ����������T
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
		//Alert�ʵe
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
