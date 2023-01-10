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
using MixyBoos.Api.Services.Helpers;
using OpenIddict.Validation.AspNetCore;
using Quartz;

namespace MixyBoos.Api.Controllers {
    public class UploadFormData {
        public string Id { get; set; }
    }

    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class UploadController : _Controller {
        private readonly UserManager<MixyBoosUser> _userManager;
        private readonly ISchedulerFactory _schedulerFactory;
        const long FileSizeLimit = 2147483648;

        public UploadController(UserManager<MixyBoosUser> userManager, ISchedulerFactory schedulerFactory,
            ILogger<UploadController> logger) : base(logger) {
            _userManager = userManager;
            _schedulerFactory = schedulerFactory;
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = FileSizeLimit)] //2Gb
        [RequestSizeLimit(FileSizeLimit)] //2Gb
        public async Task<IActionResult> UploadAudio() {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user is null) {
                return Unauthorized();
            }

            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType)) {
                ModelState.AddModelError("File",
                    $"The request couldn't be processed (Error 1).");
                // Log error

                return BadRequest(ModelState);
            }

            // Accumulate the form data key-value pairs in the request (formAccumulator).
            var formAccumulator = new KeyValueAccumulator();
            var streamedFileContent = Array.Empty<byte>();

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                2147483648);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            var section = await reader.ReadNextSectionAsync();

            while (section != null) {
                var hasContentDispositionHeader =
                    ContentDispositionHeaderValue.TryParse(
                        section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader) {
                    if (MultipartRequestHelper
                        .HasFileContentDisposition(contentDisposition)) {
                        streamedFileContent = await FileHelpers.ProcessStreamedFile(
                            section,
                            contentDisposition,
                            ModelState,
                            FileSizeLimit
                        );

                        if (!ModelState.IsValid) {
                            return BadRequest(ModelState);
                        }
                    } else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition)) {
                        // Don't limit the key name length because the 
                        // multipart headers length limit is already in effect.
                        var key = HeaderUtilities
                            .RemoveQuotes(contentDisposition.Name).Value;
                        var encoding = GetEncoding(section);

                        if (encoding == null) {
                            ModelState.AddModelError("File", $"The request couldn't be processed (Error 2).");
                            // Log error
                            return BadRequest(ModelState);
                        }

                        using var streamReader = new StreamReader(
                            section.Body,
                            encoding,
                            detectEncodingFromByteOrderMarks: true,
                            bufferSize: 1024,
                            leaveOpen: true);
                        // The value length limit is enforced by 
                        // MultipartBodyLengthLimit
                        var value = await streamReader.ReadToEndAsync();

                        if (string.Equals(value, "undefined",
                                StringComparison.OrdinalIgnoreCase)) {
                            value = string.Empty;
                        }

                        formAccumulator.Append(key, value);

                        if (formAccumulator.ValueCount > 1) {
                            ModelState.AddModelError("File",
                                $"The request couldn't be processed (Error 3).");
                            return BadRequest(ModelState);
                        }
                    }
                }

                // Drain any remaining section body that hasn't been consumed and
                // read the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            // Bind form data to the model
            var formData = new UploadFormData();
            var formValueProvider = new FormValueProvider(
                BindingSource.Form,
                new FormCollection(formAccumulator.GetResults()),
                CultureInfo.CurrentCulture);

            var bindingSuccessful = await TryUpdateModelAsync(
                formData,
                "",
                formValueProvider);

            if (!bindingSuccessful) {
                ModelState.AddModelError("File", "The request couldn't be processed (Error 5).");
                return BadRequest(ModelState);
            }

            var outputFile = $"/tmp/{formData.Id}.mp3";
            await System.IO.File.WriteAllBytesAsync(outputFile, streamedFileContent);


            var scheduler = await _schedulerFactory.GetScheduler();
            var jobData = new Dictionary<string, string>() {
                {"Id", formData.Id},
                {"FileLocation", outputFile},
                {"UserId", User.Identity.Name}
            };
            await scheduler.TriggerJob(
                new JobKey("ProcessUploadedAudioJob"),
                new JobDataMap(jobData));

            return Created(nameof(UploadController), null);
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
