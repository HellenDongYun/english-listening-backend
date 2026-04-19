using AutoMapper;
using Listening.Application.Dtos;
using Listening.Application.Exercise.Commands;
using Listening.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Listening.Controllers
{
    [Route("api/lessons")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        private readonly ILessonRepository _repo;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public LessonsController(
            ILessonRepository repo,
            IMapper mapper,
            IWebHostEnvironment environment)
        {
            _repo = repo;
            _mapper = mapper;
            _environment = environment;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LessonDto>>> GetAllLessons()
        {
            try
            {
                var lessons = await _repo.GetAllAsync();

                if (lessons == null || !lessons.Any())
                    return NotFound(new { message = "lessons not found" });

                return Ok(_mapper.Map<List<LessonDto>>(lessons));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("{lessonId}")]
        public async Task<ActionResult<LessonDto>> GetLesson([FromRoute] Guid lessonId)
        {
            var lesson = await _repo.GetByIdAsync(lessonId);

            if (lesson == null)
                return NotFound(new { message = "lesson not found" });

            return Ok(_mapper.Map<LessonDto>(lesson));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<LessonDto>> CreateLesson(
            [FromForm] CreateLessonRequest request,
            IFormFile? image)
        {
            string? imagePath = null;

            if (image != null && image.Length > 0)
            {
                imagePath = await SaveImageAsync(image);
            }

            var lesson = new Lesson(
                request.Title,
                request.Description,
                imagePath
            );

            await _repo.AddAsync(lesson);
            await _repo.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetLesson),
                new { lessonId = lesson.Id },
                _mapper.Map<LessonDto>(lesson)
            );
        }

        [HttpPut("{lessonId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateLesson(
            [FromRoute] Guid lessonId,
            [FromForm] UpdateLessonDto updateDto,
            IFormFile? image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existLesson = await _repo.GetByIdAsync(lessonId);
            if (existLesson == null)
                return NotFound(new { message = "lesson not found" });

            string? imagePath = existLesson.ImagePath;

            if (image != null && image.Length > 0)
            {
                // 可选：先删除旧图片
                DeleteImageIfExists(existLesson.ImagePath);

                imagePath = await SaveImageAsync(image);
            }

            existLesson.Update(
                updateDto.Title,
                updateDto.Description,
                imagePath
            );

            try
            {
                await _repo.UpdateAsync(existLesson);
                await _repo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while updating the database.",
                    Detail = ex.Message
                });
            }

            return NoContent();
        }

        [HttpDelete("{lessonId}")]
        public async Task<IActionResult> DeleteLesson([FromRoute] Guid lessonId)
        {
            try
            {
                var lesson = await _repo.GetByIdAsync(lessonId);
                if (lesson == null)
                    return NotFound(new { message = "lesson not found" });

                // 可选：删除本地图片
                DeleteImageIfExists(lesson.ImagePath);

                bool isDeleted = await _repo.DeleteAsync(lessonId);
                if (!isDeleted)
                    return NotFound(new { message = "lesson not found" });

                await _repo.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, "internal error, please try again later!");
            }
        }

        // ===== 保存图片 =====
        private async Task<string> SaveImageAsync(IFormFile image)
        {
            var webRootPath = _environment.WebRootPath;

            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            var uploadsFolder = Path.Combine(webRootPath, "uploads", "lessons");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var extension = Path.GetExtension(image.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(stream);

            return $"/uploads/lessons/{fileName}";
        }

        // ===== 删除旧图片（可选但推荐） =====
        private void DeleteImageIfExists(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return;

            var webRootPath = _environment.WebRootPath;

            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            var relativePath = imagePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
            var fullPath = Path.Combine(webRootPath, relativePath);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
}
