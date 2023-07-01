using Starwars.YodaQuotes.Entities;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var yodaQuotes = new[]
{
    "No! Try not. Do. Or do not. There is no try.",
    "Fear is the path to the dark side. Fear leads to anger. Anger leads to hate. Hate leads to suffering.",
    "Patience you must have, my young Padawan.",
    "A Jedi uses the Force for knowledge and defense, never for attack.",
    "A Jedi’s strength flows from the Force.",
    "Feel the Force!"
};

app.UseExceptionHandler(exceptionHandlerApp
    => exceptionHandlerApp.Run(async context => await Results.Problem().ExecuteAsync(context)));

app.UseStatusCodePages();

app.MapGet("/api/yoda/quote", () =>
{
    return new YodaQuote(yodaQuotes[Random.Shared.Next(yodaQuotes.Length)]);
})
.WithName("GetYodaQuote")
.WithOpenApi();


int yodaQuoteAttemptIndex = 0;

app.MapGet("/api/yoda/quote/witherror/each/{attemptIndex:int}", (int attemptIndex) =>
{
    yodaQuoteAttemptIndex++;

    if (yodaQuoteAttemptIndex % attemptIndex != 0)
    {
        return new YodaQuote(yodaQuotes[Random.Shared.Next(yodaQuotes.Length)]);
    }

    throw new InvalidOperationException("Yoda is not avaliable");

    //throw new InvalidOperationException("Yoda is not avaliable");
    //Results.UnprocessableEntity
    //Results.Problem("Yoda is not avaliable");
    //return StatusCode((int)HttpStatusCode.InternalServerError, "Yoda is not avaliable");

})
.WithName("GetYodaQuoteWithError")
.WithOpenApi();


app.Run();

