using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using MixyBoos.Api.Data;
using MixyBoos.Api.Services;
using MixyBoos.Api.Services.Extensions;
using MixyBoos.Api.Services.Helpers;
using OpenIddict.Validation.AspNetCore;
using Quartz;

public class InvalidFileUploadException : Exception {
    public InvalidFileUploadException(string message) : base(message) { }
}

namespace MixyBoos.Api.Controllers {
    public class FileUploadModel {
        public string Id { get; set; }
        public IFormFile File { get; set; }
    }

    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class UploadController : _Controller {
        private readonly UserManager<MixyBoosUser> _userManager;
        private readonly ISchedulerFactory _schedulerFactory;
        const long AudioFileSizeLimit = 2147483648;
        const long ImageFileSizeLimit = 52428800;

        public UploadController(UserManager<MixyBoosUser> userManager, ISchedulerFactory schedulerFactory,
            ILogger<UploadController> logger) : base(logger) {
            _userManager = userManager;
            _schedulerFactory = schedulerFactory;
        }

        private async Task<(IActionResult, string)> _preProcessUpload(string id, IFormFile file) {
            if (!ModelState.IsValid || id is null || file is null) {
                return (BadRequest(), string.Empty);
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user is null) {
                return (Unauthorized(), string.Empty);
            }

            var fileName = file.FileName;
            var extension = Path.GetExtension(fileName);
            var localPath = Path.Combine(Constants.TempFolder, $"{id}{extension}");

            await using (var stream = new FileStream(localPath, FileMode.Create)) {
                await file.CopyToAsync(stream);
            }

            return (Created(nameof(UploadController), null), localPath);
        }

        [HttpPost("image/{id}")]
        [RequestFormLimits(MultipartBodyLengthLimit = AudioFileSizeLimit)] //2Gb
        [RequestSizeLimit(AudioFileSizeLimit)] //2Gb
        [DisableFormValueModelBinding]
        public async Task<IActionResult> UploadImage([FromRoute] string id, [FromForm] IFormFile file,
            [FromQuery] string imageSource, [FromQuery] string imageType) {
            var (response, localFile) = await _preProcessUpload(id, file);

            if (string.IsNullOrEmpty(localFile)) {
                return response;
            }

            var jobData = new Dictionary<string, string>() {
                {"Id", id},
                {"FileLocation", localFile},
                {"ImageSource", imageSource},
                {"ImageType", imageType},
                {"UserId", User.Identity.Name}
            };
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.TriggerJob(
                new JobKey("ProcessUploadedImageJob"),
                new JobDataMap(jobData));

            return response;
        }


        [HttpPost("{id}")]
        [RequestFormLimits(MultipartBodyLengthLimit = AudioFileSizeLimit)] //2Gb
        [RequestSizeLimit(AudioFileSizeLimit)] //2Gb
        [DisableFormValueModelBinding]
        public async Task<IActionResult> UploadAudio([FromRoute] string id, [FromForm] IFormFile file) {
            var (response, localFile) = await _preProcessUpload(id, file);

            if (string.IsNullOrEmpty(localFile)) {
                return response;
            }

            var jobData = new Dictionary<string, string>() {
                {"Id", id},
                {"FileLocation", localFile},
                {"UserId", User.Identity.Name}
            };
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.TriggerJob(
                new JobKey("ProcessUploadedAudioJob"),
                new JobDataMap(jobData));

            return response;
        }

        private static Encoding GetEncoding(MultipartSection section) {
            var hasMediaTypeHeader =
                MediaTypeHeaderValue.TryParse(section.ContentType, out var mediaType);

            // UTF-7 is insecure and shouldn't be honored. UTF-8 succeeds in 
            // most cases.
#pragma warning disable SYSLIB0001
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding)) {
#pragma warning restore SYSLIB0001
                return Encoding.UTF8;
            }

            return mediaType.Encoding;
        }
    }
}
