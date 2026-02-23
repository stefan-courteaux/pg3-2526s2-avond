using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ShipIt.PriceQuote.Api.Controllers;

[ApiController]
public class ErrorController : ControllerBase
{
	[Route("/error-development")]
	public IActionResult HandleErrorDevelopment([FromServices] IHostEnvironment hostEnvironment)
	{
		if (!hostEnvironment.IsDevelopment())
		{
			return NotFound();
		}

		var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;

		return Problem(
			detail: exceptionHandlerFeature.Error.StackTrace,
			title: exceptionHandlerFeature.Error.Message
		);
	}

	[Route("/error")]
	public IActionResult HandleError()
	{
		var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;
		return Problem(title: exceptionHandlerFeature.Error.Message);
	}
}