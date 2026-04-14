using AutoMapper;
using Listening.Application.Dtos;
using Listening.Application.Exercise.Commands;
using Listening.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Listening.Controllers
{
    [Route("api/lessons")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        private readonly ILessonRepository _repo;
        private readonly IMapper _mapper;

        public LessonsController(ILessonRepository repo,IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lesson>>> GetAllLessons()
        {
            try
            {
                var lessons = await _repo.GetAllAsync();
                // 检查结果是否为空
                if (lessons == null || !lessons.Any()) return NotFound(new { message = "lessons not found" });
                return Ok(_mapper.Map<List<LessonDto>>(lessons));

            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            
        }

        [HttpGet("{lessonId}")]
        public async Task<ActionResult<Lesson>> GetLesson([FromRoute]Guid lessonId)
        {
            var lesson = await _repo.GetByIdAsync(lessonId);
            if (lesson == null) return NotFound(new { message = "lesson not found" });
            return Ok(_mapper.Map<LessonDto>(lesson) );
        }

        [HttpPost]
        public async Task<ActionResult<LessonDto>> CreateLesson([FromBody] CreateLessonRequest request)
        {
            var lesson = new Lesson(request.Title, request.Description);

            await _repo.AddAsync(lesson);
            await _repo.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetLesson),
                new { lessonId = lesson.Id },
                _mapper.Map<LessonDto>(lesson)
            );
        }

        [HttpPut("{lessonId}")]
        public async Task<IActionResult> UpdateLesson(Guid lessonId, [FromBody]UpdateLessonDto updateDto)
        {
            // 1. 数据模型验证 (由 [ApiController] 自动处理，但手动写出逻辑如下)
            if(!ModelState.IsValid) return BadRequest(ModelState);
            // 2. 先从数据库查找现有的原始实体
            // 这样可以确保：a. 资源存在；b. 不会覆盖掉 DTO 中没有的字段（如 CreateTime）
            var existLesson = await _repo.GetByIdAsync(lessonId);
            if (existLesson == null) return NotFound(new { message = "lesson not found" });
            existLesson.Update(updateDto.Title, updateDto.Description);
            try
            {
                // 4. 执行更新逻辑
                await _repo.UpdateAsync(existLesson);
            }
            catch (Exception ex)
            {
                // 记录日志并返回 500
                return StatusCode(500, new { Message = "An error occurred while updating the database.", Detail = ex.Message });
            }

            // 5. 按照 RESTful 规范，成功更新后返回 204 NoContent 或返回更新后的对象
            return NoContent(); 
            
        }

        [HttpDelete("{lessonId}")]
        public async Task<IActionResult> DeleteLesson(Guid lessonId)
        {
            try
            {
                bool isDeleted = await _repo.DeleteAsync(lessonId);
                if (!isDeleted) return NotFound(new { message = "lesson not found" });
                return NoContent();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, "internal error, please try again later!");
            }
        } 
        
    }
}
