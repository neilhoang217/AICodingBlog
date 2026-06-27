using AICodingBlog.Data;
using AICodingBlog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AICodingBlog.Controllers;

[Authorize]
public class PostsController(AppDbContext db) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var posts = await db.Posts.OrderByDescending(p => p.CreatedAt).ToListAsync();
        return View(posts);
    }
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var post = await db.Posts.FindAsync(id);
        if (post is null) return NotFound();
        return View(post);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Post post)
    {
        if (!ModelState.IsValid) return View(post);
        post.CreatedAt = DateTime.UtcNow;
        db.Posts.Add(post);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var post = await db.Posts.FindAsync(id);
        if (post is null) return NotFound();
        return View(post);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Post post)
    {
        if (id != post.Id) return BadRequest();
        if (!ModelState.IsValid) return View(post);
        post.UpdatedAt = DateTime.UtcNow;
        db.Posts.Update(post);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var post = await db.Posts.FindAsync(id);
        if (post is null) return NotFound();
        return View(post);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var post = await db.Posts.FindAsync(id);
        if (post is not null)
        {
            db.Posts.Remove(post);
            await db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
