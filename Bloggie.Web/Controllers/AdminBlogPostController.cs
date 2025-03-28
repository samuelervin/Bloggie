using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.CompilerServices;

namespace Bloggie.Web.Controllers
{
    public class AdminBlogPostController : Controller
    {
        private ITagRepository tagRepository;
        private IBlogPostRepository blogPostRepository;

        public AdminBlogPostController(ITagRepository tagRepository, IBlogPostRepository blogPostRepository)
        {
            this.tagRepository = tagRepository;
            this.blogPostRepository = blogPostRepository;
        }


        [HttpGet]
        public async Task<IActionResult> Add()
        {
            //get all tags
            var tags = await tagRepository.GetAllAsync(); // Await the task

            var model = new BlogPostRequest
            {
                Tags = tags.Select(t => new SelectListItem
                {
                    Text = t.Name,
                    Value = t.Id.ToString()
                })
            };

            return View(model); // Pass the model to the view
        }

        [HttpPost]
        public async Task<IActionResult> Add(BlogPostRequest blogPostRequest)
        {
            var blogPost = new BlogPost()
            {
                Heading = blogPostRequest.Heading,
                PageTitle = blogPostRequest.PageTitle,
                Content = blogPostRequest.Content,
                ShortDescription = blogPostRequest.ShortDescription,
                FeaturedImageUrl = blogPostRequest.FeaturedImageUrl,
                UrlHandle = blogPostRequest.UrlHandle,
                PublishDate = blogPostRequest.PublishDate,
                Author = blogPostRequest.Author,
                Visible = blogPostRequest.Visible
            };

            //add tags to the blog post
            //get all tags -- less trips to the DB
            var tags = await tagRepository.GetAllAsync(); // Await the task

            //technically this is doing a lookup between the two lists which is like an IN statement in SQL 
            blogPost.Tags = tags.Where(t => blogPostRequest.SelectedTags.Contains(t.Id.ToString())).ToList();
            await blogPostRepository.AddAsync(blogPost);

            if(blogPost != null)
            {
                RedirectToAction("List");
            }
            //show error message
            return View(blogPostRequest);
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var model = await blogPostRepository.GetAllAsync();

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var blogPost = await blogPostRepository.GetAsync(id);
            var tags = await tagRepository.GetAllAsync(); // Await the task

            if (blogPost != null)
            {
                var blogPostRequest = new EditBlogPostRequest()
                {
                    Id = id,
                    Heading = blogPost.Heading,
                    Content = blogPost.Content,
                    PageTitle = blogPost.PageTitle,
                    Author = blogPost.Author,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    PublishDate = blogPost.PublishDate,
                    ShortDescription = blogPost.ShortDescription,
                    UrlHandle = blogPost.UrlHandle,
                    Visible = blogPost.Visible,
                    Tags = tags.Select(t => new SelectListItem
                    {
                        Text = t.Name,
                        Value = t.Id.ToString()
                    }),
                    SelectedTags = blogPost.Tags.Select(x => x.Id.ToString()).ToArray()
                };

                return View(blogPostRequest);
            }

            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditBlogPostRequest editBlogPostRequest)
        {
            //map the view model back to the domain model
            var blogPost = new BlogPost()
            {
                Id = editBlogPostRequest.Id,
                Heading = editBlogPostRequest.Heading,
                PageTitle = editBlogPostRequest.PageTitle,
                Content = editBlogPostRequest.Content,
                ShortDescription = editBlogPostRequest.ShortDescription,
                FeaturedImageUrl = editBlogPostRequest.FeaturedImageUrl,
                UrlHandle = editBlogPostRequest.UrlHandle,
                PublishDate = editBlogPostRequest.PublishDate,
                Author = editBlogPostRequest.Author,
                Visible = editBlogPostRequest.Visible
            };

            //add tags to the blog post
            //get all tags -- less trips to the DB
            var tags = await tagRepository.GetAllAsync(); // Await the task

            //technically this is doing a lookup between the two lists which is like an IN statement in SQL 
            blogPost.Tags = tags.Where(t => editBlogPostRequest.SelectedTags.Contains(t.Id.ToString())).ToList();
            //send for save

            await blogPostRepository.UpdateAsync(blogPost);

            if(blogPost != null)
            {
                //show a Success Message and redirect to action
                return RedirectToAction("List");
            }

            //Show Error Message and redirect back
            return RedirectToAction("Edit", new { id = editBlogPostRequest.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditBlogPostRequest editBlogPostRequest)
        {
            var deleteBlogPost = await blogPostRepository.DeleteAsync(editBlogPostRequest.Id);
            if (deleteBlogPost != null)
            {
                //show success notification
                return RedirectToAction("List");
            }
            //Show Error Notification
            return RedirectToAction("Edit", new {id = editBlogPostRequest.Id});
        }

    }
}
