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
        public async Task<IActionResult> Create(Guid lessonId,[FromForm] CreateExerciseRequest request)
        {
            var command = new CreateExerciseCommand
            {
                LessonId = lessonId,
                Title = request.Title,
                Transcript = request.Transcript,
                AudioStream = request.AudioFile.OpenReadStream(),
                FileName = request.AudioFile.FileName,
                ContentType = request.AudioFile.ContentType,
                Length = request.AudioFile.Length
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
        public async Task<ActionResult> UpdateExercise(Guid exerciseId,[FromForm] UpdateExerciseRequest request )
        {
            var cmd = new UpdateExerciseCommand
            {
                Id = exerciseId,
                Title = request.Title,
                Transcript = request.Transcript,
                // 将 Web 专有对象转为 Stream
                NewAudioStream = request.AudioFile?.OpenReadStream(),
                FileName = request.AudioFile?.FileName,
                ContentType = request.AudioFile?.ContentType
            };
            await _exerciseService.UpdateExerciseAsync(cmd);
            return NoContent();
        }

    }
}
