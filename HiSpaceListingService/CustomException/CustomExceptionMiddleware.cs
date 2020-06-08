using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

//using log4net;

namespace HiSpaceListingService.CustomException
{
	public class CustomExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		//private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public CustomExceptionMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				//await _next.Invoke(context);

				//First, get the incoming request
				var request = await FormatRequest(context.Request);
				//Log.Error("test msg...");
				//Copy a pointer to the original response body stream
				var originalBodyStream = context.Response.Body;

				//Create a new memory stream...
				using (var responseBody = new MemoryStream())
				{
					//...and use that for the temporary response body
					context.Response.Body = responseBody;

					//Continue down the Middleware pipeline, eventually returning to this class
					await _next(context);

					//Format the response from the server
					var response = await FormatResponse(context.Response);

					//TODO: Save log to chosen datastore

					//Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
					await responseBody.CopyToAsync(originalBodyStream);
				}
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex);
			}
		}

		private async Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			var response = context.Response;
			var customException = exception as BaseCustomException;
			var statusCode = (int)HttpStatusCode.InternalServerError;
			var message = "Unexpected error";
			var description = "Unexpected error";

			//if (null != customException)
			if (null != exception)
			{
				message = exception.Message;
				description = exception.Source;
				//statusCode = (int)statusCode;
			}

			response.ContentType = "application/json";
			response.StatusCode = statusCode;
			await response.WriteAsync(JsonConvert.SerializeObject(new CustomErrorResponse
			{
				Message = message,
				Description = description
			}));
		}

		private async Task<string> FormatRequest(HttpRequest request)
		{
			var body = request.Body;

			//This line allows us to set the reader for the request back at the beginning of its stream.
			//request.EnableRewind();

			//We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
			var buffer = new byte[Convert.ToInt32(request.ContentLength)];

			//...Then we copy the entire request stream into the new buffer.
			await request.Body.ReadAsync(buffer, 0, buffer.Length);

			//We convert the byte[] into a string using UTF8 encoding...
			var bodyAsText = Encoding.UTF8.GetString(buffer);

			//..and finally, assign the read body back to the request body, which is allowed because of EnableRewind()
			request.Body = body;

			return $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
		}

		private async Task<string> FormatResponse(HttpResponse response)
		{
			//We need to read the response stream from the beginning...
			response.Body.Seek(0, SeekOrigin.Begin);

			//...and copy it into a string
			string text = await new StreamReader(response.Body).ReadToEndAsync();

			//We need to reset the reader for the response so that the client can read it.
			response.Body.Seek(0, SeekOrigin.Begin);

			//Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
			return $"{response.StatusCode}: {text}";
		}
	}

	public class BaseCustomException : Exception
	{
		private int _code;
		private string _description;

		public int Code
		{
			get => _code;
		}

		public string Description
		{
			get => _description;
		}

		public BaseCustomException(string message, string description, int code) : base(message)
		{
			_code = code;
			_description = description;
		}
	}

	public class CustomErrorResponse
	{
		public string Message { get; set; }
		public string Description { get; set; }
	}

	public class NotFoundCustomException : BaseCustomException
	{
		public NotFoundCustomException(string message, string description) : base(message, description, (int)HttpStatusCode.NotFound)
		{
		}
	}
}