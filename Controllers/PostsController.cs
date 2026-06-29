using AICodingBlog.Data;
using AICodingBlog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AICodingBlog.Controllers;

[Authorize]
public class PostsController(AppDbContext db, ILogger<PostsController> logger) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        try
        {
            var posts = await db.Posts.OrderByDescending(p => p.CreatedAt).ToListAsync();
            return View(posts);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load blog posts.");
            ViewBag.PostsError = "Blog posts could not be loaded right now.";
            return View(Array.Empty<Post>());
        }
    }
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var post = await db.Posts.FindAsync(id);
            if (post is null) return NotFound();
            return View(post);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load blog post {PostId}.", id);
            return RedirectToAction(nameof(Index));
        }
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Post post)
    {
        if (!ModelState.IsValid) return View(post);

        try
        {
            post.CreatedAt = DateTime.UtcNow;
            db.Posts.Add(post);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create blog post.");
            ModelState.AddModelError(string.Empty, "The post could not be published right now.");
            return View(post);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var post = await db.Posts.FindAsync(id);
            if (post is null) return NotFound();
            return View(post);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load blog post {PostId} for editing.", id);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Post post)
    {
        if (id != post.Id) return BadRequest();
        if (!ModelState.IsValid) return View(post);

        try
        {
            var existingPost = await db.Posts.FindAsync(id);
            if (existingPost is null) return NotFound();

            existingPost.Title = post.Title;
            existingPost.Summary = post.Summary;
            existingPost.Content = post.Content;
            existingPost.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update blog post {PostId}.", id);
            ModelState.AddModelError(string.Empty, "The post could not be saved right now.");
            return View(post);
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var post = await db.Posts.FindAsync(id);
            if (post is null) return NotFound();
            return View(post);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load blog post {PostId} for deletion.", id);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var post = await db.Posts.FindAsync(id);
            if (post is not null)
            {
                db.Posts.Remove(post);
                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete blog post {PostId}.", id);
        }

        return RedirectToAction(nameof(Index));
    }
}
