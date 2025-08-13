using System.Net;
using System.Text.Json;
using IdeaCenter.Models;
using RestSharp;
using RestSharp.Authenticators;

namespace IdeaCenter;

public class IdeaCenter
{

    private RestClient client;
    string baseUrl = "http://softuni-qa-loadbalancer-2137572849.eu-north-1.elb.amazonaws.com:84";
    private static string lastIdeaId;

    [SetUp]
    public void Setup()
    {

        string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKd3RTZXJ2aWNlQWNjZXNzVG9rZW4iLCJqdGkiOiI0YzMxZTcyNC03ZGMxLTQxMzItYTg3MC01ZDM3NDUwNGE4MzIiLCJpYXQiOiIwOC8xMy8yMDI1IDE0OjA0OjA2IiwiVXNlcklkIjoiNDE4YjQ1MjYtZTc5Ny00NmEyLTZkNTctMDhkZGQ1YTQxM2E3IiwiRW1haWwiOiJkaWRpQGFidi5iZyIsIlVzZXJOYW1lIjoiZGFuaTExMSIsImV4cCI6MTc1NTExNTQ0NiwiaXNzIjoiSWRlYUNlbnRlcl9BcHBfU29mdFVuaSIsImF1ZCI6IklkZWFDZW50ZXJfV2ViQVBJX1NvZnRVbmkifQ.jI3AUBdbkRVnER_5dpxrVrSq_Lvz_bfYjNc_4WGMrxE";

        var options = new RestClientOptions(baseUrl)
        {
            Authenticator = new JwtAuthenticator(token)
        };

        this.client = new RestClient(options);
    }

    [Order(1)]
    [Test]
    public void CreateANewIdeaWithTheRequiredFields()
    {
        var newIdea = new IdeaDTO
        {
            Title = "NewIdeaCreated",
            Description = "Description",
            Url = ""
        };


        var request = new RestRequest("/api/Idea/Create", Method.Post);
        request.AddJsonBody(newIdea);
        var response = this.client.Execute(request);
        var newCreatedIdea = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);


        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.That(newCreatedIdea.Msg, Is.EqualTo("Successfully created!"));

    }


    [Order(2)]
    [Test]
    public void GetAllIdeas()
    {
        var request = new RestRequest("/api/Idea/All", Method.Get);
        var response = client.Execute(request);
        var ideas = JsonSerializer.Deserialize<List<ApiResponseDTO>>(response.Content);


        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        CollectionAssert.IsNotEmpty(ideas, "The list of ideas should not be empty.");

        lastIdeaId = ideas.Last().IdeaId;
        //lastIdeaId =  ideas.LastOrDefault().IdeaId;

    }

    [Order(3)]
    [Test]
    public void EditTheLastIdeaThatIsCreated()
    {
        var request = new RestRequest("/api/Idea/Edit", Method.Put);

        request.AddQueryParameter("ideaId", lastIdeaId);

        var edited = new IdeaDTO
        {

            Title = "EditedTitle",
            Description = "EditedDescription",
            Url = ""
        };

        request.AddJsonBody(edited);

        var response = client.Execute(request);
        var editedIdea = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(editedIdea.Msg, Is.EqualTo("Edited successfully"));
    }

    [Order(4)]
    [Test]
    public void DeleteTheLastIdeaThatIsCreated()
    {
        var request = new RestRequest("/api/Idea/Delete", Method.Delete);
        request.AddQueryParameter("ideaId", lastIdeaId);

        var response = client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.Content.Contains("The idea is deleted!"));
    }

    [Order(5)]
    [Test]
    public void CreateAnIdeaWithoutTheRequiredFields()
    {
        var newIdea = new IdeaDTO
        {
            Title = "",
            Description = "",
            Url = ""
        };

        var request = new RestRequest("/api/Idea/Create", Method.Post);
        request.AddJsonBody(newIdea);
        var response = this.client.Execute(request);

        
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Order(6)]
    [Test]
    public void EditANonExistingIdea()
    {
        var request = new RestRequest("/api/Idea/Edit", Method.Put);
        request.AddQueryParameter("ideaId", "non-existing-id");

        var edited = new IdeaDTO
        {
            Title = "EditedTitle",
            Description = "EditedDescription",
            Url = ""
        };

        request.AddJsonBody(edited);

        var response = client.Execute(request);

        
       Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.That(response.Content, Does.Contain("There is no such idea!"));
        Assert.That(response.Content.Contains("There is no such idea!"));
    }

    [Order(7)]
    [Test]
    public void DeleteANonExistingIdea()
    {
        var request = new RestRequest("/api/Idea/Delete", Method.Delete);
        request.AddQueryParameter("ideaId", "non-existing-id");

        var response = client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(response.Content.Contains("There is no such idea!"));
    }


























    [TearDown]
    public void Teardown()
    {
        client.Dispose();
    }
}
