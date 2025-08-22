namespace BlazorApp2.Tests;
using BlazorApp2.Components.Pages;
using BlazorApp2.Controller;
using BlazorApp2.Controller;
using BlazorApp2.Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Moq;
using System.Text;
using System.Text.Json;
using Xunit;
using static System.Net.WebRequestMethods;

public class Testing
{
    delegate void TryGetValueCallback(string key, out byte[] value);

    public async Task SignUp() {
        var controller = new ProductsController();

        var sessionMock = new Mock<ISession>();
        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(c => c.Session).Returns(sessionMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = contextMock.Object
        };

        var request = new LoginRequest
        {
            Username = "abc",
            Password = "abc",
            Name = "abcs"
        };

        var result = await controller.SignUp(request);

    }

    [Fact]
    public async Task SignIn_ReturnsTrue()
    {
        SignUp();
        var controller = new ProductsController();
        var sessionMock = new Mock<ISession>();

        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(c => c.Session).Returns(sessionMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = contextMock.Object
        };
        var request = new LoginRequest1
        {
            Username = "abc",
            Password = "abc"
        };

        var result = await controller.SignIn(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value);

    }

    [Fact]
    public async Task CreateNote_ReturnsTrue_WhenNoteCreated()
    {
        DeleteNote();
        var controller = new ProductsController();

        var sessionMock = new Mock<ISession>();
        sessionMock
            .Setup(s => s.TryGetValue("username", out It.Ref<byte[]>.IsAny))
            .Callback(new TryGetValueCallback((string key, out byte[] value) =>
            {
                value = Encoding.UTF8.GetBytes("abc");
            }))
            .Returns(true);

        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(c => c.Session).Returns(sessionMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = contextMock.Object
        };

        var request = new CreateNote
        {
            Title = "TestTitle",
            Note = "TestNote"
        };

        var result = await controller.CreateNote(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value);
        
    }

    [Fact]
    public async Task DeleteNote_ReturnsTrue_WhenNoteDeleted()
    {
        CreateNote();
        var controller = new ProductsController();

        var sessionMock = new Mock<ISession>();
        sessionMock
            .Setup(s => s.TryGetValue("username", out It.Ref<byte[]>.IsAny))
            .Callback(new TryGetValueCallback((string key, out byte[] value) =>
            {
                value = Encoding.UTF8.GetBytes("abc"); 
            }))
            .Returns(true);

        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(c => c.Session).Returns(sessionMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = contextMock.Object
        };

        var request = new UpdateNote
        {
            Title = "TestTitle"
        };

        var result = await controller.DeleteNote(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value); 
        
    }

    [Fact]
    public async Task GetNote_ReturnsTrue_WhenNoteFound()
    {
        CreateNote("TestTitle2","Testy");
        var controller = new ProductsController();

        var sessionMock = new Mock<ISession>();
        sessionMock
            .Setup(s => s.TryGetValue("username", out It.Ref<byte[]>.IsAny))
            .Callback(new TryGetValueCallback((string key, out byte[] value) =>
            {
                value = Encoding.UTF8.GetBytes("abc");
            }))
            .Returns(true);

        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(c => c.Session).Returns(sessionMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = contextMock.Object
        };

        var request = new UpdateNote
        {
            Title = "TestTitle2"
        };

        var result = await controller.GetNote(request);

        var okResult = Assert.IsType<OkObjectResult>(result);

    }

    [Fact]
    public async Task GetAllTitles_ReturnsTrue_WhenNoteFound()
    {
        CreateNote();
        var controller = new ProductsController();

        var sessionMock = new Mock<ISession>();
        sessionMock
            .Setup(s => s.TryGetValue("username", out It.Ref<byte[]>.IsAny))
            .Callback(new TryGetValueCallback((string key, out byte[] value) =>
            {
                value = Encoding.UTF8.GetBytes("abc");
            }))
            .Returns(true);

        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(c => c.Session).Returns(sessionMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = contextMock.Object
        };

        var result = await controller.GetAllTitles();

        var okResult = Assert.IsType<OkObjectResult>(result);

    }

    [Fact]
    public async Task SaveNotes_ReturnsTrue_WhenNoteFound()
    {
        CreateNote();
        var controller = new ProductsController();

        var sessionMock = new Mock<ISession>();
        sessionMock
            .Setup(s => s.TryGetValue("username", out It.Ref<byte[]>.IsAny))
            .Callback(new TryGetValueCallback((string key, out byte[] value) =>
            {
                value = Encoding.UTF8.GetBytes("abc");
            }))
            .Returns(true);

        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(c => c.Session).Returns(sessionMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = contextMock.Object
        };

        var request = new SaveNote
        {
            OldTitle = "TestTitle",
            Title = "TestTitle1",
            Note = "eedede"
        };

        var result = await controller.SaveNote(request);

        var okResult = Assert.IsType<OkObjectResult>(result);

    }

    [Fact]
    public async Task DeleteNote_ReturnsFalse_WhenNoteDeleted()
    {
        DeleteNote();
        var controller = new ProductsController();

        var sessionMock = new Mock<ISession>();
        sessionMock
            .Setup(s => s.TryGetValue("username", out It.Ref<byte[]>.IsAny))
            .Callback(new TryGetValueCallback((string key, out byte[] value) =>
            {
                value = Encoding.UTF8.GetBytes("abc");
            }))
            .Returns(true);

        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(c => c.Session).Returns(sessionMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = contextMock.Object
        };

        var request = new UpdateNote
        {
            Title = "TestTitle"
        };

        var result = await controller.DeleteNote(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value);

    }

    public async Task DoesTitleExist_ReturnsTrue_WhenNoteDeleted()
    {
        CreateNote();
        var controller = new ProductsController();

        var sessionMock = new Mock<ISession>();
        sessionMock
            .Setup(s => s.TryGetValue("username", out It.Ref<byte[]>.IsAny))
            .Callback(new TryGetValueCallback((string key, out byte[] value) =>
            {
                value = Encoding.UTF8.GetBytes("abc");
            }))
            .Returns(true);

        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(c => c.Session).Returns(sessionMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = contextMock.Object
        };
        string request = "TestTitle";

        var result =  controller.DoesTitleExist(request);

        Assert.True(result);

    }

    [Fact]
    public async Task Login_ReturnsTrue()
    {
        var controller = new ProductsController();

        var sessionMock = new Mock<ISession>();
        sessionMock
            .Setup(s => s.TryGetValue("isLoggedIn", out It.Ref<byte[]>.IsAny))
            .Callback(new TryGetValueCallback((string key, out byte[] value) =>
            {
                value = Encoding.UTF8.GetBytes("true");
            }))
            .Returns(true);

        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(c => c.Session).Returns(sessionMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = contextMock.Object
        };

        var result = await controller.UserLogin();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value);

    }

    [Fact]
    public async Task Login_ReturnsFalse()
    {
        var controller = new ProductsController();

        var sessionMock = new Mock<ISession>();
        sessionMock
            .Setup(s => s.TryGetValue("isLoggedIn", out It.Ref<byte[]>.IsAny))
            .Callback(new TryGetValueCallback((string key, out byte[] value) =>
            {
                value = Encoding.UTF8.GetBytes("false");
            }))
            .Returns(true);

        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(c => c.Session).Returns(sessionMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = contextMock.Object
        };

        var result = await controller.UserLogin();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True(!(bool)okResult.Value);

    }

    public async Task CreateNote()
    {
        var controller = new ProductsController();

        var sessionMock = new Mock<ISession>();
        sessionMock
            .Setup(s => s.TryGetValue("username", out It.Ref<byte[]>.IsAny))
            .Callback(new TryGetValueCallback((string key, out byte[] value) =>
            {
                value = Encoding.UTF8.GetBytes("abc");
            }))
            .Returns(true);

        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(c => c.Session).Returns(sessionMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = contextMock.Object
        };

        var request = new CreateNote
        {
            Title = "TestTitle",
            Note = "TestNote"
        };

        var result = await controller.CreateNote(request);
    }

    public async Task CreateNote(string title, string note)
    {
        var controller = new ProductsController();

        var sessionMock = new Mock<ISession>();
        sessionMock
            .Setup(s => s.TryGetValue("username", out It.Ref<byte[]>.IsAny))
            .Callback(new TryGetValueCallback((string key, out byte[] value) =>
            {
                value = Encoding.UTF8.GetBytes("abc");
            }))
            .Returns(true);

        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(c => c.Session).Returns(sessionMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = contextMock.Object
        };

        var request = new CreateNote
        {
            Title = $"{title}",
            Note = $"{note}"
        };

        var result = await controller.CreateNote(request);
    }

    public async Task DeleteNote()
    {
        var controller = new ProductsController();

        var sessionMock = new Mock<ISession>();
        sessionMock
            .Setup(s => s.TryGetValue("username", out It.Ref<byte[]>.IsAny))
            .Callback(new TryGetValueCallback((string key, out byte[] value) =>
            {
                value = Encoding.UTF8.GetBytes("abc");
            }))
            .Returns(true);

        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(c => c.Session).Returns(sessionMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = contextMock.Object
        };

        var request = new UpdateNote
        {
            Title = "TestTitle"
        };

        var result = await controller.DeleteNote(request);


    }
}

