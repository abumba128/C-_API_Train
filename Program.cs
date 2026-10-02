using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Project_1.data;
using Project_1.Models;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();

var databasePath = Path.Combine(
    AppContext.BaseDirectory,
    "voting.db"
);

builder.Services.AddDbContext<VotingContext>(options =>
    options.UseSqlite($"Data Source={databasePath}"));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddOpenApi();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI();


app.UseForwardedHeaders();
app.UseCors();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VotingContext>();
    db.Database.Migrate();
}

app.UseDefaultFiles(); 
app.UseStaticFiles();

app.MapGet("/api/candidates", async (VotingContext context) =>
{
    var candidates = await context.Candidates.ToListAsync();  
    return Results.Ok(candidates);
}).WithName("Список всех кандидатов");

app.MapPost("/api/vote/{candidateId}", async (
    int candidateId,
    VotingContext context,
    HttpContext httpContext) =>
{
    var candidate = await context.Candidates.FindAsync(candidateId);

    if (candidate == null)
    {
        return Results.NotFound(new
        {
            message = "Кандидат не найден"
        });
    }
    var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();

    if (string.IsNullOrEmpty(ipAddress))
    {
        return Results.BadRequest(new
        {
            message = "Не удалось определить IP"
        });
    }

    var today = DateTime.Now.Date;
    var tomorrow = today.AddDays(1);

    var alreadyVoted = await context.Votes.AnyAsync(v =>
        v.IpAddress == ipAddress &&
        v.VotedAt >= today &&
        v.VotedAt < tomorrow);

    if (alreadyVoted)
    {
        return Results.BadRequest(new
        {
            message = "Вы уже сегодня голосовали"
        });
    }

    candidate.VoteCount++;

    var vote = new Vote
    {
        IpAddress = ipAddress,
        CandidateId = candidateId,
        VotedAt = DateTime.Now
    };

    context.Votes.Add(vote);

    await context.SaveChangesAsync();

    return Results.Ok(new
    {
        message = "Голос успешно принят!",
        candidate = candidate
    });

}).WithName("Проголосовать за кандидата");


app.MapPost("/api/candidates", async (Candidate newCandidate, VotingContext context) =>
{
    if (string.IsNullOrWhiteSpace(newCandidate.Name) || string.IsNullOrWhiteSpace(newCandidate.PhotoUrl))
    {
        return Results.BadRequest(new { message = "Name и PhotoUrl обязательны" });
    }
    await context.Candidates.AddAsync(newCandidate);
    await context.SaveChangesAsync();
    return Results.Created($"/api/candidates/{newCandidate.Id}", newCandidate);
}).WithName("Создать кандидата");

app.MapDelete("/api/candidates/{candidateId}", async (int candidateId, VotingContext context) =>
{
    var candidate = await context.Candidates.FindAsync(candidateId);
    if (candidate == null)
    {
        return Results.NotFound(new { message = "Кандидат не найден" });
    }
    context.Candidates.Remove(candidate);
    await context.SaveChangesAsync();
    return Results.NoContent();
}).WithName("Удалить кандидата");

app.MapPut("/api/candidates/{candidateId}", async (int candidateId, Candidate newCandidate, VotingContext context) =>
{
    var candidate = await context.Candidates.FindAsync(candidateId);
    if (candidate == null)
    {
        return Results.NotFound(new { message = "Кандидат не найден" });
    }
    candidate.Name = newCandidate.Name;
    candidate.VoteCount = newCandidate.VoteCount;
    candidate.PhotoUrl = newCandidate.PhotoUrl;
    await context.SaveChangesAsync();
    return Results.Ok(candidate);
}).WithName("Обновить кандидата");

app.Run();
