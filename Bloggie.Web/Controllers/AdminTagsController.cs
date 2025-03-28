using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Controllers
{
    public class AdminTagsController : Controller
    {
        private ITagRepository tagRepository;

       //Add Tag Functionality
        public AdminTagsController(ITagRepository tagRepository)
        {
            this.tagRepository = tagRepository;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Add")]
        public async Task<IActionResult> Add(AddTagRequest addTagRequst)
        {
            // Walk through the request object to get the form data
            //var name = Request.Form["name"];
            //var displayName = Request.Form["displayName"];
            
            var tag = new Tag() { Name = addTagRequst.Name, DisplayName = addTagRequst.DisplayName };

            await tagRepository.AddAsync(tag);

            return RedirectToAction("List");
        }


        //Read Tags
        [HttpGet]
        [ActionName("List")]
        public async Task<IActionResult> List()
        {

         var tags = await tagRepository.GetAllAsync();

            return View(tags);
        }

        //Update Tag
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            //var editTagRequest = bloggieDbContext.Tags.Find(id);
            var tag = await tagRepository.GetAsync(id);

            if (tag != null)
            {
                var editTagRequest = new EditTagRequest() { Id = tag.Id, Name = tag.Name, DisplayName = tag.DisplayName };
                return View(editTagRequest);
            }

            return View(null);
        }

        [HttpPost]
        [ActionName("Edit")]
        public async Task<IActionResult> Edit(EditTagRequest editTagRequest)
        {
            var tagToUpdate = new Tag() { Id = editTagRequest.Id, Name = editTagRequest.Name, DisplayName = editTagRequest.DisplayName };

            var existingTag = await tagRepository.UpdateAsync(tagToUpdate);

            if (existingTag != null)
            {
                //show success message
                return RedirectToAction("List");
            }
            else
            {
                //show error message
            }


            return RedirectToAction("Edit", new { id = editTagRequest.Id});
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(EditTagRequest editTagRequest)
        {
            var deletedTag = await tagRepository.DeleteAsync(editTagRequest.Id);

            if (deletedTag != null)
            {
                //show success message
                return RedirectToAction("List");

            }
            else
            {
                //show error message
            }
            return RedirectToAction("Edit", new { id = editTagRequest.Id });

        }

    }
}
