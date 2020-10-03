using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using System.Net.Http.Formatting;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TodoApi.Controllers
{

    //[Route("[controller]")]
    [Route("~/")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        //private List<string> testList = new List<string>();

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        [HttpDelete("todos/completed")]
        public async Task<ActionResult<TodoItemDTO>> removeCompleted()
        {
            var itemsList = _context.TodoItems.ToList();
            foreach (var item in itemsList)
            {
                if (item.IsComplete == true)
                {
                    _context.TodoItems.Remove(item);
                }
            }
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)  // check not good
            {
                return NotFound();
            }
            JsonResult json = new JsonResult(itemsList);

            return json;

        }

        [HttpPost("list")]
        [Consumes("application/x-www-form-urlencoded")]
        public JsonResult GetListItems()
        {
            var body = _context.TodoItems.ToList();

            JsonResult json = new JsonResult(body);

            return json;
        }

        //// GET: TodoItems
        //[HttpPost("list")]
        //public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        //{
        //    return await _context.TodoItems
        //        .Select(x => ItemToDTO(x))
        //        .ToListAsync();
        //}

        //GET: TodoItems/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return ItemToDTO(todoItem);
        }

        [HttpGet("todos/{id}")]
        public async Task<string> EditItemCancel(long id, [FromForm] IFormCollection form)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            return todoItem.Name;

        }
        
        
        
        [HttpPut("todos/{id}")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<ActionResult<TodoItemDTO>> EditItem(long id, [FromForm]IFormCollection form)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            string newTitle = null;
            foreach (var key in form.Keys)
            {
                newTitle = form[key];
            }

            todoItem.Name = newTitle;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)  // check not good
            {
                return NotFound();
            }
            
            var listItems = _context.TodoItems.ToList();
            JsonResult json = new JsonResult(listItems);
            return json;


        }
        
        
        


        [HttpPut("todos/toggle_all")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<ActionResult<TodoItemDTO>> ToggleAll([FromForm]IFormCollection form)
        {

            var body = _context.TodoItems.ToList();
            string status = null;
            foreach(var key in form.Keys)
            {
                status = form[key];
            }

            if (status == "true")
            {

                foreach(var item in body)
                {
                    item.IsComplete = true;
                }
            }
            else
            {
                foreach(var item in body)
                {
                    item.IsComplete = false;
                }
            }
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)  // check not good
            {
                return NotFound();
            }


            JsonResult json = new JsonResult(body);

            return json;
            
        }




        [HttpPut("todos/{id}/toggle_status")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<ActionResult<TodoItemDTO>> UpdateTodoItem(long id, [FromForm]IFormCollection form)
        {
            
            var todoItem = await _context.TodoItems.FindAsync(id);

            string status = null;
            foreach (var key in form.Keys)
            {
                status = form[key];
            }

            if(status == "true")
            {
                todoItem.IsComplete = true;
            }
            else
            {
                todoItem.IsComplete = false;
            }




            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))  // check not good
            {
                return NotFound();
            }

            return ItemToDTO(todoItem);
        }




        [HttpPost("addTodo")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<ActionResult<TodoItemDTO>> CreateTodoItem([FromForm] IFormCollection data)
        {
            string todoName = null;


            foreach (var key in data.Keys)
            {
                todoName = data[key];
            }

            var todoItem = new TodoItem
            {
                IsComplete = false,
                Name = todoName
            };

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetTodoItem),
                new { id = todoItem.Id },
                ItemToDTO(todoItem));
        }


        [HttpDelete("todos/{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(long id) =>
             _context.TodoItems.Any(e => e.Id == id);

        private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
            new TodoItemDTO
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };
    }
}
