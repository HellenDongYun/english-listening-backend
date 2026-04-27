using Listening.Application.Dtos;
using Listening.Application.Exercise;
using Listening.Application.Exercise.Commands;
using Listening.Domain.Entities;
using Listening.Infrastructure.Repositories;
using Listening.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CreateExerciseRequest = Listening.Request.CreateExerciseRequest;

namespace Listening.Controllers
{
    [Route("api/lessons/{lessonId}/exercises")]
    [ApiController]
    public class ExercisesController : ControllerBase
    {
        public readonly ExerciseService _exerciseService;
        public ExercisesController(ExerciseService exerciseService)
        {
            _exerciseService = exerciseService;

        }

        [HttpGet("{exerciseId}")]
        public async Task<IActionResult> GetById(Guid exerciseId)
        {
            var result = await _exerciseService.GetExerciseByIdAsync(exerciseId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        // POST api/<ExercisesController>
        [HttpPost]
        public async Task<IActionResult> Create(Guid lessonId, [FromForm] CreateExerciseRequest request)
        {
            if (request == null)
                return BadRequest("Request cannot be null.");

            if (request.AudioFile == null || request.AudioFile.Length == 0)
                return BadRequest("Audio file is required.");

            var command = new CreateExerciseCommand
            {
                LessonId = lessonId,
                Title = request.Title,
                AudioStream = request.AudioFile.OpenReadStream(),
                FileName = request.AudioFile.FileName,  // ✅ 自动获取
                ContentType = request.AudioFile.ContentType,  // ✅ 自动获取
                Length = request.AudioFile.Length,
                // Transcript = request.Transcript?.Trim(),
                Difficulty = request.Difficulty,
                SubtitleStream = request.SubtitleFile?.OpenReadStream(),
                SubtitleFileName = request.SubtitleFile?.FileName
            };

            var result = await _exerciseService.CreateExerciseAsync(command);

            return CreatedAtAction(
                nameof(GetById),
                new { lessonId = result.LessonId, exerciseId = result.Id },
                result
            );
        }
        [HttpDelete("{exerciseId}")]
        public async Task<IActionResult> Delete(Guid lessonId, Guid exerciseId)
        {
            var success = await _exerciseService.DeleteExerciseAsync(exerciseId);
            if (!success)
                return NotFound();
            return NoContent();
            
        }

        [HttpPut("{exerciseId}")]
        public async Task<ActionResult> UpdateExercise(
            Guid exerciseId,
            [FromForm] UpdateExerciseRequest request)
        {
            var cmd = new UpdateExerciseCommand
            {
                Id = exerciseId,
                Title = request.Title,

                // 音频
                NewAudioStream = request.AudioFile?.OpenReadStream(),
                FileName = request.AudioFile?.FileName,
                ContentType = request.AudioFile?.ContentType,
                Length = request.AudioFile?.Length ?? 0,

                // 新增字幕文件
                SubtitleStream = request.SubtitleFile?.OpenReadStream(),
                SubtitleFileName = request.SubtitleFile?.FileName
            };

            await _exerciseService.UpdateExerciseAsync(cmd);

            return NoContent();
        }
    }
}
