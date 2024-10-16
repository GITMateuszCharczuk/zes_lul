using lab2_pan_les.helpers;
using lab2_pan_les.Models;
using Microsoft.AspNetCore.Mvc;

namespace lab2_pan_les.Controllers;

public class TodoController : Controller
{
    private const string SessionKey = "TodoList";

    // Retrieve the TODO list from the session
    private List<TodoModel> GetTodoList()
    {
        var todoList = HttpContext.Session.GetObjectFromJson<List<TodoModel>>(SessionKey) ?? new List<TodoModel>();
        return todoList;
    }

    // Save the TODO list back to the session
    private void SaveTodoList(List<TodoModel> todoList)
    {
        HttpContext.Session.SetObjectAsJson(SessionKey, todoList);
    }

    // GET: Display the TODO list
    public IActionResult Todo()
    {
        var todoList = GetTodoList();
        return View(todoList);
    }

    // POST: Add a new task
    [HttpPost]
    public IActionResult Add(string task)
    {
        if (!string.IsNullOrEmpty(task))
        {
            var todoList = GetTodoList();
            todoList.Add(new TodoModel { Task = task, IsCompleted = false });
            SaveTodoList(todoList);
        }

        return RedirectToAction("Todo");
    }

    // POST: Complete a task
    [HttpPost]
    public IActionResult Complete(int index)
    {
        var todoList = GetTodoList();
        if (todoList != null && index >= 0 && index < todoList.Count)
        {
            todoList[index].IsCompleted = true;
            SaveTodoList(todoList);
        }
        return RedirectToAction("Todo");
    }

    // POST: Remove a task
    [HttpPost]
    public IActionResult Remove(int index)
    {
        var todoList = GetTodoList();
        if (todoList != null && index >= 0 && index < todoList.Count)
        {
            todoList.RemoveAt(index);
            SaveTodoList(todoList);
        }
        return RedirectToAction("Todo");
    }
}
